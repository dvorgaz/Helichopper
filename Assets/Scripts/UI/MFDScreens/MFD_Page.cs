using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MFD_Page : MonoBehaviour
{
    public abstract void ProcessButton(int idx);
    public abstract void Display();
}
