using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float offset;
    public float smoothParam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 dir = target.forward;
            dir.y = 0;
            //transform.position = target.position + dir.normalized * offset;
            transform.position = Vector3.Lerp(transform.position, target.position + dir.normalized * offset, smoothParam * Time.deltaTime);
        }
    }
}
