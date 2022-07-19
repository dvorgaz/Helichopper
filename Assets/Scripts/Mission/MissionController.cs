using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    public static MissionController Instance { get; private set; }

    private Mission[] missions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogError("MissionController instance already exists");
        }
        else
        {
            Instance = this;
        }

        missions = GetComponentsInChildren<Mission>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckMissionsCompleted()
    {
        foreach(Mission mission in missions)
        {
            mission.CheckCompleted();
        }
    }
}
