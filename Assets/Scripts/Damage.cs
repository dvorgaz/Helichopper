using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Bullet,
    Explosive,
    ArmorPiercing
}

public class DamageParams
{
    public int damage;
    public uint damageID;
    public DamageType type;
    public RaycastHit hitInfo;

    public DamageParams(DamageType type, int damage, uint damageID, RaycastHit hitInfo)
    {
        this.type = type;
        this.damage = damage;
        this.damageID = damageID;
        this.hitInfo = hitInfo;
    }
}

[System.Serializable]
public class DamageHitEffect
{
    public DamageType type;
    public GameObject effectPrefab;
}