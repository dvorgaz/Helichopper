using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public abstract class GroundMovement : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected Vector3 destination = Vector3.zero;

    public UnityEvent onDestinationReached;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        agent = GetComponent<NavMeshAgent>();        
    }

    public Vector3 Destination
    {
        get { return destination; }
        set
        {
            destination = value;
            if(destination != Vector3.zero)
                agent.SetDestination(destination);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (destination != Vector3.zero && agent.isActiveAndEnabled)
        {
            Vector3 dist = destination - transform.position;
            bool inRangeHorizontal = Vector3.ProjectOnPlane(dist, Vector3.up).sqrMagnitude < agent.stoppingDistance * agent.stoppingDistance;
            bool inRangeVertical = Mathf.Abs(dist.y) < agent.stoppingDistance * 4;

            if (inRangeHorizontal && inRangeVertical)
            {
                destination = Vector3.zero;
                onDestinationReached.Invoke();
            }
        }
    }

    public virtual void Pause()
    {
        agent.isStopped = true;
    }

    public virtual void Resume()
    {
        agent.isStopped = false;
        if(destination != Vector3.zero)
            agent.SetDestination(destination);
    }
}
