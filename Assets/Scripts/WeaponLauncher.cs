using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLauncher : MonoBehaviour
{
    public GameObject weaponPrefab;
    private GameObject weaponModel;

    private bool canFire = true;

    public bool CanFire
    {
        get { return canFire; }
    }

    private void Awake()
    {
        weaponModel = transform.Find("WeaponModel").gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reload()
    {
        weaponModel.SetActive(true);
        canFire = true;
    }

    public void Launch(Rigidbody launchPlatform = null)
    {
        if (CanFire)
        {
            weaponModel.SetActive(false);
            //canFire = false;

            GameObject obj = Instantiate(weaponPrefab, transform.position, transform.rotation);
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null && launchPlatform != null)
            {
                //rb.velocity = launchPlatform.velocity;
            }
        }
    }

    public void Launch(Vector3 targetPoint)
    {
        if (CanFire)
        {
            weaponModel.SetActive(false);
            //canFire = false;

            Vector3 dir = (targetPoint - transform.position).normalized;

            Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
            Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

            float angle = Vector3.SignedAngle(flatDir, flatForward, Vector3.up);

            dir = Quaternion.AngleAxis(angle, Vector3.up) * dir;

            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

            GameObject obj = Instantiate(weaponPrefab, transform.position, rot);
            Rigidbody rb = obj.GetComponent<Rigidbody>();
        }
    }
}
