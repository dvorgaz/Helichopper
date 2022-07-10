using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 fromCamera = (transform.position - Camera.main.transform.position).normalized;
        float angle = Vector3.SignedAngle(transform.forward, fromCamera, transform.up);
        transform.rotation = Quaternion.AngleAxis(angle, transform.up) * transform.rotation;
    }
}
