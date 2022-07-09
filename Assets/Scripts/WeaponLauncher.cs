using UnityEngine;

public class WeaponLauncher : MonoBehaviour
{
    public enum FiringMode
    {
        Single,
        Burst,
        Auto
    }

    [SerializeField] private GameObject weaponPrefab;    
    [SerializeField] private FiringMode firingMode;
    [SerializeField] private float spread;
    [SerializeField] private float rateOfFire; // rounds per minute
    [SerializeField] private int burstLength;
    [SerializeField] private int shotsMax = 0;
    private int shotsLeft;

    private GameObject weaponModel;
    private ParticleSystem[] particleSystems;
    private AudioSource audioSrc;

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
        bool hasShots = shotsMax == 0 || shotsLeft > 0;
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
        Transform tr = transform.Find("WeaponModel");
        if(tr != null)
            weaponModel = tr.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
        audioSrc = GetComponent<AudioSource>();
        Reload();
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
        if(weaponModel != null)
            weaponModel.SetActive(true);

        shotsLeft = shotsMax;
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

    public void Fire(Vector3 targetPoint, Rigidbody launchPlatform = null)
    {
        isFiring = true;

        if (CanFire())
        {           
            FireProjectile(targetPoint, launchPlatform);
        }
    }

    public void Fire(Rigidbody launchPlatform = null)
    {
        Fire(transform.position + transform.forward, launchPlatform);
    }

    private void FireProjectile(Vector3 targetPoint, Rigidbody launchPlatform)
    {
        lastShotTime = Time.time;

        if (shotsMax != 0)
            shotsLeft--;

        if (burstCounter > 0)
            burstCounter--;

        if (weaponModel != null)
            weaponModel.SetActive(false);

        Vector3 dir = (targetPoint - transform.position).normalized;
        dir += Vector3.ProjectOnPlane(Random.insideUnitSphere * spread, dir);
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        GameObject obj = Instantiate(weaponPrefab, transform.position, rot);

        if (launchPlatform != null)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = launchPlatform.velocity;
            }
        }

        if (audioSrc)
            audioSrc.Play();

        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Emit(1);
        }
    }
}
