using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
public class WeaponController : MonoBehaviour
{
    private Rigidbody rb;
    private Health health;

    [SerializeField] private float targetingRange;
    [SerializeField] private float targetingAngle;
    private List<WeaponLauncher> weapons;
    private int currWeaponIdx = 0;
    private Vector3 targetingPoint = Vector3.zero;

    private List<Transform> flareLaunchers;
    [SerializeField] private GameObject flarePrefab;
    [SerializeField] private int flareBurstCount;
    [SerializeField] private float flareBurstInterval;
    [SerializeField] private int maxFlareAmount;
    private int flareAmount;

    private Health closestTarget;
    private RectTransform crosshairTransform;
    private Camera mainCamera;    

    public WeaponLauncher Weapon
    {
        get { return currWeaponIdx < weapons.Count ? weapons[currWeaponIdx] : null; }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        mainCamera = Camera.main;
        crosshairTransform = GameController.Instance.GameUI.Crosshair;

        weapons = new List<WeaponLauncher>();
        foreach (WeaponLauncher weapon in GetComponentsInChildren<WeaponLauncher>())
        {
            weapons.Add(weapon);
        }

        flareLaunchers = new List<Transform>();
        Transform fl = transform.Find("FlareLaunchers");
        if(fl != null)
        {
            foreach(Transform tr in fl)
            {
                if(tr.gameObject.activeSelf)
                    flareLaunchers.Add(tr);
            }
        }
        else
        {
            Debug.LogError("Game object: " + gameObject.name + " missing FlareLaunchers node");
        }

        flareAmount = maxFlareAmount;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 adjustedTargetingPoint = targetingPoint;

        if(targetingPoint != Vector3.zero)
        {
            Vector3 dir = targetingPoint - transform.position;

            Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
            Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

            float angle = Vector3.SignedAngle(flatDir, flatForward, Vector3.up);

            dir = Quaternion.AngleAxis(angle, Vector3.up) * dir;
            adjustedTargetingPoint = dir + transform.position;

            crosshairTransform.position = mainCamera.WorldToScreenPoint(adjustedTargetingPoint);
        }

        if (health.Alive)
        {
            if (Input.GetMouseButton(0))
            {
                if (targetingPoint != Vector3.zero)
                {
                    if (Weapon)
                        Weapon.Fire(adjustedTargetingPoint, closestTarget != null ? closestTarget.transform : null);
                }
                else
                {
                    if (Weapon)
                        Weapon.Fire();
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Rearm();
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

    private void LateUpdate()
    {
        GameUI ui = GameController.Instance.GameUI;
        for (int i = 0; i < 3 && i < weapons.Count; ++i)
        {
            ui.SetUIText(GameUI.UIElement.Gun + i, weapons[i].ShotsLeft);
        }

        ui.SetUIText(GameUI.UIElement.Armor, health.Hp);
        ui.SetUIText(GameUI.UIElement.Flares, flareAmount);
    }

    private void FixedUpdate()
    {
        Vector3 dir = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 dirSideR = Quaternion.AngleAxis(targetingAngle / 2.0f, Vector3.up) * dir;
        Vector3 dirSideL = Quaternion.AngleAxis(targetingAngle / 2.0f, -Vector3.up) * dir;

        Vector3 planeR = Vector3.Cross(dirSideR, Vector3.up);
        Vector3 planeL = Vector3.Cross(Vector3.up, dirSideL);

        float closestDist = float.MaxValue;
        closestTarget = null;
        float range = Weapon != null ? Weapon.Range : targetingRange;

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

            if (dist > range)
                continue;

            if (dist < closestDist)
            {
                closestDist = dist;
                closestTarget = target;
            }
        }

        if(closestTarget != null)
        {
            targetingPoint = closestTarget.transform.position;
            crosshairTransform.gameObject.SetActive(true);
        }
        else
        {
            Vector3 idleDir = Quaternion.AngleAxis(25.0f, Vector3.Cross(Vector3.up, dir)) * dir;
            //Vector3 idleDir = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            //float idleDist = targetingRange * 0.8f;
            //Vector3 groundPoint = transform.position + idleDir * idleDist;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, idleDir, out hit, 200.0f, LayerMask.GetMask("Default", "Vehicle"), QueryTriggerInteraction.Ignore))
            //if (Physics.Raycast(groundPoint, -Vector3.up, out hit, 200.0f, LayerMask.GetMask("Default", "Vehicle"), QueryTriggerInteraction.Ignore))
            {
                targetingPoint = hit.point;
            }
            else
            {
                targetingPoint = Vector3.zero;
            }

            crosshairTransform.gameObject.SetActive(false);
        }
    }

    public void Rearm()
    {
        foreach(WeaponLauncher weapon in weapons)
        {
            weapon.Reload();
        }

        flareAmount = maxFlareAmount;
    }

    void LaunchFlares()
    {
        foreach(Transform launcher in flareLaunchers)
        {
            if (flareAmount > 0)
            {
                Instantiate(flarePrefab, launcher.position, launcher.rotation).GetComponent<Rigidbody>().velocity = rb.velocity;
                flareAmount--;
            }
            else
            {
                break;
            }
        }
    }

    IEnumerator LaunchFlareCoroutine()
    {
        for(int i = 0; i < flareBurstCount; ++i)
        {
            LaunchFlares();
            yield return new WaitForSeconds(flareBurstInterval);
        }
    }

    public void OnItemPickedUp(Pickup pickup)
    {
        Rearm();
        Debug.Log("Item processed: " + pickup.gameObject.name);
    }
}
