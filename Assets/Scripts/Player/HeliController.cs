using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliController : MonoBehaviour
{
    private Rigidbody rb;
    
    [SerializeField] private float mouseRate;
    [SerializeField] private float maxThrust;
    [SerializeField] private float maxVerticalThrust;
    [SerializeField] private float turnRate;
    [SerializeField] private float maxFuel;
    [SerializeField] private float fuelBurnRate;
    private float fuel;

    [SerializeField] private float speed; // Speed in km/h

    [SerializeField] private Vector3 rotorTilt = Vector3.zero;
    [SerializeField] private float maxRotorTilt;
    [SerializeField] private float tiltSmooth;
    [SerializeField] private bool useAngularVelocity;
    [SerializeField] private bool useSmoothing;

    private Transform cameraTransform;
    private Transform tailOffset;
    [SerializeField] private float tailDrag;

    private bool controllingCyclic = false;
    private bool thrustEnabled = true;

    private bool controlEnabled = true;

    private void Awake()
    {
        tailOffset = transform.Find("TailOffset");
        if (tailOffset == null)
            Debug.LogError("Object: " + gameObject.name + " missing TailOffset node");
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        cameraTransform = Camera.main.transform;
        fuel = maxFuel;
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

    public void Refuel()
    {
        fuel = maxFuel;
    }

    public void ShutDown()
    {
        controlEnabled = false;
        thrustEnabled = false;
        rb.drag = 0.0f;
        rb.angularDrag = 0.02f;
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

            if (rotorTilt.magnitude > maxRotorTilt)
            {
                rotorTilt = rotorTilt.normalized * maxRotorTilt;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rotorTilt = Vector3.zero;
        }
    }

    private void LateUpdate()
    {
        GameController.Instance.GameUI.SetUIText(GameUI.UIElement.Fuel, Mathf.RoundToInt(fuel));
    }

    private void FixedUpdate()
    {
        fuel -= fuelBurnRate * Time.fixedDeltaTime;
        if(fuel < 0.0f)
        {
            ShutDown();
        }

        Vector3 worldForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
        worldForward.y = 0;
        worldForward.Normalize();

        Vector3 worldRight = Vector3.Cross(Vector3.up, worldForward);
        Vector3 worldTiltDir = worldRight * rotorTilt.x + worldForward * rotorTilt.y;
        Vector3 tiltAxis = Vector3.Cross(Vector3.up, worldTiltDir).normalized;
        Vector3 tiltVector = Quaternion.AngleAxis(rotorTilt.magnitude, tiltAxis) * Vector3.up;

        if (thrustEnabled)
        {
            if (useAngularVelocity)
            {
                Vector3 targetTilt = useSmoothing ? Vector3.Slerp(transform.up, tiltVector, tiltSmooth * Time.fixedDeltaTime) : tiltVector;
                Quaternion deltaQuat = Quaternion.FromToRotation(transform.up, targetTilt);
                Vector3 axis;
                float angle;
                deltaQuat.ToAngleAxis(out angle, out axis);

                Vector3 yAngVel = transform.up * Vector3.Dot(transform.up, rb.angularVelocity);
                float tiltRate = angle * Mathf.Deg2Rad / Time.fixedDeltaTime;
                rb.maxAngularVelocity = 70;
                rb.angularVelocity = axis * tiltRate + yAngVel;
            }
            else
            {
                Vector3 newRight = Vector3.Cross(tiltVector, transform.forward);
                Vector3 newForward = Vector3.Cross(newRight, tiltVector);
                Quaternion targetRotation = Quaternion.LookRotation(newForward, tiltVector);

                if (useSmoothing)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, tiltSmooth * Time.fixedDeltaTime);
                }
                else
                {
                    transform.rotation = targetRotation;
                }
            }
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

        if (thrustEnabled)
        {
            rb.AddForce(rb.mass * -Physics.gravity + (Vector3.up * vertical * maxVerticalThrust), ForceMode.Force);

            Vector3 dir = transform.up - Vector3.up;
            dir.y = 0;

            rb.AddForce(dir * maxThrust, ForceMode.Force);
        }

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

        rb.AddForceAtPosition(tailOffset.right * turnRate * horizontal, tailOffset.position, ForceMode.Force);

        Vector3 flow = -rb.velocity.normalized;
        float tailDragForce = Vector3.Dot(tailOffset.right, flow) * tailDrag * rb.velocity.sqrMagnitude;
        rb.AddForceAtPosition(tailOffset.right * tailDragForce, tailOffset.position, ForceMode.Force);

        speed = rb.velocity.magnitude * 60 * 60 / 1000;
    }

    public void OnItemPickedUp(Pickup pickup)
    {
        Refuel();
        GetComponent<Health>().Heal();
    }
}
