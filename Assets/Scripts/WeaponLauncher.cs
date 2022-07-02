using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLauncher : MonoBehaviour
{
    public enum FiringMode
    {
        Single,
        Burst,
        Auto
    }

    public GameObject weaponPrefab;
    private GameObject weaponModel;

    public FiringMode firingMode;
    public float spread;
    public float rateOfFire; // rounds per minute
    public int burstCount;

    private ParticleSystem[] particleSystems;

    public float ShotInterval
    {
        get { return 1 / (rateOfFire / 60); }
    }

    private float lastShotTime;
    private int burstCounter;
    private bool isFiring = false;
    private Vector3 targetPoint;
    private bool canFire = true;

    public bool CanFire
    {
        get
        {
            bool isOnCooldown = (Time.time - lastShotTime) < ShotInterval;
            return canFire && !isOnCooldown; 
        }
    }

    private void Awake()
    {
        Transform tr = transform.Find("WeaponModel");
        if(tr != null)
            weaponModel = tr.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isFiring && CanFire)
        {
            Launch(targetPoint);
        }
    }

    public void SetFiring(bool firing, Vector3 targetPoint)
    {
        isFiring = firing;
        this.targetPoint = targetPoint;
    }

    public void Reload()
    {
        weaponModel?.SetActive(true);
        canFire = true;
    }

    public void Launch(Rigidbody launchPlatform = null)
    {
        if (CanFire)
        {
            lastShotTime = Time.time;
            weaponModel?.SetActive(false);
            //canFire = false;

            GameObject obj = Instantiate(weaponPrefab, transform.position, transform.rotation);
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null && launchPlatform != null)
            {
                //rb.velocity = launchPlatform.velocity;
            }

            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Emit(1);
            }
        }
    }

    public void Launch(Vector3 targetPoint)
    {
        if (CanFire)
        {
            lastShotTime = Time.time;
            weaponModel?.SetActive(false);
            //canFire = false;

            Vector3 dir = (targetPoint - transform.position).normalized;

            //Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
            //Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

            //float angle = Vector3.SignedAngle(flatDir, flatForward, Vector3.up);

            //dir = Quaternion.AngleAxis(angle, Vector3.up) * dir;

            dir += Vector3.ProjectOnPlane(Random.insideUnitSphere * spread, dir);

            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

            GameObject obj = Instantiate(weaponPrefab, transform.position, rot);

            foreach(ParticleSystem ps in particleSystems)
            {
                ps.Emit(1);
            }
        }
    }
}
