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

    public float rotSpeed;

    int patrolIdx = 0;
    public List<Transform> patrolRoute;

    private bool shouldDoStuff = true;

    public Transform bottomPosition;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        agent.updateRotation = false;
        agent.updatePosition = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!shouldDoStuff)
            return;

        if (patrolRoute.Count > 0 && agent.isActiveAndEnabled && agent.remainingDistance < agent.stoppingDistance)
        {
            agent.SetDestination(patrolRoute[patrolIdx].position);
            patrolIdx = (patrolIdx + 1) % patrolRoute.Count;
        }
    }

    private void FixedUpdate()
    {
        if (!shouldDoStuff)
            return;

        Vector3 vel = transform.forward * agent.desiredVelocity.magnitude;
        Vector3 newPos = transform.position + vel * Time.fixedDeltaTime;
        Vector3 newUp = Vector3.up;

        RaycastHit hit;        
        if (Physics.Raycast(bottomPosition.position + Vector3.up * 0.1f, -Vector3.up, out hit, 20.0f))
        {
            newPos = hit.point + newPos - bottomPosition.position;
            newUp = Vector3.RotateTowards(transform.up, hit.normal, rotSpeed * Time.fixedDeltaTime, 1.0f);
        }

        Vector3 targetDir = (agent.steeringTarget - transform.position).normalized;
        targetDir = Vector3.ProjectOnPlane(targetDir, transform.up);
        targetDir = Vector3.RotateTowards(transform.forward, targetDir, agent.angularSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 1.0f);

        rb.MoveRotation(Quaternion.FromToRotation(transform.forward, targetDir) * Quaternion.FromToRotation(transform.up, newUp) * transform.rotation);
        rb.MovePosition(newPos);

        agent.nextPosition = rb.position;
    }

    public void OnDeath()
    {
        shouldDoStuff = false;
        agent.enabled = false;
        rb.isKinematic = false;
    }
}
