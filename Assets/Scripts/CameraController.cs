using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float offset;
    public float smoothParam;
    public Bounds mapBounds;

    public float topOffset;
    public float bottomOffset;
    public float horizontalOffset;

    // Start is called before the first frame update
    void Start()
    {
        FindTargetIfInvalid();

        if (IsTargetValid())
        {
            Vector3 dir = target.forward;
            dir.y = 0;
            transform.position = TargetPosition() + dir.normalized * offset;
        }

        if (GameController.Instance != null)
            mapBounds = GameController.Instance.MapBounds;
    }

    private Vector3 TargetPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(target.position, -Vector3.up, out hit, 200.0f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            return Vector3.Lerp(target.position, hit.point, 0.5f);
        }

        return target.position;
    }

    private Vector3 ClampPosition(Vector3 pos)
    {
        Vector3 res = pos;
        res.x = Mathf.Clamp(res.x, mapBounds.min.x + horizontalOffset, mapBounds.max.x - horizontalOffset);
        res.z = Mathf.Clamp(res.z, mapBounds.min.z + bottomOffset, mapBounds.max.z - topOffset);
        return res;
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
            transform.position = Vector3.Lerp(transform.position, TargetPosition() + dir.normalized * offset, smoothParam * Time.deltaTime);
            transform.position = ClampPosition(transform.position);
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
