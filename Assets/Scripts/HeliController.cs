using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliController : MonoBehaviour
{
    Rigidbody rb;

    public float mouseRate;

    public float thrustChangeRate;
    public float maxThrust;
    public float maxVerticalThrust;
    public float turnRate;
    public float mouseTurnRate;

    [SerializeField] float speed;

    [SerializeField] Vector3 rotorTilt = Vector3.zero;
    public float maxRotorTilt;
    public float tiltSmooth;
    public float tiltTorque;

    public float maxSpeedKPH;
    public AnimationCurve dragCurve;
    public float drag;
    public Transform cameraTransform;
    public Transform tail;
    public float tailDrag;

    bool controllingCyclic = false;
    bool thrustEnabled = true;

    public float dampenFactor = 0.8f; // this value requires tuning
    public float adjustFactor = 0.5f; // this value requires tuning

    private bool controlEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
    }

    void ToggleCyclicContorl()
    {
        controllingCyclic = !controllingCyclic;
        Cursor.visible = !controllingCyclic;
        Cursor.lockState = controllingCyclic ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void OnDeath()
    {
        ShutDown();
    }

    public void ShutDown()
    {
        controlEnabled = false;
        thrustEnabled = false;
        rb.drag = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            ToggleCyclicContorl();
        }
        if (Input.GetMouseButtonUp(2))
        {
            thrustEnabled = !thrustEnabled && controlEnabled;
        }

        Vector3 mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

        if (controllingCyclic)
        {            
            rotorTilt += mouseDelta * mouseRate;

            if(rotorTilt.magnitude > maxRotorTilt)
            {
                rotorTilt = rotorTilt.normalized * maxRotorTilt;
            }            
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            rotorTilt = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        Vector3 worldForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
        worldForward.y = 0;
        worldForward.Normalize();

        Vector3 worldRight = Vector3.Cross(Vector3.up, worldForward);
        Vector3 worldTiltDir = worldRight * rotorTilt.x + worldForward * rotorTilt.y;
        Vector3 tiltAxis = Vector3.Cross(Vector3.up, worldTiltDir).normalized;
        Vector3 tiltVector = Quaternion.AngleAxis(rotorTilt.magnitude, tiltAxis) * Vector3.up;

        Vector3 newRight = Vector3.Cross(tiltVector, transform.forward);
        Vector3 newForward = Vector3.Cross(newRight, tiltVector);
        Quaternion targetRotation = Quaternion.LookRotation(newForward, tiltVector);

        Quaternion deltaQuat = Quaternion.FromToRotation(transform.up, tiltVector);
        Vector3 axis;
        float angle;
        deltaQuat.ToAngleAxis(out angle, out axis);

        if (thrustEnabled)
        {
            //rb.AddTorque(-rb.angularVelocity * dampenFactor, ForceMode.Acceleration);
            //rb.AddTorque(axis.normalized * angle * adjustFactor, ForceMode.Acceleration);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, tiltSmooth * Time.fixedDeltaTime);
        }

        float vertical = 0.0f;

        if (controlEnabled)
        {
            if (Input.GetKey(KeyCode.W))
            {
                vertical += 1;
            }

            if (Input.GetKey(KeyCode.S))
            {
                vertical -= 1;
            }
        }

        //rb.AddForce(Vector3.up * vertical * maxThrust, ForceMode.Force);
        if (thrustEnabled)
            rb.AddForce(rb.mass * -Physics.gravity + (Vector3.up * vertical * maxVerticalThrust), ForceMode.Force);

        Vector3 dir = transform.up - Vector3.up;
        dir.y = 0;

        if (thrustEnabled)
            rb.AddForce(dir * maxThrust, ForceMode.Force);

        float horizontal = 0.0f;

        if (controlEnabled)
        {
            if (Input.GetKey(KeyCode.A))
            {
                horizontal += 1;
            }

            if (Input.GetKey(KeyCode.D))
            {
                horizontal -= 1;
            }
        }

        rb.AddForceAtPosition(tail.right * turnRate * horizontal, tail.position, ForceMode.Force);

        //float p = 1.225f;
        //float cd = 0.45f;
        //float a = 4 * Mathf.Abs(Vector3.Dot(tail.right, -rb.velocity.normalized)) * 8;
        //float v = rb.velocity.magnitude;

        Vector3 direction = -rb.velocity.normalized;
        //float forceAmount = (p * v * v * cd * a) / 2;
        //rb.AddForceAtPosition(direction * forceAmount, tail.position, ForceMode.Force);
        //rb.AddForce(direction * (rb.velocity.sqrMagnitude * drag), ForceMode.Force);
        float maxVelocity = (maxSpeedKPH * 1000) / (60 * 60);
        float dragForce = (maxThrust * Mathf.Sin(maxRotorTilt)) * dragCurve.Evaluate(rb.velocity.magnitude / maxVelocity);
        //rb.AddForce(direction * dragForce, ForceMode.Force);

        Vector3 flow = -rb.velocity.normalized;
        float tailDragForce = Vector3.Dot(tail.right, flow) * tailDrag * rb.velocity.sqrMagnitude;
        rb.AddForceAtPosition(tail.right * tailDragForce, tail.position, ForceMode.Force);

        //Vector3 t = Vector3.Cross(tail.position - rb.worldCenterOfMass, tail.right * tailDragForce);
        //rb.AddTorque(t, ForceMode.Force);

        speed = rb.velocity.magnitude * 60 * 60 / 1000;
    }
}
