using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private HeliController heli;
    private WeaponController wpn;

    [SerializeField] private float mouseRate;
    private bool controllingCyclic = true;

    private void Awake()
    {
        heli = GetComponent<HeliController>();
        wpn = GetComponent<WeaponController>();
    }

    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        bool isLanding = false;

        if(heli != null)
        {
            isLanding = heli.Landing;

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(2))
            {
                SetCyclicControl(!controllingCyclic);
            }
#endif

            if (controllingCyclic)
            {
                Vector3 mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
                heli.SetRotorTiltDelta(mouseDelta * mouseRate);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                heli.SetRotorTilt(Vector3.zero);
            }

            heli.Vertical = 0.0f;
            if (Input.GetKey(KeyCode.W))
            {
                heli.Vertical += 1.0f;
            }

            if (Input.GetKey(KeyCode.S))
            {
                heli.Vertical -= 1.0f;
            }

            heli.Horizontal = 0.0f;
            if (Input.GetKey(KeyCode.A))
            {
                heli.Horizontal += 1.0f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                heli.Horizontal -= 1.0f;
            }

            heli.StabilizeAim = Input.GetMouseButton(0) || Input.GetMouseButton(1);
        }

        if (wpn != null)
        {
            if (Input.GetMouseButton(0) && !isLanding)
            {
                wpn.FireSelectedWeapon();
            }

            wpn.Aiming = Input.GetMouseButton(1);

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.R))
            {
                wpn.Rearm();
            }
#endif

            if (Input.GetKeyDown(KeyCode.F))
            {
                wpn.LaunchFlares();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                wpn.SelectWeapon(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                wpn.SelectWeapon(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                wpn.SelectWeapon(2);
            }

            if(Input.mouseScrollDelta.y != 0)
            {
                wpn.CycleWeapon(Mathf.RoundToInt(-Input.mouseScrollDelta.y));
            }
        }
    }

    void SetCyclicControl(bool enable)
    {
        controllingCyclic = enable;
        Cursor.visible = !controllingCyclic;
        Cursor.lockState = controllingCyclic ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnEnable()
    {
        //SetCyclicControl(true);
    }

    private void OnDisable()
    {
        //SetCyclicControl(false);
    }

    public void OnDeath()
    {
        enabled = false;
    }
}
