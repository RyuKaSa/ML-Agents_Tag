using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToTarget : Agent
{

    // inputs for the AI here (what data does it need)
    // pass in self position,
    // pass in target position,
    [SerializeField] private Transform targetTransform;
    // no need to pass obstacles, but will need raycast sensor

    [SerializeField, Range(0.1f, 10f)] private float moveSpeed;
    [SerializeField, Range(1f, 360f)] private float rotationSpeed;

    [SerializeField] private Material winMat;
    [SerializeField] private Material loseMat;
    [SerializeField] private MeshRenderer floorMat;

    public override void OnEpisodeBegin()
    {
        // reset the env, position of self
        transform.localPosition = new Vector3(Random.Range(-4f, +4f), 0, Random.Range(-4f, -1.5f));

        // Reset rotation
        transform.localRotation = Quaternion.identity;
        
        // Reset velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // reset position of target too
        targetTransform.localPosition = new Vector3(Random.Range(-4f, +4f), 0, Random.Range(+1.5f, +4f));

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // self position and rotation
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localRotation.y);  // 1 float

        // target position
        // sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float rotateY = actions.ContinuousActions[2];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        transform.Rotate(0, rotateY * Time.deltaTime * rotationSpeed, 0);

        AddReward(-0.001f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // we pass Target or Wall using scripts named as Target and Wall, to look for component in collider other
        if (other.TryGetComponent<Target>(out Target target))
        {
            // AddReward for incremental
            Debug.Log("+ 1");
            floorMat.material = winMat;
            SetReward(+1f);
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            Debug.Log("- 1");
            floorMat.material = loseMat;
            SetReward(-1f);
            EndEpisode();
        }
    }
}
