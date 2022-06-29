using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    public int damage;
    public float impulseThreshold;
    public GameObject explosionPrefab;

    public bool splashDamage;
    public float blastRadius;
    public float blastForce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Impact impulse: " + collision.impulse.magnitude);
        if(collision.impulse.magnitude > impulseThreshold)
        {
            if (splashDamage)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
                foreach(Collider coll in colliders)
                {
                    Rigidbody rb = coll.gameObject.GetComponent<Rigidbody>();
                    if(rb != null) rb.AddExplosionForce(blastForce, transform.position, blastRadius, 4);

                    Health h = coll.gameObject.GetComponent<Health>();
                    if (h != null)
                    {
                        h.Damage(damage);
                        Debug.Log("BlastDamage");
                    }
                }
            }
            else
            {
                Health h = collision.gameObject.GetComponent<Health>();
                if (h != null)
                {
                    h.Damage(damage);
                    Debug.Log("Damage");
                }
            }

            if(explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);

            Debug.Log("Destroyed");
        }
    }
}
