using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CounterMeasure : MonoBehaviour
{
    [SerializeField] private float launchForce;
    [SerializeField] private float launchSpread;
    [Range(0, 100)] public int successChance;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce((Vector3.forward + Random.insideUnitSphere * launchSpread) * launchForce, ForceMode.Impulse);

        AudioSource sfx = GetComponent<AudioSource>();
        if (sfx != null)
            sfx.pitch = sfx.pitch + Random.Range(-0.2f, 0.2f);

        TargetHoming[] missiles = GameObject.FindObjectsOfType<TargetHoming>();
        foreach (TargetHoming missile in missiles)
        {
            if (missile.IsInFov(transform.position) && Random.Range(1, 101) <= successChance)
                missile.target = transform;
        }
    }

}
