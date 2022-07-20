using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mission : MonoBehaviour
{
    public string title;
    public string description;
    private Task[] tasks;
    public bool completed;
    public UnityEvent missionCompleted;    

    private void Awake()
    {
        tasks = GetComponentsInChildren<Task>();

#if UNITY_EDITOR
        if(title.Length > 0) name = title;
#endif
    }

    public bool CheckCompleted()
    {
        if (completed)
            return true;

        foreach(Task task in tasks)
        {
            if (!task.completed)
                return false;
        }

#if UNITY_EDITOR
        name += " (Completed)";
#endif

        completed = true;
        missionCompleted.Invoke();
        Debug.Log("Mission " + title + " completed");

        return true;
    }

    public List<Vector3> GetMarkerPositions()
    {
        List<Vector3> markers = new List<Vector3>();

        foreach(Task task in tasks)
        {
            if (task.taskSubject != null && !task.completed)
            {
                markers.Add(task.taskSubject.transform.position);
            }
        }

        return markers;
    }
}
