using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScreenFader : MonoBehaviour
{
    public bool playOnStart;
    public float signedDuration;
    public UnityEvent onFadeInCompleted;
    public UnityEvent onFadeOutCompleted;

    private UnityAction fadeCompletedAction = null;

    // Start is called before the first frame update
    void Start()
    {
        if (playOnStart)
            Fade(signedDuration);
    }

    public void Fade(float signedDuration)
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeCoroutine(signedDuration));
    }

    public void Fade(float signedDuration, UnityAction action)
    {
        fadeCompletedAction = action;
        Fade(signedDuration);
    }

    private IEnumerator FadeCoroutine(float signedDuration)
    {
        float dir = Mathf.Sign(signedDuration);
        float duration = Mathf.Abs(signedDuration);
        Image image = GetComponent<Image>();

        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            float param = t / duration;
            Color col = image.color;
            col.a = dir > 0 ? param : 1 - param;
            image.color = col;

            yield return null;
        }

        if (dir < 0)
        {
            gameObject.SetActive(false);
            onFadeOutCompleted.Invoke();
        }
        else
        {
            onFadeInCompleted.Invoke();
        }

        if (fadeCompletedAction != null)
        {
            fadeCompletedAction.Invoke();
            fadeCompletedAction = null;
        }
    }
}
