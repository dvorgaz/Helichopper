using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    public UnityEvent<GameObject> gameEvent;

    public void Raise(GameObject subject)
    {
        gameEvent.Invoke(subject);
        Debug.Log(name + " event raised");
    }

    public void AddListener(UnityAction<GameObject> call)
    {
        gameEvent.AddListener(call);
    }

    public void RemoveListener(UnityAction<GameObject> call)
    {
        gameEvent.RemoveListener(call);
    }
}
