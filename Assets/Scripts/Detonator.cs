using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Detonator : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, rb.velocity.normalized, out hitInfo, rb.velocity.magnitude * Time.fixedDeltaTime, LayerMask.GetMask("Default")))
        {
            gameObject.SendMessage("TriggerDamage", hitInfo.collider);
        }
    }
}
