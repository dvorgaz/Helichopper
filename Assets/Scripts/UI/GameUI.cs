using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public RectTransform Crosshair { get; private set; }

    public enum UIElement
    {
        Armor = 0,
        Fuel,
        Gun,
        Rocket,
        Missile,
        Flares,
        Num
    }

    [SerializeField]
    private TextMeshProUGUI[] texts;

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

        if(texts == null || texts.Length != (int)UIElement.Num)
        {
            Debug.LogError("Invalid number of text UI references");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUIText(UIElement element, int amount)
    {
        texts[(int)element].text = amount.ToString();
    }
}
