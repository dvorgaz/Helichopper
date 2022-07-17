using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeliStatusDisplay : MonoBehaviour
{
    private Health health;
    private WeaponController weaponController;
    private HeliController heliController;

    public enum UIElement
    {
        Armor = 0,
        Fuel,
        Weapon1,
        Weapon2,
        Weapon3,
        Flares,
        Num
    }

    [SerializeField]
    private TextMeshProUGUI[] texts;

    // Start is called before the first frame update
    void Start()
    {
        if (texts == null || texts.Length != (int)UIElement.Num)
        {
            Debug.LogError("Invalid number of text UI references");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health != null && weaponController != null && heliController != null)
        {
            SetUIText(UIElement.Armor, health.Hp);
            SetUIText(UIElement.Fuel, Mathf.RoundToInt(heliController.Fuel));
            SetUIText(UIElement.Flares, weaponController.Flares);

            for(int i = 0; i < 3; ++i)
            {
                WeaponLauncher wpn = weaponController.GetWeapon(i);
                if (wpn == null)
                    break;

                SetUIText(UIElement.Weapon1 + i, wpn.ShotsLeft, wpn.displayName.ToUpper());
            }
        }
    }

    public bool SetObject(GameObject heli)
    {
        if (heli != null)
        {
            Health h = heli.GetComponent<Health>();
            WeaponController wc = heli.GetComponent<WeaponController>();
            HeliController hc = heli.GetComponent<HeliController>();

            if (h != null && wc != null && hc != null)
            {
                health = h;
                weaponController = wc;
                heliController = hc;

                return true;
            }
        }

        return false;
    }

    public void SetUIText(UIElement element, int amount, string label = null)
    {
        texts[(int)element].text = amount.ToString();

        if(label != null)
        {
            Transform child = texts[(int)element].transform.Find("Label");
            if (child != null)
            {
                TextMeshProUGUI labelText = child.GetComponent<TextMeshProUGUI>();
                if (labelText != null)
                {
                    labelText.text = label;
                }
            }
        }
    }
}
