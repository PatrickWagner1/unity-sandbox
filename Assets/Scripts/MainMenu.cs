using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    /// The loading circle object, when loading the scene.
    private GameObject loadingCircle;

    /// <summary>
    /// Sets the UI elements in the menu.
    /// </summary>
    public void Start()
    {
        loadingCircle = GameObject.Find("LoadingCircle");
        loadingCircle.SetActive(false);

        Slider seedSlider = GameObject.Find("SeedSlider").GetComponent<Slider>();
        seedSlider.value = TerrainObject.seed;

        Slider roughSlider = GameObject.Find("RoughnessSlider").GetComponent<Slider>();
        roughSlider.value = TerrainObject.rough;

        Toggle showContourLinesToggle = GameObject.Find("ShowContourLinesToggle")
        .GetComponent<Toggle>();

        showContourLinesToggle.isOn = TerrainObject.showContourLines;
    }

    /// <summary>
    /// Starts loading the unity sandbox asynchronous.
    /// </summary>    
    public void StartUnitySandobx()
    {
        StartCoroutine(LoadUnitySandbox());
    }

    /// <summary>
    /// Loads the sandbox scene asynchronous and disable to change options in the menu.
    /// </summary>
    /// <returns>Null until loading of scene is completed</returns>
    public IEnumerator LoadUnitySandbox()
    {
        AsyncOperation loadSandbox = SceneManager.LoadSceneAsync("unity-sandbox");
        loadSandbox.allowSceneActivation = false;
        CanvasGroup canvasGroup = GameObject.Find("Canvas").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        GameObject.Find("StartSandboxButton").SetActive(false);
        loadingCircle.SetActive(true);
        loadSandbox.allowSceneActivation = true;

        while (!loadSandbox.isDone)
        {
            //Output the current progress


            yield return null;
        }
    }
}
