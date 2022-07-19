using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    [SerializeField] private DamageType type;
    [SerializeField] private int damage;
    [SerializeField] private GameObject explosionPrefab;

    [SerializeField] private bool splashDamage;
    [SerializeField] private float blastRadius;
    [SerializeField] private float blastForce;

    static uint damageID = 0;

    public void TriggerDamage(RaycastHit hitInfo)
    {
        damageID++;

        DamageParams dmgParams = new DamageParams(type, damage, damageID, hitInfo);

        if (splashDamage)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius, LayerMask.GetMask("Default", "Vehicle"));
            foreach (Collider coll in colliders)
            {                
                coll.gameObject.SendMessageUpwards("Damage", dmgParams, SendMessageOptions.DontRequireReceiver);

                Rigidbody rb = coll.gameObject.GetComponent<Rigidbody>();
                if (rb != null) rb.AddExplosionForce(blastForce, transform.position, blastRadius, 4);
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
    }
}
