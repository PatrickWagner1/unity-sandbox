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
        float seed = DiamondSquareGenerator.seed;
        if (seed < 100)
        {
            seed = Random.value * 400.0f + 100.0f;
        }
        seedSlider.value = seed;

        Slider roughSlider = GameObject.Find("RoughnessSlider").GetComponent<Slider>();
        roughSlider.value = DiamondSquareGenerator.rough;

        Toggle showContourLinesToggle = GameObject.Find("ShowContourLinesToggle").GetComponent<Toggle>();
        showContourLinesToggle.isOn = DiamondSquareGenerator.showContourLines;
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

        while (!loadSandbox.isDone)
        {
            //Output the current progress
            loadText.text = "Loading progress: " + (loadSandbox.progress * 100) + "%";

            if (loadSandbox.progress >= 0.9f)
            {
                //Change the Text to show the Scene is ready
                loadText.text = "Press the space bar to continue";
                //Wait to you press the space key to activate the Scene
                if (Input.GetKeyDown(KeyCode.Space))
                    //Activate the Scene
                    loadSandbox.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
