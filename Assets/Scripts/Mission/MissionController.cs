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

        missions = GetComponentsInChildren<Mission>(true);
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
        for(int i = 0; i < missions.Length; ++i)
        {
            bool wasCompleted = missions[i].completed;
            allCompleted &= missions[i].CheckCompleted();

            if(!wasCompleted && missions[i].completed)
            {
                GameController.Instance.ShowNotification(string.Format("Mission {0} completed", (i + 1).ToString()));
            }
        }

        if(allCompleted)
        {
            GameController.Instance.OnAllMissionsCompleted();
            GameController.Instance.ShowNotification("Return to Base");
        }
    }
}
