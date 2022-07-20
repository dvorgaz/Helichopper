using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MFDButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject buttonContainer;
    private Button[] buttons;
    public UnityEvent<int> onButtonPressed;

    private void Awake()
    {
        buttons = buttonContainer.GetComponentsInChildren<Button>();
        SetupButtonCallbacks();
    }

    private void SetupButtonCallbacks()
    {
        for(int i = 0; i < buttons.Length; ++i)
        {
            int idx = i + 1;
            buttons[i].onClick.AddListener(() => OnButtonPressed(idx));
        }
    }

    public void OnButtonPressed(int idx)
    {
        Debug.Log("OSB " + idx + " pressed");
        onButtonPressed.Invoke(idx);
    }
}
