using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Task : MonoBehaviour
{
    public GameObject taskSubject;
    public bool completed;
    public int target = 1;
    public int progress = 0;
    public UnityEvent taskCompleted;

    public void ProcessEvent(GameObject subject)
    {
        if (taskSubject == null || subject == taskSubject)
        {
            progress++;
        }

        if(!completed && progress >= target)
        {
#if UNITY_EDITOR
            name += " (Completed)";
#endif

            completed = true;
            taskCompleted.Invoke();
            MissionController.Instance.CheckMissionsCompleted();
        }
    }
}
