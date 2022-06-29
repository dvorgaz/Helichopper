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
                rb.velocity = launchPlatform.velocity;
            }
        }
    }
}
