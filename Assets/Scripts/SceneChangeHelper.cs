using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeHelper : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        Debug.Log("clicked");
        GameSceneManager.Instance.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
