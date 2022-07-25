using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringPoint : MonoBehaviour
{
    private ParticleSystem[] particleSystems;
    private GameObject weaponModel;

    private void Awake()
    {
        particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
        Transform tr = transform.Find("WeaponModel");
        if (tr != null)
            weaponModel = tr.gameObject;
    }

    public void Reload(bool forReal = true)
    {
        if (weaponModel != null)
            weaponModel.SetActive(forReal);
    }

    public void Fire()
    {
        if (weaponModel != null)
            weaponModel.SetActive(false);

        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Emit(1);
        }
    }
}
