using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    private List<Transform> pathPoints;
    [SerializeField] private bool loop = true;

    public class Tracker
    {
        private PatrolRoute patrolRoute;
        private int pointIdx = 0;
        private bool reverse = false;

        public Tracker(PatrolRoute pr)
        {
            patrolRoute = pr;
        }

        public Vector3 SteerPoint
        {
            get
            {
                return patrolRoute.pathPoints[pointIdx].position;
            }
        }

        public void Next()
        {
            int numPoints = patrolRoute.pathPoints.Count;
            if (patrolRoute.loop)
            {
                pointIdx = (pointIdx + (reverse ? numPoints - 1 : 1)) % numPoints;
            }
            else if(numPoints > 1)
            {
                int nextIdx = pointIdx + (reverse ? -1 : 1);
                if(nextIdx < 0 || nextIdx >= numPoints)
                {
                    reverse = !reverse;
                    Next();
                }
                else
                {
                    pointIdx = nextIdx;
                }
            }
        }
    }

    public Tracker GetTracker()
    {
        return new Tracker(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        pathPoints = new List<Transform>();

        if (transform.childCount == 0)
        {
            Debug.LogError("Patrol route: " + gameObject.name + " path points empty");
            GameObject obj = new GameObject("Dummy");
            obj.transform.parent = transform;
            obj.transform.position = transform.position;
        }

        foreach(Transform child in transform)
        {
            pathPoints.Add(child);
        }
    }

    private void OnDrawGizmos()
    {        
        int numPoints = transform.childCount;

        if (numPoints > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < numPoints; ++i)
            {
                if (i < numPoints - 1)
                {
                    Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
                }
                else if (loop)
                {
                    Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(0).position);
                }
            }

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.GetChild(0).position);
        }
    }
}