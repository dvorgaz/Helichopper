using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TextMeshProUGUI))]
public class NotificationText : MonoBehaviour
{
    public string notificationText;
    public float transitionTime;
    public float visibleDuration;
    public AnimationCurve animCurve;
    private UnityAction onFinishedAction = null;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ShowNotification(string text, UnityAction onFinished = null)
    {
        notificationText = text;
        onFinishedAction = onFinished;
        StartCoroutine(AnimateCoroutine());
    }

    private IEnumerator AnimateCoroutine()
    {
        RectTransform transform = GetComponent<RectTransform>();
        TextMeshProUGUI guiText = GetComponent<TextMeshProUGUI>();
        guiText.text = notificationText;

        float width = transform.sizeDelta.x;
        Vector3 origPos = transform.position;
        Vector3 outPos = new Vector3(origPos.x - width, origPos.y, origPos.z);

        for (float t = 0; t < transitionTime; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(outPos, origPos, animCurve.Evaluate(t / transitionTime));
            yield return null;
        }

        yield return new WaitForSeconds(visibleDuration);

        for (float t = 0; t < transitionTime; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(outPos, origPos, animCurve.Evaluate(1.0f - t / transitionTime));
            yield return null;
        }

        if (onFinishedAction != null)
            onFinishedAction.Invoke();

        Destroy(gameObject);
    }
}
