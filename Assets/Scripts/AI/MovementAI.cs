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
    private Vector3 startingPosition;
    [SerializeField] private bool idleMovement;
    [SerializeField] private float idleRadius;
    [SerializeField] private Vector2 idlePauseTime;
    private bool isMoving;

    private void Awake()
    {
        groundMovement = GetComponent<GroundMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;

        if (patrolRoute != null)
        {
            patrol = patrolRoute.GetTracker();

            if(groundMovement != null)
            {
                MoveTo(patrol.SteerPoint);
            }
        }

        if(idleMovement)
        {
            StartCoroutine(IdleCoroutine());
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
        isMoving = false;

        if (patrol != null)
        {
            patrol.Next();
            MoveTo(patrol.SteerPoint);
        }
    }

    void MoveTo(Vector3 pos)
    {
        isMoving = true;
        groundMovement.Destination = pos;
    }

    IEnumerator IdleCoroutine()
    {
        while(true)
        {
            if(!isMoving)
            {
                yield return new WaitForSeconds(Random.Range(idlePauseTime.x, idlePauseTime.y));
                MoveTo(startingPosition + Vector3.ProjectOnPlane(Random.insideUnitSphere, Vector3.up) * idleRadius);
            }

            yield return null;
        }
    }

    public void OnDeath()
    {
        StopAllCoroutines();
        enabled = false;
    }
}
