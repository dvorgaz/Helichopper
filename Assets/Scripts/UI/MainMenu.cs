using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private ScreenFader screenFader;

    private void Start()
    {
        screenFader.Fade(-1, () => ShowCursor(true));
    }

    public void StartCampaign()
    {
        ShowCursor(false);
        screenFader.Fade(1, () => SceneManager.LoadScene("SampleScene"));
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ShowCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
