using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float targetingRange;
    [SerializeField] private float minRange;
    [SerializeField] private float targetingAngle;
    [SerializeField] private bool useAngledMinDistance;
    [SerializeField] private float minRangeAngle;
    private List<WeaponLauncher> weapons;
    private int currWeaponIdx = 0;
    private Vector3 targetingPoint = Vector3.zero;
    private Vector3 adjustedTargetingPoint = Vector3.zero;

    public float MinRange { get { return minRange; } }
    public float MinRangeAngle { get { return minRangeAngle; } }
    public bool UseAngledMinDistance { get { return useAngledMinDistance; } }
    public Vector3 TargetPoint { get { return adjustedTargetingPoint; } }
    public bool ShowCrosshair { get; private set; } = false;

    public bool Aiming { get; set; } = false;

    private List<Transform> flareLaunchers;
    [SerializeField] private GameObject flarePrefab;
    [SerializeField] private int flareBurstCount;
    [SerializeField] private float flareBurstInterval;
    [SerializeField] private int maxFlareAmount;    
    public int Flares { get; private set; }

    private Health closestTarget;
    private Health lastLaunchTarget;

    public Transform Target
    {
        get { return closestTarget != null ? closestTarget.transform : null; }
    }

    public Transform LaunchTarget
    {
        get { return lastLaunchTarget != null ? lastLaunchTarget.transform : null; }
    }

    public bool CameraImageValid { get; set; }

    public float Range { get { return Weapon != null ? Weapon.Range : targetingRange; } }

    public WeaponLauncher Weapon
    {
        get { return currWeaponIdx < weapons.Count ? weapons[currWeaponIdx] : null; }
    }

    public WeaponLauncher GetWeapon(int idx)
    {
        return idx < weapons.Count ? weapons[idx] : null;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        weapons = new List<WeaponLauncher>();
        foreach (WeaponLauncher weapon in GetComponentsInChildren<WeaponLauncher>())
        {
            weapons.Add(weapon);
        }

        flareLaunchers = new List<Transform>();
        Transform fl = transform.Find("FlareLaunchers");
        if (fl != null)
        {
            foreach (Transform tr in fl)
            {
                if (tr.gameObject.activeSelf)
                    flareLaunchers.Add(tr);
            }
        }
        else
        {
            Debug.LogError("Game object: " + gameObject.name + " missing FlareLaunchers node");
        }

        Flares = maxFlareAmount;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        adjustedTargetingPoint = targetingPoint;

        if(targetingPoint != Vector3.zero)
        {
            Vector3 dir = targetingPoint - transform.position;

            Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
            Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

            float angle = Vector3.SignedAngle(flatDir, flatForward, Vector3.up);

            dir = Quaternion.AngleAxis(angle, Vector3.up) * dir;
            adjustedTargetingPoint = dir + transform.position;
        }
    }

    private void FixedUpdate()
    {
        Vector3 dir = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, dir);
        Vector3 dirSideR = Quaternion.AngleAxis(targetingAngle / 2.0f, Vector3.up) * dir;
        Vector3 dirSideL = Quaternion.AngleAxis(targetingAngle / 2.0f, -Vector3.up) * dir;
        Vector3 dirSideBottom = Quaternion.AngleAxis(MinRangeAngle, right) * dir;

        Vector3 planeR = Vector3.Cross(dirSideR, Vector3.up);
        Vector3 planeL = Vector3.Cross(Vector3.up, dirSideL);
        Vector3 planeBottom = Vector3.Cross(dirSideBottom, right);

        float threshold = 3.0f;
        float closestDist = float.MaxValue;
        bool closesWithinThreshold = false;
        closestTarget = null;

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

            float dist = Vector3.ProjectOnPlane(target.transform.position - transform.position, Vector3.up).magnitude;

            if (dist > Range)
                continue;

            if (useAngledMinDistance)
            {
                if (Vector3.Dot(planeBottom, pos) < 0)
                    continue;
            }
            else
            {
                if (dist < minRange)
                    continue;
            }

            bool foundBetterTarget = false;
            bool withinThreshold = Mathf.Abs(Vector3.Dot(right, pos)) < threshold;

            if(withinThreshold == closesWithinThreshold)
            {
                foundBetterTarget = dist < closestDist;
            }
            else
            {
                foundBetterTarget = withinThreshold && !closesWithinThreshold;
            }

            if (foundBetterTarget)
            {
                closesWithinThreshold = withinThreshold;
                closestDist = dist;
                closestTarget = target;
            }
        }

        if(closestTarget != null)
        {
            targetingPoint = closestTarget.transform.position;
            ShowCrosshair = true;
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

            ShowCrosshair = false;
        }
    }

    public void FireSelectedWeapon()
    {
        if (targetingPoint != Vector3.zero)
        {
            if (Weapon)
            {
                bool launched = Weapon.Fire(adjustedTargetingPoint, closestTarget != null ? closestTarget.transform : null);
                if(launched && Weapon.showOnCamera && closestTarget != null)
                {
                    lastLaunchTarget = closestTarget;
                    CameraImageValid = true;
                }
            }
        }
        else
        {
            if (Weapon)
                Weapon.Fire();
        }
    }

    public void OnLaunchHit(GameObject obj)
    {
        lastLaunchTarget = null;
    }

    public void Rearm()
    {
        foreach(WeaponLauncher weapon in weapons)
        {
            weapon.Reload();
        }

        Flares = maxFlareAmount;
    }

    public void SelectWeapon(int idx)
    {
        currWeaponIdx = Mathf.Clamp(idx, 0, weapons.Count - 1);
    }

    public void CycleWeapon(int dir)
    {
        dir = Mathf.RoundToInt(Mathf.Sign(dir));
        currWeaponIdx = Mathf.Clamp(currWeaponIdx + dir, 0, weapons.Count - 1);
    }

    public void LaunchFlares()
    {
        StartCoroutine(LaunchFlareCoroutine());
    }

    void LaunchFlaresSingle()
    {
        foreach(Transform launcher in flareLaunchers)
        {
            if (Flares > 0)
            {
                Instantiate(flarePrefab, launcher.position, launcher.rotation).GetComponent<Rigidbody>().velocity = rb.velocity;
                Flares--;
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
            LaunchFlaresSingle();
            yield return new WaitForSeconds(flareBurstInterval);
        }
    }

    public void OnItemPickedUp(Pickup pickup)
    {
        if(pickup.content.type.name == "AmmoRefill")
            Rearm();
    }

    public Dictionary<ItemType, int> GetRemainingAmmo()
    {
        Dictionary<ItemType, int> result = new Dictionary<ItemType, int>();
        foreach (WeaponLauncher weapon in weapons)
        {
            result.Add(weapon.AmmoItem, weapon.ShotsLeft);
        }

        return result;
    }

    public void SetRemainingAmmo(Dictionary<ItemType, int> remainingAmmo)
    {
        foreach (WeaponLauncher weapon in weapons)
        {
            if (remainingAmmo.ContainsKey(weapon.AmmoItem))
            {
                weapon.ShotsLeft = remainingAmmo[weapon.AmmoItem];
            }
            else
            {
                weapon.ShotsLeft = 0;
            }
        }
    }
}
