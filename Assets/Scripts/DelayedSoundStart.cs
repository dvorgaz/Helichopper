using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DelayedSoundStart : MonoBehaviour
{
    public float delay;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(StartSound), delay);
    }

    void StartSound()
    {
        GetComponent<AudioSource>().Play();
    }
}
