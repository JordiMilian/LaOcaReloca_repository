using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;
using UnityEditor;
using System;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] string firstScene;
    [SerializeField] string testScene;
    [SerializeField] float fadeDuration = 1f;
     CanvasGroup canvasGroup;
    string currentScene = "";

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    void Start()
    {
        if(SceneManager.GetSceneAt(0) == SceneManager.GetSceneByName("LoadingScreen"))
        {
            LoadNewScene(firstScene);
        }
        else
        {
            currentScene = SceneManager.GetSceneAt(0).name;
        }
    }
    public void LoadNewScene(string newScene)
    {
        StartCoroutine(LoadSceneCoroutine(newScene));
    }
    IEnumerator LoadSceneCoroutine(string scene)
    {
        //fade in
        Tween fadeIn = canvasGroup.DOFade(1, fadeDuration);
        while (canvasGroup.alpha < 1)
        {
            yield return null;
        }

        //Descargar anterior
        if(currentScene != "")
        {
            Debug.Log("Unloading scene: " + currentScene);
            AsyncOperation unloadCurrent = SceneManager.UnloadSceneAsync(currentScene);
            while (!unloadCurrent.isDone)
            {
                yield return null;
            }
        }

        //Cargar la nueva
        {
            Debug.Log("Loading scene: " + scene);
            AsyncOperation currentLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            while (!currentLoad.isDone)
            {
                yield return null;
            }
            currentScene = scene;
        }

        //Fade out
        Tween fadeOut = canvasGroup.DOFade(0, fadeDuration);
        while (!fadeOut.IsComplete())
        {
            yield return null;
        }
    }

    
    [ContextMenu("Load Test Scene")]
    void LoadTestScene()
    {
        LoadNewScene(testScene);
    }

#if UNITY_EDITOR

    [InitializeOnLoadMethod]
    static void SubscribeToChangeState()
    {
        Debug.Log("InitizializeOnLoadMethod called");
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange change)
    {
        Debug.Log(change);
        if(change == PlayModeStateChange.EnteredPlayMode)
        {
            if(SceneManager.GetSceneByName("LoadingScreen").isLoaded == false)
            {
                SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Additive);
            }
        }
    }
#endif
}
