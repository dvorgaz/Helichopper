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

    // Start is called before the first frame update
    void Start()
    {        
        rope = GetComponentInChildren<Rope>();
        hookNode = rope.GetEndNode();
        rope.gameObject.SetActive(false);
        rope.OnRetracted += OnRopeRetracted;
        rope.OnExtended += OnRopeExtended;
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V) && pickedItem != null)
        {
            pickedItem.transform.parent = null;
            pickedItem.GetComponent<Rigidbody>().isKinematic = false;
            pickedItem = null;
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
                        pickedItem = item;
                        pickedItem.transform.position = hookNode.position;
                        pickedItem.transform.parent = hookNode;
                        pickedItem.OnHooked();
                        Debug.Log("Item: " + pickedItem.gameObject.name + " picked up");

                        audioSrc.PlayOneShot(hookConnectSfx);
                        Retract();

                        break;
                    }
                }
            }
        }
    }

    private void Extend()
    {
        rope.gameObject.SetActive(true);
        rope.TargetPosition = 1.0f;

        audioSrc.pitch = 0.95f;
        audioSrc.Play();
    }

    private void Retract()
    {
        rope.TargetPosition = 0.0f;

        audioSrc.pitch = 1.0f;
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

            if (pickupsInRange <= 0 && pickedItem == null)
            {
                Retract();
            }
        }        
    }
}
