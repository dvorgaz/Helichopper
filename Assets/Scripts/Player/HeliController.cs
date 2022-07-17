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
    public float Fuel { get; private set; }

    [SerializeField] private float speed; // Speed in km/h

    [SerializeField] private Vector3 rotorTilt = Vector3.zero;
    [SerializeField] private float maxRotorTilt;
    [SerializeField] private float tiltSmooth;
    [SerializeField] private bool useSmoothing;
    private Vector3 worldTiltDir;

    public Vector3 TiltDirection { get { return worldTiltDir; } }

    private Transform cameraTransform;
    private Transform groundContact;
    private Transform tailOffset;
    [SerializeField] private float tailDrag;
    [SerializeField] private float landingHeight;
    [SerializeField] private float maxAltitude;

    private bool controllingCyclic = false;
    private bool thrustEnabled = true;
    private bool controlEnabled = true;
    private bool isLanding = false;
    private bool isOnGround = false;
    private float lastLandingTime = 0.0f;

    private void Awake()
    {
        tailOffset = transform.Find("TailOffset");
        if (tailOffset == null)
            Debug.LogError("Object: " + gameObject.name + " missing TailOffset node");

        groundContact = transform.Find("GroundContact");
        if (groundContact == null)
            Debug.LogError("Object: " + gameObject.name + " missing GroundContact node");
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        rb.maxAngularVelocity = 70;
        cameraTransform = Camera.main.transform;
        Fuel = maxFuel;
        lastLandingTime = Time.time;
    }

    void ToggleCyclicControl()
    {
        SetCyclicControl(!controllingCyclic);
    }

    void SetCyclicControl(bool enable)
    {
        controllingCyclic = enable;
        Cursor.visible = !controllingCyclic;
        Cursor.lockState = controllingCyclic ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void OnDeath()
    {
        ShutDown();
        Invoke("NotifyDeath", 2.0f);
    }

    private void NotifyDeath()
    {
        GameController.Instance.OnPlayerDead();
    }

    public void Refuel()
    {
        Fuel = maxFuel;
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
        if (GameController.Instance.CanProcessGameInput)
        {
            if (Input.GetMouseButtonDown(1))
            {
                ToggleCyclicControl();
            }
            //if (Input.GetMouseButtonUp(2))
            //{
            //    thrustEnabled = !thrustEnabled && controlEnabled;
            //}

            Vector3 mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

            if (controllingCyclic && thrustEnabled)
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
    }

    private void FixedUpdate()
    {
        Fuel -= fuelBurnRate * Time.fixedDeltaTime;
        if(Fuel < 0.0f)
        {
            ShutDown();
        }

        UpdateRotorTilt();
        UpdateVerticalMove();
        UpdateHorizontalTurn();        

        speed = rb.velocity.magnitude * 60 * 60 / 1000;
    }

    private void UpdateRotorTilt()
    {
        Vector3 worldForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
        worldForward.y = 0;
        worldForward.Normalize();

        Vector3 worldRight = Vector3.Cross(Vector3.up, worldForward);
        worldTiltDir = worldRight * rotorTilt.x + worldForward * rotorTilt.y;
        Vector3 tiltAxis = Vector3.Cross(Vector3.up, worldTiltDir).normalized;
        Vector3 tiltVector = Quaternion.AngleAxis(rotorTilt.magnitude, tiltAxis) * Vector3.up;

        if (thrustEnabled)
        {
            Vector3 targetTilt = useSmoothing ? Vector3.Slerp(transform.up, tiltVector, tiltSmooth * Time.fixedDeltaTime) : tiltVector;
            Quaternion deltaQuat = Quaternion.FromToRotation(transform.up, targetTilt);
            Vector3 axis;
            float angle;
            deltaQuat.ToAngleAxis(out angle, out axis);

            Vector3 yAngVel = transform.up * Vector3.Dot(transform.up, rb.angularVelocity);
            float tiltRate = angle * Mathf.Deg2Rad / Time.fixedDeltaTime;            
            rb.angularVelocity = axis * tiltRate + yAngVel;

            Vector3 dir = Vector3.ProjectOnPlane(transform.up, Vector3.up);
            if (isLanding)
                dir *= 0.5f;

            rb.AddForce(dir * maxThrust, ForceMode.Force);
        }
    }

    private void UpdateVerticalMove()
    {
        float vertical = 0.0f;
        bool ignoreLanding = false;

        if (controlEnabled)
        {
            if (Input.GetKey(KeyCode.W))
            {
                vertical += 1;

                ignoreLanding = true;
                if (isOnGround)
                {
                    isOnGround = false;
                    thrustEnabled = true;
                    lastLandingTime = Time.time;
                }
            }

            if (Input.GetKey(KeyCode.S))
            {
                vertical -= 1 * (isLanding ? 0.5f : 1.0f);
            }
        }

        if (thrustEnabled && transform.position.y < maxAltitude)
        {
            rb.AddForce(rb.mass * -Physics.gravity + (Vector3.up * vertical * maxVerticalThrust), ForceMode.Force);
        }

        if(!ignoreLanding)
            CheckLanding();
    }

    private void UpdateHorizontalTurn()
    {
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

        if(Input.GetMouseButton(0))
        {
            horizontal *= 0.3f;
        }

        rb.AddForceAtPosition(tailOffset.right * turnRate * horizontal, tailOffset.position, ForceMode.Force);

        if (!Input.GetMouseButton(0))
        {
            Vector3 flow = -rb.velocity.normalized;
            float tailDragForce = Vector3.Dot(tailOffset.right, flow) * tailDrag * rb.velocity.sqrMagnitude;
            rb.AddForceAtPosition(tailOffset.right * tailDragForce, tailOffset.position, ForceMode.Force);
        }
    }

    private void CheckLanding()
    {
        if (!isLanding)
            return;

        float heightOffset = 10.0f;
        RaycastHit hit;
        if (Physics.Raycast(groundContact.position + Vector3.up * heightOffset, -Vector3.up, out hit, heightOffset + landingHeight + 1.0f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            if(hit.distance < heightOffset + landingHeight)
            {
                if (!isOnGround)
                    OnLanded();

                isOnGround = true;
                thrustEnabled = false;
                rotorTilt = Vector3.zero;                
            }
        }
    }

    public void OnItemPickedUp(Pickup pickup)
    {
        Refuel();
        GetComponent<Health>().Heal();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("LandingZone"))
        {
            Debug.Log("Entering landing zone");
            isLanding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LandingZone"))
        {
            Debug.Log("Leaving landing zone");
            isLanding = false;
        }
    }

    private void OnLanded()
    {
        if (Time.time - lastLandingTime > 1.0f)
        {
            Invoke("TryRearm", 2.0f);
        }
    }

    private void TryRearm()
    {
        if(isOnGround)
        {
            GameController.Instance.OnLandedOnBase();
        }
    }

    public void ControlEnable(bool enabled)
    {
        SetCyclicControl(enabled);
    }
}
