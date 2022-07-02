using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CounterMeasure : MonoBehaviour
{
    public float launchForce;
    [Range(0, 100)] public int successChance;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * launchForce, ForceMode.Impulse);

        TargetHoming[] missiles = GameObject.FindObjectsOfType<TargetHoming>();
        foreach (TargetHoming missile in missiles)
        {
            if (Random.Range(1, 101) <= successChance)
                missile.target = transform;
        }
    }

}
