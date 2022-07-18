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

    [SerializeField] private GameEvent playerDeathEvent;
    [SerializeField] private GameEvent playerLandingEvent;

    int lives = 1;

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
        player = GameObject.FindGameObjectWithTag("Player");
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

    public void SetPlayerInputEnable(bool enable)
    {
        if(player != null)
        {
            player.GetComponent<PlayerInputHandler>().enabled = enable;
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
        if (this.player == player)
        {
            lives--;
            //ShowCursor(true);
            //GameUI.ShowRetryButton(true);
        }
    }

    public IEnumerator GameLoopCoroutine()
    {
        SetPlayerInputEnable(false);
        yield return new WaitForSeconds(1.0f);

        SetPlayerInputEnable(true);

        while(lives > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);

        ShowCursor(true);
        GameUI.ShowRetryButton(true);
    }
}
