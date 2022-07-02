using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponLauncher weapon;

    private Rigidbody rb;

    public float targetingRange;
    public float targetingAngle;

    private Vector3 targetingPoint = Vector3.zero;

    public RectTransform crosshairTransform;
    public Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(targetingPoint != Vector3.zero)
        {
            crosshairTransform.gameObject.SetActive(true);

            Vector3 dir = targetingPoint - transform.position;

            Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
            Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

            float angle = Vector3.SignedAngle(flatDir, flatForward, Vector3.up);

            dir = Quaternion.AngleAxis(angle, Vector3.up) * dir;

            crosshairTransform.position = mainCamera.WorldToScreenPoint(dir + transform.position);
        }
        else
        {
            crosshairTransform.gameObject.SetActive(false);
        }

        if (Input.GetMouseButton(0))
        {
            if (targetingPoint != Vector3.zero)
            {
                weapon.Launch(targetingPoint);
            }
            else
            {
                weapon.Launch(rb);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            weapon.Reload();
        }
    }

    private void FixedUpdate()
    {
        Vector3 dir = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 dirSideR = Quaternion.AngleAxis(targetingAngle / 2.0f, Vector3.up) * dir;
        Vector3 dirSideL = Quaternion.AngleAxis(targetingAngle / 2.0f, -Vector3.up) * dir;

        Vector3 planeR = Vector3.Cross(dirSideR, Vector3.up);
        Vector3 planeL = Vector3.Cross(Vector3.up, dirSideL);

        float closestDist = float.MaxValue;
        Health closestTarget = null;

        foreach (Health target in FindObjectsOfType<Health>())
        {
            if (target.gameObject == gameObject)
                continue;

            if (!target.Alive)
                continue;

            Vector3 pos = target.transform.position - transform.position;

            if (Vector3.Dot(planeR, pos) < 0)
                continue;

            if (Vector3.Dot(planeL, pos) < 0)
                continue;

            float dist = Vector3.Magnitude(target.transform.position - transform.position);

            if (dist < closestDist)
            {
                closestDist = dist;
                closestTarget = target;
            }
        }

        if(closestTarget != null)
        {
            targetingPoint = closestTarget.transform.position;
        }
        else
        {
            Vector3 idleDir = Quaternion.AngleAxis(15.0f, Vector3.Cross(Vector3.up, dir)) * dir;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, idleDir, out hit, 200.0f, LayerMask.GetMask("Default")))
            {
                targetingPoint = hit.point;
            }
            else
            {
                targetingPoint = Vector3.zero;
            }
        }
    }
}
