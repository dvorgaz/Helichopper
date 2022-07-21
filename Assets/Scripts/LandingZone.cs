using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingZone : MonoBehaviour
{
    public enum Type
    {
        HomeBase,
        Other
    }

    public Type type = Type.Other;
}
