using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Winch : MonoBehaviour
{
    private Transform hookNode;
    private Rope rope;
    private int pickupsInRange = 0;
    private Pickup pickedItem;
    private AudioSource audioSrc;
    [SerializeField] private float hookRadius;
    [SerializeField] private AudioClip hookConnectSfx;
    [SerializeField] private AudioClip pickupSfx;
    [SerializeField] private float stayExtendedTime;
    [SerializeField] private GameObject hookPrefab;
    private float lastExtendedTime;
    private bool delayedRetract = false;

    // Start is called before the first frame update
    void Start()
    {        
        rope = GetComponentInChildren<Rope>();
        hookNode = rope.GetEndNode();
        rope.gameObject.SetActive(false);
        rope.OnRetracted += OnRopeRetracted;
        rope.OnExtended += OnRopeExtended;
        audioSrc = GetComponent<AudioSource>();

        Instantiate(hookPrefab, hookNode);
    }

    // Update is called once per frame
    void Update()
    {
        if (delayedRetract && (Time.time - lastExtendedTime > stayExtendedTime))
        {
            Retract();            
        }
    }

    private void FixedUpdate()
    {
        if(pickupsInRange > 0 && pickedItem == null)
        {
            foreach(Collider coll in Physics.OverlapSphere(hookNode.position, hookRadius))
            {
                if(coll.CompareTag("Pickup"))
                {
                    Pickup item = coll.GetComponent<Pickup>();
                    if(item != null)
                    {
                        AttachItem(item);
                        break;
                    }
                }
            }
        }
    }

    private void AttachItem(Pickup item)
    {
        rope.PickupConstraint = item.transform.position + item.AttachOffset;
        pickedItem = item;
        pickedItem.transform.position = hookNode.position - item.AttachOffset;
        pickedItem.transform.parent = hookNode;
        pickedItem.OnHooked();
        Debug.Log("Item: " + pickedItem.gameObject.name + " picked up");

        audioSrc.PlayOneShot(hookConnectSfx);
        Retract();
    }

    private void Extend()
    {
        delayedRetract = false;
        lastExtendedTime = Time.time;
        rope.gameObject.SetActive(true);
        rope.TargetPosition = 1.0f;

        if (rope.Position < 0.9f)
            audioSrc.Play();
    }

    private void Retract()
    {
        delayedRetract = false;
        rope.TargetPosition = 0.0f;

        if (rope.Position > 0.1f)
            audioSrc.Play();
    }

    private void OnRopeRetracted()
    {
        rope.gameObject.SetActive(false);
        audioSrc.Stop();

        if (pickedItem != null)
        {
            SendMessageUpwards("OnItemPickedUp", pickedItem);
            Destroy(pickedItem.gameObject);
            pickedItem = null;

            audioSrc.PlayOneShot(pickupSfx);

            if(pickupsInRange > 0)
            {
                Extend();
            }
        }        
    }

    private void OnRopeExtended()
    {
        audioSrc.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Pickup") && pickedItem == null)
        {
            Debug.Log("Pickup in range");

            if (pickupsInRange < 0)
                pickupsInRange = 0;

            pickupsInRange++;
            Extend();
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            Debug.Log("Pickup out of range");

            pickupsInRange--;

            lastExtendedTime = Time.time;
            delayedRetract = true;
        }        
    }
}
