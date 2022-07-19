using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasicEventListener : GameEventListener
{
    public UnityEvent<GameObject> response;

    public override void OnEventRaised(GameObject subject)
    {
        response.Invoke(subject);
    }
}
