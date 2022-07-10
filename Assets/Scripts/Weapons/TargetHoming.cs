using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetHoming : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [SerializeField] private float turnRate;
    [SerializeField] private float fov;
    [SerializeField] private bool loftTrajectory;
    [SerializeField] private float climbRatio;
    [SerializeField] private float climbHeight;
    [SerializeField] private float climbTurnRate;

    private Rigidbody rb;
    private float initialTargetDistance;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (target == null)
        {
            GameObject tgt = GameObject.FindWithTag("Player");
            if (tgt != null)
            {
                target = tgt.transform;
            }
        }

        if(loftTrajectory && target != null)
        {
            initialTargetDistance = Vector3.Distance(transform.position, target.position);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target != null && IsInFov(target.position))
        {
            Vector3 toTarget = (target.position - transform.position).normalized;
            float adjustedTurnRate = turnRate;

            if (loftTrajectory)
            {
                float targetDistance = Vector3.Distance(transform.position, target.position);
                float distanceRatio = targetDistance / initialTargetDistance;

                if(distanceRatio > climbRatio)
                {
                    //toTarget = Vector3.ProjectOnPlane(toTarget, Vector3.up).normalized;
                    //toTarget = Quaternion.AngleAxis(climbAngle, Vector3.Cross(toTarget, Vector3.up)) * toTarget;
                    float f = 1.0f - (distanceRatio - climbRatio) / (1.0f - climbRatio);
                    toTarget = ((target.position + Vector3.up * climbHeight * f) - transform.position).normalized;
                    adjustedTurnRate = climbTurnRate;
                }
            }

            Quaternion targetRotation = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, adjustedTurnRate * Time.fixedDeltaTime);
            Vector3 newVelocity = toTarget * rb.velocity.magnitude;
            rb.velocity = Vector3.RotateTowards(rb.velocity, newVelocity, adjustedTurnRate * Mathf.Deg2Rad * Time.fixedDeltaTime, 0.0f);
        }
    }

    public bool IsInFov(Vector3 pos)
    {
        Vector3 toTarget = pos - transform.position;
        float angle = Vector3.Angle(transform.forward, toTarget);
        if (angle > fov / 2.0f)
            return false;

        return true;
    }
}
