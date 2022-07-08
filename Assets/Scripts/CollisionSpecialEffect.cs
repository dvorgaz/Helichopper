using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CollisionSpecialEffect : MonoBehaviour
{
    [SerializeField] private AudioClip soundClip;
    private AudioSource audioSrc;

    [SerializeField] private float impulseThreshold;

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.impulse.magnitude > impulseThreshold)
        {
            audioSrc.PlayOneShot(soundClip);
        }
    }
}
