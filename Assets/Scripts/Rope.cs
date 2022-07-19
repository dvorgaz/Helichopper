using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Rope : MonoBehaviour
{
    private struct RopeSegment
    {
        public Vector3 position;
        public Vector3 lastPosition;

        public RopeSegment(Vector3 pos)
        {
            lastPosition = position = pos;
        }
    }

    [SerializeField] private float length;
    private float segmentLength;
    [Range(3, 20)] [SerializeField] private int numSegments;    
    [SerializeField] private float thickness;
    [Range(1, 50)] [SerializeField] private int iterations;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private LineRenderer lineRenderer;
    [SerializeField] private Transform ropeEndNode;
    private float targetPosition = 0.0f;
    [SerializeField] private float speed;
    [SerializeField] float rotationSmoothing;

    public delegate void RopeDelegate();
    public RopeDelegate OnRetracted;
    public RopeDelegate OnExtended;

    public float Position { get; private set; } = 0.0f;

    public float TargetPosition
    {
        get { return targetPosition; }
        set { targetPosition = Mathf.Clamp(value, 0.0f, 1.0f); }
    }

    public Vector3 PickupConstraint
    {
        get;
        set;
    }

    public Transform GetEndNode()
    {
        return ropeEndNode;
    }

    private void Awake()
    {
        segmentLength = length / (numSegments - 1);

        Vector3 ropeStartPoint = transform.position;
        for (int i = 0; i < numSegments; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= segmentLength;
        }

        ropeEndNode.position = ropeSegments[ropeSegments.Count - 1].position;

        PickupConstraint = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float epsilon = 0.01f;
        bool wasRetracted = Position < epsilon;
        bool wasExtended = Position > 1.0f - epsilon;

        Position = Mathf.MoveTowards(Position, TargetPosition, speed * Time.deltaTime);
        Position = Mathf.Clamp(Position, 0.0f, 1.0f);

        if(!wasRetracted && Position < epsilon && OnRetracted != null)
        {
            OnRetracted();
        }

        if (!wasExtended && (Position > 1.0f - epsilon) && OnExtended != null)
        {
            OnExtended();
        }

        DrawRope();
    }

    private void FixedUpdate()
    {
        Simulate();

        Vector3 dir = ropeSegments[ropeSegments.Count - 2].position - ropeSegments[ropeSegments.Count - 1].position;
        dir.Normalize();
        Quaternion targetRot = Quaternion.FromToRotation(ropeEndNode.up, dir) * ropeEndNode.rotation;
        ropeEndNode.rotation = Quaternion.Slerp(ropeEndNode.rotation, targetRot, Time.fixedDeltaTime * rotationSmoothing);
        ropeEndNode.position = ropeSegments[ropeSegments.Count - 1].position;
    }

    private void DrawRope()
    {
        Vector3 offset = transform.position - ropeSegments[0].position;

        lineRenderer.startWidth = thickness;
        lineRenderer.endWidth = thickness;

        Vector3[] ropePositions = new Vector3[numSegments];
        for (int i = 0; i < numSegments; i++)
        {
            ropePositions[i] = ropeSegments[i].position + offset;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
        ropeEndNode.position = ropeSegments[ropeSegments.Count - 1].position + offset;
    }

    private void Simulate()
    {
        for (int i = 1; i < numSegments; i++)
        {
            RopeSegment firstSegment = ropeSegments[i];
            Vector3 velocity = firstSegment.position - firstSegment.lastPosition;
            firstSegment.lastPosition = firstSegment.position;
            firstSegment.position += velocity;
            firstSegment.position += Physics.gravity * Time.fixedDeltaTime;
            ropeSegments[i] = firstSegment;
        }

        for (int i = 0; i < iterations; i++)
        {
            ApplyConstraints();
        }

        if (PickupConstraint != Vector3.zero)
        {
            for (int i = 1; i < numSegments; i++)
            {
                RopeSegment firstSegment = ropeSegments[i];
                firstSegment.lastPosition = firstSegment.position;
                ropeSegments[i] = firstSegment;
            }

            PickupConstraint = Vector3.zero;
        }
    }

    private void ApplyConstraints()
    {
        RopeSegment firstSegment = ropeSegments[0];
        firstSegment.position = transform.position;
        ropeSegments[0] = firstSegment;

        float invPos = 1.0f - Position;
        float s = 1.0f / (numSegments - 1);
        int sectionIdx = Mathf.FloorToInt(invPos / s);
        float sectionFraction = 1.0f - (Mathf.Repeat(invPos, s) / s);

        for (int i = 0; i < numSegments - 1; i++)
        {
            RopeSegment firstSeg = ropeSegments[i];
            RopeSegment secondSeg = ropeSegments[i + 1];

            if((i == numSegments - 2) && PickupConstraint != Vector3.zero)
            {
                secondSeg.position = PickupConstraint;
            }

            if(i <= sectionIdx)
            {
                firstSeg.position = transform.position;
            }

            float desiredDist = segmentLength;
            if (i == sectionIdx)
            {
                desiredDist *= sectionFraction;
            }

            float dist = (firstSeg.position - secondSeg.position).magnitude;
            float error = Mathf.Abs(dist - desiredDist);
            Vector3 changeDir = Vector3.zero;

            if (dist > desiredDist)
            {
                changeDir = (firstSeg.position - secondSeg.position).normalized;
            }
            else if (dist < desiredDist)
            {
                changeDir = (secondSeg.position - firstSeg.position).normalized;
            }

            Vector3 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.position -= changeAmount * 0.5f;
                ropeSegments[i] = firstSeg;
                secondSeg.position += changeAmount * 0.5f;
                ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.position += changeAmount;
                ropeSegments[i + 1] = secondSeg;
            }
        }
    }
}
