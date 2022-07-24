using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    public int Hp { get; private set; }

    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private GameObject smokeEffectPrefab;
    [SerializeField] private UnityEvent<GameObject> onDeath;

    private uint lastDamageID = 0;

    public bool Alive
    {
        get { return Hp > 0; }
    }

    // Start is called before the first frame update
    void Start()
    {
        Hp = maxHealth;
    }

    public void Damage(DamageParams dp)
    {
        if (lastDamageID < dp.damageID)
        {
            if (dp.damageCallback != null)
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                    dp.damageCallback.Invoke(rb);
            }

            if (!Alive)
                return;

            lastDamageID = dp.damageID;
            Hp -= dp.damage;

            if (Hp <= 0)
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
        onDeath.Invoke(gameObject);
        //Destroy(gameObject);
    }

    public void Heal()
    {
        Hp = maxHealth;
    }
}
