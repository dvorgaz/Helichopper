using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingCamera : MonoBehaviour
{
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private float targetSize = 10.0f;
    [SerializeField] private float smooth = 5.0f;
    [SerializeField] private Camera cam;
    private float lastValidTargetTime = 0.0f;
    private Vector3 tgt = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void LateUpdate()
    {        
        if (weaponController.Target != null)
        {
            tgt = weaponController.Target.position;
            lastValidTargetTime = Time.time;
        }
        else if(Time.time - lastValidTargetTime > 2.0f)
        {
            tgt = weaponController.TargetPoint;
        }
        
        Vector3 toTgt = tgt - transform.position;
        float dist = toTgt.magnitude;
        Vector3 point = tgt + Vector3.Cross(Vector3.up, toTgt.normalized) * targetSize;
        Vector3 toPoint = point - transform.position;
        cam.fieldOfView = Vector3.Angle(toTgt, toPoint) * 2;
        Quaternion targetRot = Quaternion.LookRotation((tgt - transform.position).normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * smooth);
    }
}
