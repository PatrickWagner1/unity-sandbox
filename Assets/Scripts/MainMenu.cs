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
        if (seed == 0) {
            seed = Random.value * 400.0f + 100.0f;
        }
        seedSlider.value = seed;

        Slider roughSlider = GameObject.Find("RoughnessSlider").GetComponent<Slider>();
        roughSlider.value = DiamondSquareGenerator.rough;
    }

    public void LoadUnitySandobx()
    {
        SceneManager.LoadScene("unity-sandbox");
    }

    /// <summary>
    /// Sets the roughness and recalculate the mesh heights.
    /// </summary>
    /// <param name="rough">roughness</param>
    public void OnRoughChanged(float rough)
    {
        DiamondSquareGenerator.rough = rough;
    }

    /// <summary>
    /// Sets the seed and recalculate the mesh heights.
    /// </summary>
    /// <param name="seed">seed</param>
    public void OnSeedChanged(float seed)
    {
        DiamondSquareGenerator.seed = (int)seed;
    }
}
