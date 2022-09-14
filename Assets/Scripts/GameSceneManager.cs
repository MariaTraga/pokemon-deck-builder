using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    [SerializeField] private GameObject loadingCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); 
            return;
        }
        DontDestroyOnLoad(gameObject);

        
    }

    public async void LoadScene(string sceneName)
    {
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;
        loadingCanvas.SetActive(!loadingCanvas.activeInHierarchy);

        do{
            await Task.Delay(100);
        } while (scene.progress < 0.9f);
        scene.allowSceneActivation = true;
        await Task.Delay(200); 
        loadingCanvas.SetActive(!loadingCanvas.activeInHierarchy);        
    }
}
