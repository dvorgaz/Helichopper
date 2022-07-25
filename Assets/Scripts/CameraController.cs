using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float offset;
    public float smoothParam;
    private Bounds mapBounds;

    public float topOffset;
    public float bottomOffset;
    public float horizontalOffset;
    public Camera attachedCamera;
    public bool useHack;

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
        if (Physics.Raycast(target.position, -Vector3.up, out RaycastHit hit, 200.0f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            Vector3 pos = Vector3.Lerp(target.position, hit.point, 0.5f);

            if(useHack)
            {
                Vector3 dir = target.forward;
                dir.y = 0;
                Vector3 offsetPos = target.position + dir.normalized * offset;
                Vector3 camToOffsetPos = offsetPos - attachedCamera.transform.position;

                Vector3 camToPos = transform.position - attachedCamera.transform.position;
                Vector3 upperLimitDir = Quaternion.AngleAxis(attachedCamera.fieldOfView / 2.0f, -attachedCamera.transform.right) * camToPos;
                upperLimitDir /= Vector3.ProjectOnPlane(upperLimitDir, Vector3.up).magnitude;

                float d = Vector3.ProjectOnPlane(camToOffsetPos, Vector3.up).magnitude;
                float maxH = -camToOffsetPos.y - (d * -upperLimitDir.y) - 15.0f;

                if ((target.position.y - pos.y) > maxH)
                {
                    pos.y = target.position.y - maxH;
                }
            }

            return pos;
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
