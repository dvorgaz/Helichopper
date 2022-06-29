using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;

    int health;

    public GameObject deathEffectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    public void Damage(int dmg)
    {
        health -= dmg;

        if(health <= 0)
        {
            Kill();
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
