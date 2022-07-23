using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickupNPC : Pickup
{
    [SerializeField] private Animator anim;
    [SerializeField] private UnityEvent<GameObject> onPickedUpEvent;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
    }

    public override void OnHooked()
    {
        base.OnHooked();
        GameObject orig = anim.transform.parent.gameObject;
        anim = Instantiate(anim.gameObject, transform).GetComponent<Animator>();
        anim.SetTrigger("Sit");
        orig.SetActive(false);
        onPickedUpEvent.Invoke(orig);
    }
}
