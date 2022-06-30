using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;

    int health;

    public GameObject deathEffectPrefab;

    private uint lastDamageID = 0;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    public void Damage(DamageParams dp)
    {
        if (lastDamageID < dp.damageID)
        {
            lastDamageID = dp.damageID;
            health -= dp.damage;

            if (health <= 0)
            {
                Kill();
            }
        }
    }

    public void Kill()
    {
        if(deathEffectPrefab !=  null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
