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

    public override void Initialize()
    {
        //some errors to be thrown if not set correctly
        if (!ballRb)
            Debug.LogError("PaddleAgent: ballRb not assigned.");
        if (!paddle)
            Debug.LogError("PaddleAgent: paddle not assigned.");
    }

    public override void OnEpisodeBegin()
    {
        stepCount = 0;
        episodeCount++;
        Debug.Log("On Episode Begin function...");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (!ballRb || !paddle)
        {
            sensor.AddObservation(new float[5]); // keep shape consistent if miswired
            return;
        }

        // positions here are normalized to between -1, 1
        float nxBallX = Mathf.Clamp(ballRb.position.x / Mathf.Max(0.001f, arenaHalfWidth), -1f, 1f);
        float nxBallY = Mathf.Clamp(
            ballRb.position.y / Mathf.Max(0.001f, arenaHalfHeight),
            -1f,
            1f
        );

        // velocities normalized
        Vector2 v = ballRb.linearVelocity;
        float nxVx = Mathf.Clamp(v.x / Mathf.Max(0.001f, maxBallSpeed), -1f, 1f);
        float nxVy = Mathf.Clamp(v.y / Mathf.Max(0.001f, maxBallSpeed), -1f, 1f);

        // paddle x position normalized
        float nxPaddleX = Mathf.Clamp(
            paddle.position.x / Mathf.Max(0.001f, arenaHalfWidth),
            -1f,
            1f
        );

        //add all observations to the sensor
        sensor.AddObservation(nxBallX);
        sensor.AddObservation(nxBallY);
        sensor.AddObservation(nxVx);
        sensor.AddObservation(nxVy);
        sensor.AddObservation(nxPaddleX);

        //simple console logging to verify
        stepCount++;
        if (logObservations && (stepCount % Mathf.Max(1, logEveryNSteps) == 0))
        {
            Debug.Log(
                $"[OBS] ball({nxBallX:F2},{nxBallY:F2}) v({nxVx:F2},{nxVy:F2}) paddleX({nxPaddleX:F2}) len=5"
            );
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(stepPenalty); //small penalty for each decision

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
}
