using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDamageTaken : MonoBehaviour
{
    [SerializeField] private List<DamageHitEffect> hitEffects;

    public void Damage(DamageParams dp)
    {
        DamageHitEffect hitEffect = hitEffects.Find(x => x.type == dp.type);
        if(hitEffect != null && hitEffect.effectPrefab != null)
        {
            Instantiate(hitEffect.effectPrefab, dp.hitInfo.point, Quaternion.LookRotation(dp.hitInfo.normal));
        }
    }
}
