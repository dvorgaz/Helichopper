using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public float Horizontal { get; set; } = 0.0f;
    public float Vertical { get; set; } = 0.0f;
    public bool StabilizeAim { get; set; } = false;

    public Vector3 TiltDirection { get { return worldTiltDir; } }

    private Transform cameraTransform;
    private Transform groundContact;
    private Transform tailOffset;
    [SerializeField] private float tailDrag;
    [SerializeField] private float landingHeight;
    [SerializeField] private float maxAltitude;

    private bool thrustEnabled = true;
    private bool controlEnabled = true;
    private int isInLandingZone = 0;
    private bool isOnGround = false;
    private float lastTakeoffTime = 0.0f;
    [SerializeField] private UnityEvent<GameObject> onLanding;
    private GameObject landingZone;

    public bool Landing { get { return isInLandingZone > 0; } }

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
        lastTakeoffTime = Time.time;
    }

    public void OnDeath()
    {
        ShutDown();
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

    public void SetRotorTilt(Vector3 tilt)
    {
        rotorTilt = tilt;

        if (rotorTilt.magnitude > maxRotorTilt)
        {
            rotorTilt = rotorTilt.normalized * maxRotorTilt;
        }
    }

    public void SetRotorTiltDelta(Vector3 delta)
    {
        SetRotorTilt(rotorTilt + delta);
    }

    // Update is called once per frame
    void Update()
    {

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
            if (Landing)
                dir *= 0.5f;

            rb.AddForce(dir * maxThrust, ForceMode.Force);
        }
    }

    private void UpdateVerticalMove()
    {
        bool ignoreLanding = false;

        Vector3 vThrust = Vector3.zero;

        if (controlEnabled)
        {
            vThrust = Vector3.up * Vertical * maxVerticalThrust;

            float epsilon = 0.01f;
            if(Vertical > epsilon)
            {
                ignoreLanding = true;
                if (isOnGround)
                {
                    isOnGround = false;
                    thrustEnabled = true;
                    lastTakeoffTime = Time.time;
                }
            }
            else if (Vertical < -epsilon && Landing)
            {
                vThrust *= 0.5f;
            }
        }

        if (thrustEnabled && transform.position.y < maxAltitude)
        {
            rb.AddForce(rb.mass * -Physics.gravity + vThrust, ForceMode.Force);
        }

        if(!ignoreLanding)
            CheckLanding();
    }

    private void UpdateHorizontalTurn()
    {
        if (controlEnabled)
        {
            Vector3 hThrust = tailOffset.right * turnRate * Horizontal;
            if (StabilizeAim)
                hThrust *= 0.3f;

            rb.AddForceAtPosition(hThrust, tailOffset.position, ForceMode.Force);
        }

        if (!StabilizeAim)
        {
            Vector3 flow = -rb.velocity.normalized;
            float tailDragForce = Vector3.Dot(tailOffset.right, flow) * tailDrag * rb.velocity.sqrMagnitude;
            rb.AddForceAtPosition(tailOffset.right * tailDragForce, tailOffset.position, ForceMode.Force);
        }
    }

    private void CheckLanding()
    {
        if (!Landing)
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
            isInLandingZone = Mathf.Max(isInLandingZone + 1, 1);
            landingZone = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LandingZone"))
        {
            Debug.Log("Leaving landing zone");
            isInLandingZone = Mathf.Max(isInLandingZone - 1, 0);
            if (!Landing)
                landingZone = null;
        }
    }

    private void OnLanded()
    {
        if (Time.time - lastTakeoffTime > 1.0f)
        {           
            Invoke("TryRearm", 2.0f);
        }
    }

    private void TryRearm()
    {
        if(isOnGround)
        {
            if (onLanding != null)
                onLanding.Invoke(landingZone);
        }
    }
}
