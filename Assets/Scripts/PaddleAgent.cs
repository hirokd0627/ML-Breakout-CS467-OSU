using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PaddleAgent : Agent
{
    public Rigidbody2D ballRb;
    public Transform paddle;
    public float arenaHalfWidth = 8.8f; //full span of cpu side since center is 0

    public float arenaHalfHeight = 4.5f; //for normialization, half of the y height span of cpu side

    public float maxBallSpeed = 5f; // fasted expected speed

    public bool logObservations = false; // for simple logging reasons only

    public int logEveryNSteps = 25;
    private int stepCount;

    public override void Initialize()
    {
        //some errors to be thrown if not set correctly
        if (!ballRb)
            Debug.LogError("PaddleAgent: ballRb not assigned.");
        if (!paddle)
            Debug.LogError("PaddleAgent: paddle not assigned.");

        //NOTE: the initialize method will be used more later on...
    }

    public override void OnEpisodeBegin()
    {
        stepCount = 0;
        Debug.Log("On Episode Begin function...");
        //TBD Placehold method for now
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
        //TBD Placehold method for now
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //TBD Placehold method for now
    }
}
