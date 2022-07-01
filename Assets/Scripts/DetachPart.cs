using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachPart : MonoBehaviour
{
    public Transform part;

    public bool addRigidBody;
    public float mass;
    public float force;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Detach()
    {
        part.parent = null;
        part.gameObject.AddComponent<Rigidbody>();
        if (addRigidBody)
        {
            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = mass;
                rb.AddForce((Vector3.up + Random.insideUnitSphere * 1.0f) * force, ForceMode.Force);
                rb.AddTorque(Random.insideUnitSphere * force, ForceMode.Force);
            }

            Rigidbody parentRb = GetComponent<Rigidbody>();
            if (parentRb != null)
            {
                parentRb.mass = Mathf.Max(0, parentRb.mass - mass);
            }
        }

        ParticleSystem ps = part.GetComponent<ParticleSystem>();
        if(ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public void OnDeath()
    {
        Detach();
    }
}
