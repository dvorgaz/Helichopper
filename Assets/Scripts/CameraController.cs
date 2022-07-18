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
        FindTargetIfInvalid();

        if (IsTargetValid())
        {
            Vector3 dir = target.forward;
            dir.y = 0;
            transform.position = target.position + dir.normalized * offset;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        FindTargetIfInvalid();

        if (IsTargetValid())
        {
            Vector3 dir = target.forward;
            dir.y = 0;
            //transform.position = target.position + dir.normalized * offset;
            transform.position = Vector3.Lerp(transform.position, target.position + dir.normalized * offset, smoothParam * Time.deltaTime);
        }        
    }

    void FindTargetIfInvalid()
    {
        if (!IsTargetValid())
        {
            GameObject obj = GameController.Instance.Player;
            if (obj != null)
            {
                target = obj.transform;
            }
        }
    }

    bool IsTargetValid()
    {
        return target != null && target.gameObject.activeInHierarchy;
    }
}
