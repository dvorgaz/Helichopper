using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public RectTransform Crosshair { get; private set; }

    [SerializeField] private HeliStatusDisplay heliStatus;
    [SerializeField] private GameObject rearmPanel;
    [SerializeField] private GameObject retryButton;
    private WeaponController weaponController;
    private Camera mainCamera;

    private void Awake()
    {
        Transform tr = transform.Find("Crosshair");
        if(tr != null)
        {
            Crosshair = tr.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("Missing Crosshair UI");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;        
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
    }

    public void ShowRetryButton(bool visible)
    {
        retryButton.SetActive(visible);
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
