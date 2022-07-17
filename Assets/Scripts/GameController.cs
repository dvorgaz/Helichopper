using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] private GameObject gameUIPrefab;
    public GameUI GameUI { get; private set; }

    private GameObject player;

    public bool CanProcessGameInput { get; private set; } = false;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogError("GameController instance already exists");
        }
        else
        {
            Instance = this;
        }

        GameUI = Instantiate(gameUIPrefab).GetComponent<GameUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ShowRearmMenu(true);
        GameUI.ShowRetryButton(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Retry();
        }
    }

    public void OnLandedOnBase()
    {
        ShowRearmMenu(true);
    }

    public void ShowCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void ShowRearmMenu(bool visible)
    {
        CanProcessGameInput = !visible;
        ShowCursor(visible);
        Time.timeScale = visible ? 0.0f : 1.0f;
        GameUI.ShowRearmPanel(visible);
        player.SendMessage("ControlEnable", !visible);
    }

    public void Retry()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnPlayerDead()
    {
        ShowCursor(true);
        GameUI.ShowRetryButton(true);
    }
}
