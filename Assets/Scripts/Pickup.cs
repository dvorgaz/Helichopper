using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickup : MonoBehaviour
{
    [SerializeField] protected float attachOffsetY;    
    protected Rigidbody rb;

    public InventoryItem content;

    public Vector3 AttachOffset
    {
        get { return transform.up * attachOffsetY; }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void OnHooked()
    {
        rb.isKinematic = true;
    }
}
