using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveProjectile : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float acceleration;
    [SerializeField] public float impulse;
    [SerializeField] private bool turnTowardsVelocity;
    [SerializeField] public float gravityMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * impulse, ForceMode.Impulse);        
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
        
        if(gravityMultiplier > 1.0f)
        {
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }

        if (turnTowardsVelocity && rb.velocity.magnitude > 10)
        {
            Vector3 dir = rb.velocity.normalized;
            transform.rotation = Quaternion.LookRotation(dir, transform.up);
        }
    }
}
