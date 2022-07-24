using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Detonator : MonoBehaviour
{
    private Rigidbody rb;
    private bool armed = false;
    private Vector3 launchPos;
    [SerializeField] private float minArmDistance;

    public UnityEvent<GameObject> onImpact;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        launchPos = transform.position;
        armed = minArmDistance < 0.01f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!armed)
            armed = Vector3.Magnitude(transform.position - launchPos) > minArmDistance;

        if (armed)
        {
            float d = rb.velocity.magnitude * Time.fixedDeltaTime;
            Vector3 dir = rb.velocity.normalized;

            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position - dir * d, dir, out hitInfo, d * 2.0f, LayerMask.GetMask("Default", "Vehicle"), QueryTriggerInteraction.Ignore))
            {
                onImpact.Invoke(gameObject);
                gameObject.SendMessage("TriggerDamage", hitInfo);
                gameObject.BroadcastMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
                Destroy(gameObject);
            }
        }
    }
}
