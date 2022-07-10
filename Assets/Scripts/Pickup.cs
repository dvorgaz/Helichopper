using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickup : MonoBehaviour
{
    [SerializeField] private float attachOffsetY;
    private Rigidbody rb;

    public Vector3 AttachOffset
    {
        get { return transform.up * attachOffsetY; }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHooked()
    {
        rb.isKinematic = true;
    }
}
