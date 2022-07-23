using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class MovementAI : MonoBehaviour
{
    [SerializeField] private PatrolRoute patrolRoute;
    private PatrolRoute.Tracker patrol;
    private GroundMovement groundMovement;

    private void Awake()
    {
        groundMovement = GetComponent<GroundMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {        
        if (patrolRoute != null)
        {
            patrol = patrolRoute.GetTracker();

            if(groundMovement != null)
            {
                groundMovement.Destination = patrol.SteerPoint;
            }
        }
    }

    private void OnEnable()
    {
        if (groundMovement != null)
        {
            groundMovement.onDestinationReached.AddListener(OnDestinationReached);
        }
    }

    private void OnDisable()
    {
        if (groundMovement != null)
        {
            groundMovement.onDestinationReached.RemoveListener(OnDestinationReached);
        }
    }

    public void OnDestinationReached()
    {
        patrol.Next();
        groundMovement.Destination = patrol.SteerPoint;
    }
}
