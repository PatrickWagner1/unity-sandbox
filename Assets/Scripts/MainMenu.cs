using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    
    public void Start()
    {
        Slider seedSlider = GameObject.Find("SeedSlider").GetComponent<Slider>();
        float seed = TerrainObject.seed;
        if (seed < 100)
        {
            seed = Random.value * 400.0f + 100.0f;
        }
        seedSlider.value = seed;

        Slider roughSlider = GameObject.Find("RoughnessSlider").GetComponent<Slider>();
        roughSlider.value = TerrainObject.rough;

        Toggle showContourLinesToggle = GameObject.Find("ShowContourLinesToggle").GetComponent<Toggle>();
        showContourLinesToggle.isOn = TerrainObject.showContourLines;
    }

    public void StartUnitySandobx()
    {
        StartCoroutine(LoadUnitySandbox());
    }

    public IEnumerator LoadUnitySandbox()
    {
        AsyncOperation loadSandbox = SceneManager.LoadSceneAsync("unity-sandbox");
        loadSandbox.allowSceneActivation = false;
        CanvasGroup canvasGroup = GameObject.Find("Canvas").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        GameObject.Find("StartSandboxButton").SetActive(false);
        Text loadText = GameObject.Find("LoadText").GetComponent<Text>();
        loadSandbox.allowSceneActivation = true;

        while (!loadSandbox.isDone)
        {
            //Output the current progress
            loadText.text = "Loading progress: " + (loadSandbox.progress * 100) + "%";

            yield return null;
        }
    }
}
