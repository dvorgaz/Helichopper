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

        for(int i = 0; i < MissionController.Instance.MissionCount; ++i)
        {
            Mission msn = MissionController.Instance.GetMission(i);
            status += (i + 1).ToString() + " - " + msn.title + ": " + (msn.completed ? "COMPLETED" : "PENDING") + "\n";
        }

        statusText.text = status;
    }

    public override void ProcessButton(int idx)
    {
        
    }
}
