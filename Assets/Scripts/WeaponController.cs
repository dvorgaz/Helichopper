using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public List<WeaponLauncher> weapons;
    private int currWeaponIdx = 0;

    public List<Transform> flareLaunchers;
    public GameObject flarePrefab;

    private Rigidbody rb;

    public float targetingRange;
    public float targetingAngle;

    private Vector3 targetingPoint = Vector3.zero;

    public RectTransform crosshairTransform;
    public Camera mainCamera;

    private Health health;

    public int flareCount;
    public float flareInterval;

    public WeaponLauncher Weapon
    {
        get { return currWeaponIdx < weapons.Count ? weapons[currWeaponIdx] : null; }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 adjustedTargetingPoint = targetingPoint;

        if(targetingPoint != Vector3.zero)
        {
            crosshairTransform.gameObject.SetActive(true);

            Vector3 dir = targetingPoint - transform.position;

            Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
            Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

            float angle = Vector3.SignedAngle(flatDir, flatForward, Vector3.up);

            dir = Quaternion.AngleAxis(angle, Vector3.up) * dir;
            adjustedTargetingPoint = dir + transform.position;

            crosshairTransform.position = mainCamera.WorldToScreenPoint(adjustedTargetingPoint);
        }
        else
        {
            crosshairTransform.gameObject.SetActive(false);
        }

        if (health.Alive)
        {
            if (Input.GetMouseButton(0))
            {
                if (targetingPoint != Vector3.zero)
                {
                    Weapon?.Launch(adjustedTargetingPoint);
                }
                else
                {
                    Weapon?.Launch(rb);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Weapon?.Reload();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine(LaunchFlareCoroutine());
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currWeaponIdx = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currWeaponIdx = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                currWeaponIdx = 2;
            }
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

    void LaunchFlares()
    {
        foreach(Transform launcher in flareLaunchers)
        {
            Instantiate(flarePrefab, launcher.position, launcher.rotation).GetComponent<Rigidbody>().velocity = rb.velocity;
        }
    }

    IEnumerator LaunchFlareCoroutine()
    {
        for(int i = 0; i < flareCount; ++i)
        {
            LaunchFlares();
            yield return new WaitForSeconds(flareInterval);
        }
    }
}
