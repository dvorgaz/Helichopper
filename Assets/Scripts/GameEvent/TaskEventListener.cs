using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Task))]
public class TaskEventListener : GameEventListener
{
    private Task task;

    private void Awake()
    {
        task = GetComponent<Task>();
    }

    public override void OnEventRaised(GameObject subject)
    {
        task.ProcessEvent(subject);
    }
}
