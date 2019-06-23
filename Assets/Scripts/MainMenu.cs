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
        SceneInteraction.changeContourLines(TerrainObject.showContourLines);

        this.loadingCircle = GameObject.Find("LoadingCircle");
        this.loadingCircle.SetActive(false);

        Slider seedSlider = GameObject.Find("SeedSlider").GetComponent<Slider>();
        seedSlider.value = TerrainObject.seed;

        Slider roughSlider = GameObject.Find("RoughnessSlider").GetComponent<Slider>();
        roughSlider.value = TerrainObject.rough;

        Toggle showContourLinesToggle = GameObject.Find("ShowContourLinesToggleMenu")
        .GetComponent<Toggle>();

        showContourLinesToggle.isOn = TerrainObject.showContourLines;
    }
}
