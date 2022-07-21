using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] private GameObject gameUIPrefab;
    [SerializeField] private ScreenFader screenFader;
    public GameUI GameUI { get; private set; }

    private GameObject playerTemplate;
    public GameObject Player { get; private set; }

    [SerializeField] private int lives = 3;
    private bool playerWasKilled = false;
    private bool campaignCompleted = false;
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
        playerTemplate = GameObject.FindGameObjectWithTag("Player");
        if(playerTemplate != null)
        {
            playerTemplate.SetActive(false);
        }
        else
        {
            Debug.LogError("Missing player template");
        }

        ShowRearmMenu(false);
        GameUI.ShowRetryButton(false);

        screenFader.Fade(-1);
        StartCoroutine(GameLoopCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainMenu();
            //Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Retry();
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            //ShowNotification("sdfsdf");
            StopAllCoroutines();
            screenFader.Fade(1, () => SceneManager.LoadScene("VictoryScene"));
        }
    }

    public void SpawnPlayer()
    {
        if(Player != null)
        {
            Destroy(Player);
            Player = null;
        }

        Player = Instantiate(playerTemplate);
        Player.SetActive(true);
    }

    public void SetPlayerInputEnable(bool enable)
    {
        if(Player != null)
        {
            Player.GetComponent<PlayerInputHandler>().enabled = enable;
        }
    }

    public void OnLandedOnBase(GameObject landingZone)
    {
        LandingZone lz = landingZone.GetComponent<LandingZone>();
        if (lz != null && lz.type == LandingZone.Type.HomeBase)
        {
            if (campaignCompleted)
            {
                StopAllCoroutines();
                screenFader.Fade(1, () => SceneManager.LoadScene("VictoryScene"));
            }
            else
            {
                ShowRearmMenu(true);
            }
        }
    }

    public void ShowCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void ShowRearmMenu(bool visible)
    {
        ShowCursor(visible);
        Time.timeScale = visible ? 0.0f : 1.0f;
        GameUI.ShowRearmPanel(visible);
        SetPlayerInputEnable(!visible);
    }

    public void ShowIngameMenu(bool visible)
    {
        ShowCursor(visible);
        Time.timeScale = visible ? 0.0f : 1.0f;
        GameUI.ShowIngameMenu(visible);
        SetPlayerInputEnable(!visible);
    }

    public void Retry()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ReturnToMainMenu()
    {
        StopAllCoroutines();
        screenFader.Fade(1, () => SceneManager.LoadScene("MainMenu"));
    }

    public void OnPlayerDead(GameObject player)
    {
        if (this.Player == player)
        {
            lives--;
            playerWasKilled = true;
            //ShowCursor(true);
            //GameUI.ShowRetryButton(true);
        }
    }

    public IEnumerator GameLoopCoroutine()
    {
        SpawnPlayer();
        SetPlayerInputEnable(false);

        yield return new WaitForSeconds(1.0f);
        
        SetPlayerInputEnable(true);

        while(lives > 0)
        {
            if(playerWasKilled)
            {
                playerWasKilled = false;
                yield return new WaitForSeconds(3.0f);

                SpawnPlayer();
            }

            if(Input.GetKeyDown(KeyCode.Tab))
            {
                ShowIngameMenu(!GameUI.IsIngameMenuOpen());
            }

            yield return null;
        }

        yield return new WaitForSeconds(2.0f);

        ShowCursor(true);
        GameUI.ShowRetryButton(true);
    }

    public Vector3 WorldToMapPos(Vector3 worldPos)
    {
        // TODO
        float mapSize = 1000.0f;
        return new Vector3(worldPos.x / mapSize, worldPos.z / mapSize, 0.0f);
    }

    public void OnAllMissionsCompleted()
    {
        campaignCompleted = true;
    }

    public void ShowNotification(string text)
    {
        GameUI.ShowNotification(text);
    }
}
