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
        agent.updateRotation = false;
        agent.updatePosition = false;
        agent.updateUpAxis = false;
    }

    public Vector3 Destination
    {
        get { return destination; }
        set
        {
            destination = value;
            agent.SetDestination(destination);
        }
    }
}
