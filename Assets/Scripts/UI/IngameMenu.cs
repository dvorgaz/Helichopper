using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMenu : MonoBehaviour
{
    public static IngameMenu Instance { get; private set; }

    public enum MFDScreen
    {
        Map,
        Mission,
        Status,
        Num
    }

    [SerializeField] private GameObject[] menuScreens;
    private MFDScreen selectedScreen;
    private int selectedMission = 0;

    public int SelectedMission
    {
        get { return selectedMission; }
        set { selectedMission = (value + MissionController.Instance.MissionCount) % MissionController.Instance.MissionCount; }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogError("IngameMenu instance already exists");
        }
        else
        {
            Instance = this;
        }

        if (menuScreens.Length != (int)MFDScreen.Num)
        {
            Debug.LogError("IngameMenu: invlaud number of menu screens");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ShowScreen(MFDScreen.Map);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        ShowScreen(selectedScreen);
    }

    public void ShowScreen(MFDScreen scr)
    {
        selectedScreen = scr;
        for (int i = 0; i < menuScreens.Length; ++i)
        {
            menuScreens[i].SetActive(i == (int)scr);
            if (i == (int)scr)
                menuScreens[(int)selectedScreen].GetComponent<MFD_Page>().Display();
        }
    }

    public void OnButtonPressed(int idx)
    {
        switch(idx)
        {
            case 18:
                ShowScreen(MFDScreen.Map);
                return;
            case 16:
                ShowScreen(MFDScreen.Mission);
                return;
            case 13:
                ShowScreen(MFDScreen.Status);
                return;
        }

        menuScreens[(int)selectedScreen].GetComponent<MFD_Page>().ProcessButton(idx);
    }
}
