using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MFD_Mission : MFD_Page
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI selectedMissionText;

    public override void Display()
    {
        Mission msn = MissionController.Instance.GetMission(IngameMenu.Instance.SelectedMission);
        if (msn != null)
        {
            titleText.text = msn.title;
            descriptionText.text = msn.description;
            selectedMissionText.text = "MISSION " + (IngameMenu.Instance.SelectedMission + 1).ToString();
        }
    }

    public override void ProcessButton(int idx)
    {
        switch(idx)
        {
            case 7:
                IngameMenu.Instance.SelectedMission--;
                Display();
                break;
            case 8:
                IngameMenu.Instance.SelectedMission++;
                Display();
                break;
        }
    }
}
