using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    public DamageType type;
    public int damage;
    public GameObject explosionPrefab;

    public bool splashDamage;
    public float blastRadius;
    public float blastForce;

    static uint damageID = 0;

    public void TriggerDamage(RaycastHit hitInfo)
    {
        damageID++;

        DamageParams dmgParams = new DamageParams(type, damage, damageID, hitInfo);

        if (splashDamage)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius, LayerMask.GetMask("Default"));
            foreach (Collider coll in colliders)
            {
                Rigidbody rb = coll.gameObject.GetComponent<Rigidbody>();
                if (rb != null) rb.AddExplosionForce(blastForce, transform.position, blastRadius, 4);

                Debug.Log("Blast collider hit: " + coll.name);
                coll.gameObject.SendMessageUpwards("Damage", dmgParams, SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            hitInfo.collider.gameObject.SendMessageUpwards("Damage", dmgParams, SendMessageOptions.DontRequireReceiver);
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.LookRotation(hitInfo.normal));
        }

        gameObject.BroadcastMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);            
    }
}
