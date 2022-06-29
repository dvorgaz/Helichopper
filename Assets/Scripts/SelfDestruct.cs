using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float time;

    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - startTime > time)
        {
            Health h = GetComponent<Health>();
            if(h != null)
            {
                h.Kill();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
