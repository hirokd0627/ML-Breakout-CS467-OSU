using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RollerAgent : Agent
{
    public Transform target;
    Rigidbody2D rBody;
    public BoxCollider2D floorCollider;

    public override void Initialize()
    {
        this.rBody = GetComponent<Rigidbody2D>();
    }

    public override void OnEpisodeBegin()
    {
        if (IsOutOfFloor)
        {
            this.rBody.angularVelocity = 0.0f;
            this.rBody.linearVelocity = Vector2.zero;
            this.transform.localPosition = new Vector2(0.0f, 0.0f);
        }

        target.localPosition = new Vector2(Random.value * 8 - 4, Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition.x);
        sensor.AddObservation(target.localPosition.y);
        sensor.AddObservation(this.transform.localPosition.x);
        sensor.AddObservation(this.transform.localPosition.y);
        sensor.AddObservation(rBody.linearVelocity.x);
        sensor.AddObservation(rBody.linearVelocity.y);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector2 controlSignal = Vector2.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.y = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * 10);

        float distanceToTarget = Vector2.Distance(
            this.transform.localPosition,
            target.localPosition
        );
        if (distanceToTarget < 1.0f)
        {
            AddReward(1.0f);
            EndEpisode();
        }

        if (IsOutOfFloor)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionBuffers)
    {
        var actionsOut = actionBuffers.ContinuousActions;
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }

    private bool IsOutOfFloor
    {
        get
        {
            if (floorCollider == null)
            {
                return true;
            }
            return !floorCollider.bounds.Contains(transform.position);
        }
    }
}
