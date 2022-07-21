using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScene : MonoBehaviour
{
    public ScreenFader screenFader;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InputCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator InputCoroutine()
    {
        yield return new WaitForSeconds(2);

        while(true)
        {
            if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape))
            {
                break;
            }

            yield return null;
        }

        screenFader.Fade(1, () => SceneManager.LoadScene("MainMenu"));
    }
}
