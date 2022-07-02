using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CollisionSpecialEffect : MonoBehaviour
{
    public AudioClip soundClip;
    private AudioSource audio;

    public float impulseThreshold;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.impulse.magnitude > impulseThreshold)
        {
            audio.PlayOneShot(soundClip);
        }
    }
}
