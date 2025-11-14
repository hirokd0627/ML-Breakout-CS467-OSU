using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PaddleAgent : Agent
{
    // tuning for rewards
    public float rewardHitBrick = 2f;
    public float rewardHitPaddle = 0.05f;
    public float rewardMiss = -3f;
    public float stepPenalty = -0.0005f;

    // Used to regenerate bricks when all bricks are destroyed
    public BrickGeneration cpuBrickGeneration;
    private int currentBrickCount = 0;

    // Used to set the ball's position when starting new episode
    public Ball cpuBall;

    // called from external codes
    public void AddBrickReward()
    {
        AddReward(rewardHitBrick);
        Debug.Log(
            $"[PaddleAgent] AddBrickReward {rewardHitBrick} totalReward={GetCumulativeReward()}",
            this
        );
    }

    public void AddPaddleHitReward()
    {
        AddReward(rewardHitPaddle);
        Debug.Log(
            $"[PaddleAgent] AddPaddleHitReward {rewardHitPaddle} totalReward={GetCumulativeReward()}",
            this
        );
    }

    public void AddMissPenaltyAndEnd()
    {
        AddReward(rewardMiss);
        Debug.Log($"[PaddleAgent] Miss penalty {rewardMiss} -> EndEpisode()", this);
        EndEpisode();

        // If training, regenerate bricks and reset ball's position for new episode.
        if (PaddleAgent.IsTraining())
        {
            cpuBrickGeneration.ResetBricks(); // regenerate bricks
            cpuBall.LaunchBall(); // reset ball position
        }
    }

    public Rigidbody2D ballRb;
    public Transform paddle;
    public float arenaHalfWidth = 8.8f; //full span of cpu side since center is 0

    public float arenaHalfHeight = 4.5f; //for normialization, half of the y height span of cpu side
    public float paddleMoveMaxX = 7.5f; // Maximum X position of the paddle
    public float paddleMoveMinX = 1.4f; // Minimum X position of the paddle
    public float paddleMoveSpeed = 18; // Paddle move speed

    public float maxBallSpeed = 5f; // fasted expected speed

    public bool logObservations = false; // for simple logging reasons only

    public int logEveryNSteps = 25;
    public int stepCount;
    public int episodeCount = 0;

    [Header("Training Parameters")]
    // Round trip of ball is about 25 steps and 84 blocks.
    // 25 * 84 = 2100, set 3000 as initial value, leaving some margin.
    public int maxStepsPerEpisode = 3000;

    public override void Initialize()
    {
        //some errors to be thrown if not set correctly
        if (!ballRb)
            Debug.LogError("PaddleAgent: ballRb not assigned.");
        if (!paddle)
            Debug.LogError("PaddleAgent: paddle not assigned.");
        if (!cpuBrickGeneration)
            Debug.LogError("PaddleAgent: cpuBrickGeneration not assigned.");
        if (!cpuBall)
            Debug.LogError("PaddleAgent: cpuBall not assigned.");

        // Add an event to get bricks count
        cpuBrickGeneration.OnGenerationComplete += OnBrickGenerationComplete;
    }

    public override void OnEpisodeBegin()
    {
        stepCount = 0;
        episodeCount++;
        Debug.Log("On Episode Begin function...");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // New observation vector (14 floats):
        // 0: nxBallX (ball X pos normalized to arenaHalfWidth)
        // 1: nxBallY (ball Y pos normalized to arenaHalfHeight)
        // 2: nxVx (ball X velocity normalized)
        // 3: nxVy (ball Y velocity normalized)
        // 4: nxPaddleX (paddle X pos normalized)
        // 5: distPaddleBall (distance paddle->ball normalized)
        // 6: relBallX (ball X relative to paddle normalized by arenaHalfWidth)
        // 7: relBallY (ball Y relative to paddle normalized by arenaHalfHeight)
        // 8: dirToBallX (unit vector from paddle to ball, x)
        // 9: dirToBallY (unit vector from paddle to ball, y)
        // 10: leftDistNorm (distance from left wall to paddle left edge normalized)
        // 11: rightDistNorm (distance from right wall to paddle right edge normalized)
        // 12: nxBallSpeed (ball speed magnitude normalized)
        // 13: ballMovingDown (1 if ball moving downwards, 0 otherwise)

        if (!ballRb || !paddle)
        {
            sensor.AddObservation(new float[14]); // keep shape consistent if miswired
            return;
        }

        // basic normalized positions and velocities
        float nxBallX = Mathf.Clamp(ballRb.position.x / Mathf.Max(0.001f, arenaHalfWidth), -1f, 1f);
        float nxBallY = Mathf.Clamp(
            ballRb.position.y / Mathf.Max(0.001f, arenaHalfHeight),
            -1f,
            1f
        );

        Vector2 v = ballRb.linearVelocity;
        float nxVx = Mathf.Clamp(v.x / Mathf.Max(0.001f, maxBallSpeed), -1f, 1f);
        float nxVy = Mathf.Clamp(v.y / Mathf.Max(0.001f, maxBallSpeed), -1f, 1f);

        float nxPaddleX = Mathf.Clamp(
            paddle.position.x / Mathf.Max(0.001f, arenaHalfWidth),
            -1f,
            1f
        );

        // compute paddle half-width using collider or sprite bounds if available
        float paddleHalfWidth = 0.5f; // fallback
        Collider2D col = paddle.GetComponent<Collider2D>();
        if (col != null)
        {
            paddleHalfWidth = col.bounds.size.x * 0.5f;
        }
        else
        {
            var sr = paddle.GetComponent<SpriteRenderer>();
            if (sr != null)
                paddleHalfWidth = sr.bounds.size.x * 0.5f;
            else
                paddleHalfWidth = Mathf.Abs(paddle.localScale.x) * 0.5f; // last resort guess
        }

        // distance paddle -> ball (Euclidean) normalized by largest arena half-dimension * 2 (approx full span)
        float maxHalf = Mathf.Max(arenaHalfWidth, arenaHalfHeight);
        float rawDist = Vector2.Distance(ballRb.position, paddle.position);
        float distPaddleBall = Mathf.Clamp(rawDist / (2f * maxHalf), 0f, 1f);

        // ball relative to paddle (signed), normalized
        float relBallX = Mathf.Clamp(
            (ballRb.position.x - paddle.position.x) / Mathf.Max(0.001f, arenaHalfWidth),
            -1f,
            1f
        );
        float relBallY = Mathf.Clamp(
            (ballRb.position.y - paddle.position.y) / Mathf.Max(0.001f, arenaHalfHeight),
            -1f,
            1f
        );

        // direction from paddle center to ball (unit vector)
        Vector2 dirToBall = (ballRb.position - (Vector2)paddle.position);
        if (dirToBall == Vector2.zero)
            dirToBall = Vector2.up; // avoid zero
        dirToBall.Normalize();

        // distance from walls to paddle edges (no raycast) � compute in world units then normalize by full arena width
        // left wall at -arenaHalfWidth, right wall at +arenaHalfWidth
        float leftEdgeX = paddle.position.x - paddleHalfWidth;
        float rightEdgeX = paddle.position.x + paddleHalfWidth;
        float leftDist = leftEdgeX - (-arenaHalfWidth); // >= 0
        float rightDist = arenaHalfWidth - rightEdgeX; // >= 0
        // normalize by full arena width (2 * arenaHalfWidth) to roughly map to 0..1
        float arenaFullWidth = Mathf.Max(0.001f, 2f * arenaHalfWidth);
        float leftDistNorm = Mathf.Clamp(leftDist / arenaFullWidth, 0f, 1f);
        float rightDistNorm = Mathf.Clamp(rightDist / arenaFullWidth, 0f, 1f);

        // ball speed normalized
        float nxBallSpeed = Mathf.Clamp(v.magnitude / Mathf.Max(0.001f, maxBallSpeed), 0f, 1f);

        // ball moving downwards flag
        float ballMovingDown = v.y < 0f ? 1f : 0f;

        // add observations in the order documented above
        sensor.AddObservation(nxBallX); // 0
        sensor.AddObservation(nxBallY); // 1
        sensor.AddObservation(nxVx); // 2
        sensor.AddObservation(nxVy); // 3
        sensor.AddObservation(nxPaddleX); // 4
        sensor.AddObservation(distPaddleBall); // 5
        sensor.AddObservation(relBallX); // 6
        sensor.AddObservation(relBallY); // 7
        sensor.AddObservation(dirToBall.x); // 8
        sensor.AddObservation(dirToBall.y); // 9
        sensor.AddObservation(leftDistNorm); // 10
        sensor.AddObservation(rightDistNorm); // 11
        sensor.AddObservation(nxBallSpeed); // 12
        sensor.AddObservation(ballMovingDown); // 13

        //simple console logging to verify
        stepCount++;
        if (logObservations && (stepCount % Mathf.Max(1, logEveryNSteps) == 0))
        {
            Debug.Log(
                $"[OBS] ball({nxBallX:F2},{nxBallY:F2}) v({nxVx:F2},{nxVy:F2}) paddleX({nxPaddleX:F2}) distPB({distPaddleBall:F2}) rel({relBallX:F2},{relBallY:F2}) dir({dirToBall.x:F2},{dirToBall.y:F2}) leftR({leftDistNorm:F2}) rightR({rightDistNorm:F2}) speed({nxBallSpeed:F2}) down({ballMovingDown}) len=14"
            );
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(stepPenalty); //small penalty for each decision

        // End the game if ball cannot break last brick and keeps hitting wall, to prevent learning from stagnating.
        if (stepCount >= maxStepsPerEpisode)
        {
            StartNewEpisode();
            return;
        }

        if (actions.ContinuousActions.Length == 0) // no actions
            return;

        float horizMovement = actions.ContinuousActions[0];

        // Determines next paddle position.
        // Since this function called at fixed intervals of DecisionRequester's DecisionPeriod,
        // use fixedDeltaTime.
        Vector3 nextPos = PaddleMovement.NextPaddlePosition(
            horizMovement,
            paddle,
            paddleMoveMinX,
            paddleMoveMaxX,
            paddleMoveSpeed,
            Time.fixedDeltaTime
        );

        // Sets the paddle position
        paddle.position = nextPos;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // To control agent, set user input into action buffers.
        if (!PaddleMovement.IsHumanInput())
        {
            var actions = actionsOut.ContinuousActions;
            if (actions.Length < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actionsOut),
                    "ContinuousActions should be set to 1 in Behavior Parameters"
                );
            }
            actions[0] = PaddleMovement.GetMovementInput();
        }
    }

    // An action called when brick generation is completed
    private void OnBrickGenerationComplete(int brickCount)
    {
        currentBrickCount = brickCount;
    }

    // Called when a CPU's brick is destroyed
    public void DecrementBrickCount()
    {
        currentBrickCount -= 1;
        if (currentBrickCount == 0)
        {
            StartNewEpisode();
        }
    }

    // Regenerate bricks to start new episode
    private void StartNewEpisode()
    {
        EndEpisode();
        cpuBrickGeneration.ResetBricks(); // regenerate bricks
        cpuBall.LaunchBall(); // reset ball position
    }

    // Returns whether agent is training
    public static bool IsTraining()
    {
        // If training, communicating with Python
        bool isTraining = Academy.Instance.IsCommunicatorOn;
        return isTraining;
    }
}
