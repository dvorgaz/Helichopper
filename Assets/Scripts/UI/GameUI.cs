using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{    
    [SerializeField] private HeliStatusDisplay heliStatus;
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private RectTransform hudContainer;
    [SerializeField] private GameObject rearmPanel;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private GameObject menuPrefab;
    private GameObject menu;
    private WeaponController weaponController;
    private Camera mainCamera;

    public RectTransform Crosshair { get { return crosshair; } }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        menu = Instantiate(menuPrefab, hudContainer.parent);
        menu.SetActive(false);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(weaponController == null || weaponController.gameObject.activeInHierarchy == false)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                heliStatus.SetObject(playerObj);
                weaponController = playerObj.GetComponent<WeaponController>();
            }
        }

        if (weaponController != null)
        {
            Crosshair.gameObject.SetActive(weaponController.ShowCrosshair);
            Crosshair.position = mainCamera.WorldToScreenPoint(weaponController.TargetPoint);
        }
    }

    public void ShowRearmPanel(bool visible)
    {
        rearmPanel.SetActive(visible);

        if (visible)
        {
            menu.SetActive(false);
        }
    }

    public void ShowIngameMenu(bool visible)
    {
        menu.SetActive(visible);

        if(visible)
        {
            rearmPanel.SetActive(false);
        }
    }

    public bool IsIngameMenuOpen()
    {
        return menu.activeInHierarchy;
    }

    public void ShowRetryButton(bool visible)
    {
        retryButton.SetActive(visible);

        if (visible)
        {
            menu.SetActive(false);
            rearmPanel.SetActive(false);
        }
    }

    public void OnRearmClose()
    {
        GameController.Instance.ShowRearmMenu(false);
    }

    public void OnRetry()
    {
        GameController.Instance.Retry();
    }
}
