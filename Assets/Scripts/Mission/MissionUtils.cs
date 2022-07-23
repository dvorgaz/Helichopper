using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MissionUtils : MonoBehaviour
{
    public GameObject targetObject;
    public UnityEvent<GameObject> onFinished;
    public bool useAsTrigger;
    public bool destroyOnTriggerEnter;

    public void TeleportSubject(Transform newPos)
    {
        targetObject.transform.position = newPos.position;
        targetObject.SetActive(true);
        onFinished.Invoke(targetObject);
    }

    public void SendSubjectTo(Transform newPos)
    {
        var move = targetObject.GetComponent<GroundMovement>();
        move.Destination = newPos.position;
        onFinished.Invoke(targetObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(useAsTrigger && other.gameObject == targetObject)
        {
            onFinished.Invoke(targetObject);
            if (destroyOnTriggerEnter)
                Destroy(targetObject);
        }
    }
}
