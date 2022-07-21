using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventUtils : MonoBehaviour
{
    public void PlaySound()
    {
        GetComponent<AudioSource>().Play();
    }
}
