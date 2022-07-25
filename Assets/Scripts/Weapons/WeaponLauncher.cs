using UnityEngine;

public class WeaponLauncher : MonoBehaviour
{
    public enum FiringMode
    {
        Single,
        Burst,
        Auto
    }

    public string displayName;
    [SerializeField] private GameObject weaponPrefab;    
    [SerializeField] private FiringMode firingMode;
    [SerializeField] private float spread;
    [SerializeField] private float rateOfFire; // rounds per minute
    [SerializeField] private int burstLength;
    [SerializeField] private int shotsMax = 0;
    [SerializeField] private float range;
    [SerializeField] private float soundPitchRange = 0.1f;
    [SerializeField] private ItemType ammoItem;
    [SerializeField] private bool inheritVelocity;
    public bool showOnCamera;
    private float origPitch;
    private int shotsLeft;

    [SerializeField] private FiringPoint[] firingPoints;
    int firingPointIdx;

    private Transform _transform
    {
        get
        {
            if (firingPoints == null || firingPoints.Length == 0)
                return transform;

            return firingPoints[firingPointIdx].transform;
        }
    }

    public ItemType AmmoItem { get { return ammoItem; } }
    public int ShotsLeft
    {
        get
        {
            return shotsLeft;
        }

        set
        {
            shotsLeft = value;

            if (shotsMax != 0)
            {
                for (int i = 0; i < firingPoints.Length; ++i)
                {
                    firingPoints[i].Reload(i >= firingPoints.Length - shotsLeft);
                }

                firingPointIdx = (shotsMax - shotsLeft) % firingPoints.Length;
            }
        }
    }
    public float Range { get { return range; } }

    private AudioSource audioSrc;
    [SerializeField] private bool useDualAudio;
    private AudioSource[] audioBank;
    private int audioIdx;

    public float ShotInterval
    {
        get { return 1 / (rateOfFire / 60); }
    }

    private float lastShotTime;
    private int burstCounter;
    private bool isFiring = false;
    private bool wasFiring = false;

    public bool CanFire()
    {
        bool hasShots = shotsMax == 0 || ShotsLeft > 0;
        bool isOnCooldown = (Time.time - lastShotTime) < ShotInterval;
        bool burstEnded = false;
        if (firingMode == FiringMode.Single || firingMode == FiringMode.Burst)
        {
            burstEnded = burstCounter <= 0;
        }

        return hasShots && !isOnCooldown && !burstEnded; 
    }

    private void Awake()
    {
        if(firingPoints == null || firingPoints.Length == 0)
        {
            FiringPoint fp = gameObject.AddComponent<FiringPoint>();
            firingPoints = new FiringPoint[1];
            firingPoints[0] = fp;
        }

        Reload();
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();    
        if(audioSrc != null)
        {
            origPitch = audioSrc.pitch;

            if (useDualAudio)
            {
                AudioSource copy = gameObject.AddComponent<AudioSource>();
                copy.clip = audioSrc.clip;
                copy.spatialBlend = audioSrc.spatialBlend;
                copy.minDistance = audioSrc.minDistance;
                copy.pitch = origPitch;
                copy.playOnAwake = audioSrc.playOnAwake;
                copy.volume = audioSrc.volume;
                copy.priority = audioSrc.priority;

                audioBank = new AudioSource[2];
                audioBank[0] = audioSrc;
                audioBank[1] = copy;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        if(wasFiring && !isFiring)
        {
            // Trigger released
            ResetBurstCounter();
        }

        wasFiring = isFiring;
        isFiring = false;
    }

    public void Reload()
    {
        foreach (FiringPoint fp in firingPoints)
            fp.Reload();

        ShotsLeft = shotsMax;
        ResetBurstCounter();
    }

    private void ResetBurstCounter()
    {
        switch (firingMode)
        {
            case FiringMode.Single:
                burstCounter = 1;
                break;
            case FiringMode.Burst:
                burstCounter = burstLength;
                break;
        }
    }

    public bool Fire(Vector3 targetPoint, Transform lockedTarget = null, Rigidbody launchPlatform = null)
    {
        isFiring = true;

        if (CanFire())
        {           
            FireProjectile(targetPoint, lockedTarget, launchPlatform);
            return true;
        }

        return false;
    }

    public void Fire(Rigidbody launchPlatform = null)
    {
        Fire(_transform.position + transform.forward, null, launchPlatform);
    }

    private void FireProjectile(Vector3 targetPoint, Transform lockedTarget, Rigidbody launchPlatform)
    {
        lastShotTime = Time.time;        

        Vector3 dir = (targetPoint - _transform.position).normalized;

        MoveProjectile mp = weaponPrefab.GetComponent<MoveProjectile>();
        if (mp.gravityMultiplier > 1.0f)
        {
            float vel = mp.impulse / weaponPrefab.GetComponent<Rigidbody>().mass;
            dir = GetLaunchAngle(vel, _transform.position, targetPoint, Physics.gravity.magnitude * mp.gravityMultiplier);
        }

        dir += Vector3.ProjectOnPlane(Random.insideUnitSphere * spread, dir);
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        GameObject obj = Instantiate(weaponPrefab, _transform.position, rot);

        //if (launchPlatform != null && inheritVelocity)
        //{
        //    obj.GetComponent<MoveProjectile>().inheritedVelocity = Vector3.ProjectOnPlane(launchPlatform.velocity, obj.transform.forward);
        //}

        if(lockedTarget != null)
        {
            TargetHoming homing = obj.GetComponent<TargetHoming>();
            if (homing != null)
            {
                homing.target = lockedTarget;
                obj.transform.rotation = _transform.rotation;
            }
        }

        if (audioSrc)
        {
            if (useDualAudio)
            {                
                audioBank[audioIdx].pitch = origPitch + Random.Range(-soundPitchRange, soundPitchRange);
                audioBank[audioIdx].Play();
                audioIdx = (audioIdx + 1) % 2;
            }
            else
            {
                audioSrc.pitch = origPitch + Random.Range(-soundPitchRange, soundPitchRange);
                audioSrc.Play();
            }
        }

        firingPoints[firingPointIdx].Fire();

        if (firingPoints != null && firingPoints.Length > 0)
        {
            firingPointIdx = (firingPointIdx + 1) % firingPoints.Length;
        }

        if (shotsMax != 0)
            ShotsLeft--;

        if (burstCounter > 0)
            burstCounter--;
    }
    private Vector3 GetLaunchAngle(float speed, Vector3 launchPos, Vector3 targetPos, float gravity)
    {
        Vector3 toTarget = targetPos - launchPos;
        Vector3 launchAngle = _transform.forward;
        Vector3 gravVector = -Vector3.up * gravity;
        float gSquared = gravity * gravity;
        float b = speed * speed + Vector3.Dot(toTarget, gravVector);
        float discriminant = b * b - gSquared * toTarget.sqrMagnitude;
        if (discriminant >= 0)
        {
            float discRoot = Mathf.Sqrt(discriminant);
            //float tMax = Mathf.Sqrt((b + discRoot) * 2 / gSquared);
            float tMin = Mathf.Sqrt((b - discRoot) * 2 / gSquared);
            float time = tMin;
            launchAngle = (toTarget / time - gravVector * time / 2).normalized;
        }
        return launchAngle;
    }
}
