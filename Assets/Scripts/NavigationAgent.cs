using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class NavigationAgent : Agent
{
    [SerializeField]
    float speed;
    [SerializeField]
    Transform target;

    private float distFromTarget;

    public override void OnEpisodeBegin()
    {
        distFromTarget = Vector3.Distance(target.localPosition, transform.localPosition);
        transform.localPosition = new Vector3(Random.Range(-3.0f, 3.0f), 1.5f, Random.Range(-3.0f, 3.0f));
        target.localPosition = new Vector3(Random.Range(-3.0f, 3.0f), 1.0f, Random.Range(-3.0f, 3.0f));

        /*GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
        foreach(GameObject obst in obstacles)
        {
            obst.transform.localPosition = new Vector3(Random.Range(-3.0f, 3.0f), 0.5f, Random.Range(-3.0f, 3.0f));
        }*/
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        float moveX = vectorAction[0];
        float moveZ = vectorAction[1];

        transform.localPosition += new Vector3(moveX, 0.0f, moveZ) * speed * Time.deltaTime;
        
        float currDist = Vector3.Distance(target.localPosition, transform.localPosition);
        if(currDist < distFromTarget)
        {
            AddReward(0.1f);
        }
        else
        {
            AddReward(-0.1f);
        }
        distFromTarget = currDist;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("goal"))
        {
            SetReward(1.0f);
            Debug.Log("WIN!");
            EndEpisode();
        }
        if ((other.gameObject.CompareTag("wall")) || (other.gameObject.CompareTag("obstacle")))
        {
            SetReward(-1.0f);
            Debug.Log("LOSE!");
            EndEpisode();
        }
    }
}
