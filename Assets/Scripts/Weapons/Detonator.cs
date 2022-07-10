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
        float d = rb.velocity.magnitude * Time.fixedDeltaTime;
        Vector3 dir = rb.velocity.normalized;

        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position - dir * d, dir, out hitInfo, d * 2.0f, LayerMask.GetMask("Default", "Vehicle"), QueryTriggerInteraction.Ignore))
        {
            gameObject.SendMessage("TriggerDamage", hitInfo);
        }
    }
}
