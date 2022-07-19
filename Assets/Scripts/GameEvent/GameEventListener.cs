using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;

    private void OnEnable()
    {
        gameEvent.AddListener(this);
    }

    private void OnDisable()
    {
        gameEvent.RemoveListener(this);
    }

    public abstract void OnEventRaised(GameObject subject);
}
