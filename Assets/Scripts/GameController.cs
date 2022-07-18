using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] private GameObject gameUIPrefab;
    public GameUI GameUI { get; private set; }

    private GameObject playerTemplate;
    public GameObject Player { get; private set; }

    [SerializeField] private GameEvent playerDeathEvent;
    [SerializeField] private GameEvent playerLandingEvent;

    [SerializeField] private int lives = 3;
    private bool playerWasKilled = false;

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

    private void OnEnable()
    {
        playerDeathEvent.AddListener(OnPlayerDead);
        playerLandingEvent.AddListener(OnLandedOnBase);
    }

    private void OnDisable()
    {
        playerDeathEvent.RemoveListener(OnPlayerDead);
        playerLandingEvent.RemoveListener(OnLandedOnBase);
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

        StartCoroutine(GameLoopCoroutine());
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
        ShowRearmMenu(true);
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

    public void Retry()
    {
        SceneManager.LoadScene("SampleScene");
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
        SetPlayerInputEnable(false);
        yield return new WaitForSeconds(1.0f);

        SpawnPlayer();
        SetPlayerInputEnable(true);

        while(lives > 0)
        {
            if(playerWasKilled)
            {
                playerWasKilled = false;
                yield return new WaitForSeconds(3.0f);

                SpawnPlayer();
            }

            yield return null;
        }

        yield return new WaitForSeconds(2.0f);

        ShowCursor(true);
        GameUI.ShowRetryButton(true);
    }
}
