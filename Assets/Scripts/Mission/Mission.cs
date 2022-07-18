using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    [System.Serializable]
    public class Task
    {
        public GameEvent taskEvent;
        public GameObject taskSubject;
        public bool completed = false;

        public void ProcessEvent(GameObject subject)
        {
            if(taskSubject == null || subject == taskSubject)
            {
                completed = true;
            }
        }
    }

    [SerializeField] private string title;
    [SerializeField] private string description;
    [SerializeField] private List<Task> tasks;    

    public void OnEnable()
    {
        foreach(Task task in tasks)
        {
            task.taskEvent.AddListener(task.ProcessEvent);
        }
    }

    public void OnDisable()
    {
        foreach (Task task in tasks)
        {
            task.taskEvent.RemoveListener(task.ProcessEvent);
        }
    }
}
