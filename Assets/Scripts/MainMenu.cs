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
        if (seed == 0)
        {
            seed = Random.value * 400.0f + 100.0f;
        }
        seedSlider.value = seed;

        Slider roughSlider = GameObject.Find("RoughnessSlider").GetComponent<Slider>();
        roughSlider.value = DiamondSquareGenerator.rough;

        Toggle showContourLinesToggle = GameObject.Find("ShowContourLinesToggle").GetComponent<Toggle>();
        showContourLinesToggle.isOn = DiamondSquareGenerator.showContourLines;
    }


}
