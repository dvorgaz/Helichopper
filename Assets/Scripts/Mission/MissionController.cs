using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    [SerializeField] private List<Mission> missions;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        foreach(Mission mission in missions)
        {
            mission.OnEnable();
        }
    }

    private void OnDisable()
    {
        foreach (Mission mission in missions)
        {
            mission.OnDisable();
        }
    }
}
