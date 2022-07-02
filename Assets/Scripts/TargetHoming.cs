using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetHoming : MonoBehaviour
{
    [HideInInspector] public Transform target;
    public float turnRate;
    public float fov;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject tgt = GameObject.FindWithTag("Player");
        if(tgt != null)
        {
            target = tgt.transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target != null && IsInFov(target.position))
        {
            Vector3 toTarget = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnRate * Time.fixedDeltaTime);
            Vector3 newVelocity = toTarget * rb.velocity.magnitude;
            rb.velocity = Vector3.RotateTowards(rb.velocity, newVelocity, turnRate * Mathf.Deg2Rad * Time.fixedDeltaTime, 0.0f);
        }
    }

    bool IsInFov(Vector3 pos)
    {
        Vector3 toTarget = target.position - transform.position;
        float angle = Vector3.Angle(transform.forward, toTarget);
        if (angle > fov / 2.0f)
            return false;

        return true;
    }
}
