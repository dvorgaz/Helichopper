using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;

    int health;

    public GameObject deathEffectPrefab;
    public GameObject smokeEffectPrefab;

    private uint lastDamageID = 0;

    public bool Alive
    {
        get { return health > 0; }
    }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    public void Damage(DamageParams dp)
    {
        if (!Alive)
            return;

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

        if (smokeEffectPrefab != null)
        {
            Instantiate(smokeEffectPrefab, transform);
        }

        gameObject.BroadcastMessage("OnDeath", SendMessageOptions.DontRequireReceiver);

        //Destroy(gameObject);
    }
}
