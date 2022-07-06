using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class GroundMoveAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;

    int patrolIdx = 0;
    public List<Transform> patrolRoute;

    private bool moveEnabled = true;

    private Transform[] wheelOffsets;
    private Vector3 tiltVelocity = Vector3.zero;
    private Vector3 turnVelocity = Vector3.zero;

    public float tiltDampTime;
    public float turnDampTime;

    private void Awake()
    {
        Transform wheelOffsetFront = transform.Find("WheelOffsetFront");
        Transform wheelOffsetRear = transform.Find("WheelOffsetRear");

        if (wheelOffsetFront != null && wheelOffsetRear != null)
        {
            wheelOffsets = new Transform[] { wheelOffsetFront, wheelOffsetRear };
        }
        else
        {
            Debug.LogError("Object: " + gameObject.name + " missing WheelOffset nodes");
        }
    }

    // Start is called before the first frame update
    void Start()
    {        
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updatePosition = false;
        agent.updateUpAxis = false;

        if (patrolRoute.Count > 0 )
            agent.SetDestination(patrolRoute[patrolIdx].position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!moveEnabled)
            return;

        if (patrolRoute.Count > 0 && agent.isActiveAndEnabled)
        {
            Vector3 dist = patrolRoute[patrolIdx].position - transform.position;
            bool inRangeHorizontal = Vector3.ProjectOnPlane(dist, Vector3.up).sqrMagnitude < agent.stoppingDistance * agent.stoppingDistance;
            bool inRangeVertical = Mathf.Abs(dist.y) < agent.stoppingDistance * 4;

            if (inRangeHorizontal && inRangeVertical)
            {
                patrolIdx = (patrolIdx + 1) % patrolRoute.Count;
                agent.SetDestination(patrolRoute[patrolIdx].position);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!moveEnabled)
            return;

        Vector3 vel = transform.forward * agent.desiredVelocity.magnitude;
        Vector3 newPos = transform.position + vel * Time.fixedDeltaTime;
        Vector3 newUp = Vector3.up;

        if(wheelOffsets != null)
        {
            const float yOffset = 4.0f;
            const float maxDist = 20.0f;
            int mask = LayerMask.GetMask("Default");

            RaycastHit hit1;
            if (Physics.Raycast(wheelOffsets[0].position + Vector3.up * yOffset, -Vector3.up, out hit1, maxDist, mask))
            {
                RaycastHit hit2;
                if (Physics.Raycast(wheelOffsets[1].position + Vector3.up * yOffset, -Vector3.up, out hit2, maxDist, mask))
                {
                    Vector3 axis = (hit1.point - hit2.point).normalized;

                    Vector3 avgNormal = (Vector3.ProjectOnPlane(hit1.normal, axis).normalized + Vector3.ProjectOnPlane(hit2.normal, axis).normalized) / 2;
                    newUp = Vector3.SmoothDamp(transform.up, avgNormal, ref tiltVelocity, tiltDampTime);
                    newPos = (hit1.point + hit2.point) / 2 + newPos - (wheelOffsets[0].position + wheelOffsets[1].position) / 2;
                }
            }
        }

        Vector3 targetDir = agent.desiredVelocity.normalized;
        targetDir = Vector3.ProjectOnPlane(targetDir, transform.up);
        targetDir = Vector3.SmoothDamp(transform.forward, targetDir, ref turnVelocity, turnDampTime); //Vector3.RotateTowards(transform.forward, targetDir, agent.angularSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 1.0f);

        rb.MoveRotation(Quaternion.FromToRotation(transform.forward, targetDir) * Quaternion.FromToRotation(transform.up, newUp) * transform.rotation);
        rb.MovePosition(newPos);

        agent.nextPosition = rb.position;
    }

    public void OnDeath()
    {
        moveEnabled = false;
        agent.enabled = false;
        rb.isKinematic = false;
    }
}
