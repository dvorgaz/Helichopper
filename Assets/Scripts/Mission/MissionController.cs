using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    public static MissionController Instance { get; private set; }

    private Mission[] missions;

    public int MissionCount { get { return missions.Length; } }

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

    public Mission[] GetMissions()
    {
        return missions;
    }

    public Mission GetMission(int idx)
    {
        if(idx >= 0 && idx < missions.Length)
        {
            return missions[idx];
        }

        return null;
    }

    public void CheckMissionsCompleted()
    {
        bool allCompleted = true;
        foreach(Mission mission in missions)
        {
            allCompleted &= mission.CheckCompleted();
        }

        if(allCompleted)
        {
            GameController.Instance.OnAllMissionsCompleted();
            GameController.Instance.ShowNotification("Return to Base");
        }
    }
}
