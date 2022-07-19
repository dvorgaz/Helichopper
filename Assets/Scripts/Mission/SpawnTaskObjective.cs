using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Task))]
public class SpawnTaskObjective : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool activateTaskAfterSpawn;

    private void Awake()
    {
        if (activateTaskAfterSpawn)
        {
            GetComponent<TaskEventListener>().enabled = false;
        }
    }

    public void Spawn()
    {
        GameObject obj = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);
        Task task = GetComponent<Task>();
        task.taskSubject = obj;

        if(activateTaskAfterSpawn)
        {
            GetComponent<TaskEventListener>().enabled = true;
        }
    }
}
