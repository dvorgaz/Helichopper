using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Detonator : MonoBehaviour
{
    private Rigidbody rb;

    public UnityEvent<GameObject> onImpact;

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
            onImpact.Invoke(gameObject);
            gameObject.SendMessage("TriggerDamage", hitInfo);
            gameObject.BroadcastMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
    }
}
