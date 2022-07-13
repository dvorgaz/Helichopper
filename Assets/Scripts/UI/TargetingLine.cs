using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingLine : MonoBehaviour
{
    public Transform player;
    public float thickness;
    public int numSegments;
    public float startDist;
    public float endDist;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        lineRenderer.startWidth = thickness;
        lineRenderer.endWidth = thickness;
        
        Vector3 dir = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up).normalized;
        Vector3 pos = player.transform.position + dir * startDist;
        float segLength = (endDist - startDist) / (numSegments - 1);

        Vector3[] ropePositions = new Vector3[numSegments];
        for (int i = 0; i < numSegments; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos + Vector3.up * 100.0f, -Vector3.up, out hit, 200.0f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
            {
                ropePositions[i] = hit.point;
            }

            pos += dir * segLength;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }
}
