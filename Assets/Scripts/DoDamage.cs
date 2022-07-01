using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageParams
{
    public int damage;
    public uint damageID;

    public DamageParams(int damage, uint damageID)
    {
        this.damage = damage;
        this.damageID = damageID;
    }
}

public class DoDamage : MonoBehaviour
{
    public int damage;
    public GameObject explosionPrefab;

    public bool splashDamage;
    public float blastRadius;
    public float blastForce;

    static uint damageID = 0;

    public void TriggerDamage(Collider other)
    {
        if (true)
        {
            damageID++;

            if (splashDamage)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius, LayerMask.GetMask("Default"));
                foreach (Collider coll in colliders)
                {
                    Rigidbody rb = coll.gameObject.GetComponent<Rigidbody>();
                    if (rb != null) rb.AddExplosionForce(blastForce, transform.position, blastRadius, 4);

                    Debug.Log("Blast collider hit: " + coll.name);
                    coll.gameObject.SendMessageUpwards("Damage", new DamageParams(damage, damageID), SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
                other.gameObject.SendMessageUpwards("Damage", new DamageParams(damage, damageID), SendMessageOptions.DontRequireReceiver);
            }

            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            gameObject.BroadcastMessage("OnDeath");
            Destroy(gameObject);            
        }
    }
}
