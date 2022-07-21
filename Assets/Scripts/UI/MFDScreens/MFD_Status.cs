using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MFD_Status : MFD_Page
{
    [SerializeField] private TextMeshProUGUI statusText;

    public override void Display()
    {
        string status = "";

        bool allComplete = true;
        for(int i = 0; i < MissionController.Instance.MissionCount; ++i)
        {
            Mission msn = MissionController.Instance.GetMission(i);
            allComplete &= msn.completed;
            status += (i + 1).ToString() + " - " + msn.title.ToUpper() + ": " + (msn.completed ? "COMPLETED" : "PENDING") + "\n";
        }

        if(allComplete)
        {
            status += "\n\nAll missions completed. Return to base.";
        }

        statusText.text = status;
    }

    public override void ProcessButton(int idx)
    {
        
    }
}
