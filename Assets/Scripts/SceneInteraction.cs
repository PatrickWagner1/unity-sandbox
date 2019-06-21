using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInteraction : MonoBehaviour
{

    /// <summary>
    /// Loads the main menu scene.
    /// </summary>
    public void OpenMainMenu()
    {
        SceneManager.LoadScene("menu");
    }

    /// <summary>
    /// Sets the roughness for the terrain.
    /// </summary>
    /// <param name="rough">roughness</param>
    public void OnRoughChanged(float rough)
    {
        TerrainObject.rough = rough;
    }

    /// <summary>
    /// Sets the seed for the terrain.
    /// </summary>
    /// <param name="seed">seed</param>
    public void OnSeedChanged(float seed)
    {
        TerrainObject.seed = (int)seed;
    }

    /// <summary>
    /// Sets status of showContourLines in the terrain object to true or false
    /// and sets a global shader int variable representing the status.
    /// If @showContourLines is true global shader variable will be set to 42.
    /// If @showContourLines is false global shader variable will be set to 0.
    /// </summary>
    /// <param name="showContourLines">
    /// True if contour lines should be presented, otherwise false
    /// </param>
    public static void changeContourLines(bool showContourLines)
    {
        TerrainObject.showContourLines = showContourLines;
        int showContourLinesInteger = 0;
        if (showContourLines)
        {
            showContourLinesInteger = 42;
        }
        Shader.SetGlobalInt("_SHOW_CONTOUR_LINES", showContourLinesInteger);
    }

    /// <summary>
    /// Corresponds the register of changeContourLines() as an event listener
    /// </summary>
    /// <param name="showContourLines">
    /// True, if contour lines should be presented, otherwise false.
    /// </param>
    public void OnContourLinesChanged(bool showContourLines)
    {
        SceneInteraction.changeContourLines(showContourLines);
    }
}
