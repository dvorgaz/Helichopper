using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingLine : MonoBehaviour
{
    public enum Symbol
    {
        TargetingLine,
        GroundPosition,
        Velocity
    }

    public Symbol symbology;
    private Transform player;
    private HeliController heli;
    private WeaponController weapon;

    public float thickness;
    public int numSegments;
    public float startDist;
    public float endDist;
    public float radius;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {        
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = thickness;
        lineRenderer.endWidth = thickness;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(player == null)
        {
            GameObject po = GameController.Instance.Player;
            if (po == null)
                return;

            player = po.transform;
            heli = player.GetComponent<HeliController>();
            weapon = player.GetComponent<WeaponController>();
        }

        switch(symbology)
        {
            case Symbol.TargetingLine:
                DrawTargetingLine2();
                break;
            case Symbol.GroundPosition:
                DrawGroundPosition();
                break;
            case Symbol.Velocity:
                DrawVelocity();
                break;
        }        
    }

    private void DrawTargetingLine()
    {
        Vector3 dir = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up).normalized;
        Vector3 pos = player.transform.position + dir * startDist;
        float segLength = (endDist - startDist) / (numSegments - 1);

        Vector3[] positions = new Vector3[numSegments];
        for (int i = 0; i < numSegments; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos + Vector3.up * 100.0f, -Vector3.up, out hit, 200.0f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
            {
                positions[i] = hit.point;
            }
            else
            {
                positions[i] = pos;
            }

            pos += dir * segLength;
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    private void DrawTargetingLine2()
    {
        if(!Input.GetMouseButton(1))
        {
            lineRenderer.positionCount = 0;
            return;
        }

        Vector3 dir = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up).normalized;
        Vector3 pos = player.transform.position;
        
        Vector3 idleDir = Quaternion.AngleAxis(45.0f, Vector3.Cross(Vector3.up, dir)) * dir;
        RaycastHit hit_;
        if (Physics.Raycast(pos, idleDir, out hit_, 200.0f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            pos = new Vector3(hit_.point.x, pos.y, hit_.point.z);
            startDist = (player.transform.position - pos).magnitude;
        }

        float segLength = (endDist - startDist) / (numSegments - 1);

        Vector3[] positions = new Vector3[numSegments];
        for (int i = 0; i < numSegments; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos + Vector3.up * 100.0f, -Vector3.up, out hit, 200.0f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
            {
                positions[i] = hit.point;
            }
            else
            {
                positions[i] = pos;
            }

            pos += dir * segLength;
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    private void DrawGroundPosition()
    {
        Vector3 pos = player.transform.position;

        RaycastHit hit;
        if (Physics.Raycast(pos, -Vector3.up, out hit, 200.0f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            pos = hit.point;
        }
        
        float deltaAngle = (360.0f / numSegments) * Mathf.Deg2Rad;
        float angle = 0.0f;

        Vector3[] points = new Vector3[numSegments];
        for (int i = 0; i < numSegments; i++)
        {
            Vector3 point = pos + new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)) * radius;
            points[i] = point;
            angle += deltaAngle;
        }

        lineRenderer.positionCount = points.Length;
        lineRenderer.loop = true;
        lineRenderer.SetPositions(points);
    }

    private void DrawVelocity()
    {
        Vector3 pos = player.transform.position;

        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 100.0f, -Vector3.up, out hit, 200.0f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            //pos = hit.point;           
        }

        /*
            pos += transform.up * startDist;
            float deltaAngle = (360.0f / numSegments) * Mathf.Deg2Rad;
            float angle = 0.0f;

            Vector3[] points = new Vector3[numSegments];
            for (int i = 0; i < numSegments; i++)
            {
                Vector3 point = pos + new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)) * radius;
                points[i] = point;
                angle += deltaAngle;
            }

            lineRenderer.positionCount = points.Length;
            lineRenderer.loop = true;
            lineRenderer.SetPositions(points);
           */

        //pos = hit.point;
        //pos += player.transform.up * endDist;
        Vector3 endPos = pos + heli.TiltDirection * radius;
        //pos = heli.TiltDirection.magnitude * radius < startDist ? pos : (endPos + (pos - endPos).normalized * startDist);
        endPos = pos + transform.up * endDist;
        pos += transform.up * startDist;

        Vector3[] points = new Vector3[2];
        points[0] = pos;
        points[1] = endPos;

        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }
}
