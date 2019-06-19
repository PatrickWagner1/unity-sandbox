using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInteraction : MonoBehaviour
{

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("menu");
    }

    /// <summary>
    /// Sets the roughness and recalculate the mesh heights.
    /// </summary>
    /// <param name="rough">roughness</param>
    public void OnRoughChanged(float rough)
    {
        TerrainObject.rough = rough;
    }

    /// <summary>
    /// Sets the seed and recalculate the mesh heights.
    /// </summary>
    /// <param name="seed">seed</param>
    public void OnSeedChanged(float seed)
    {
        TerrainObject.seed = (int)seed;
    }

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

    public void OnContourLinesChanged(bool showContourLines)
    {
        SceneInteraction.changeContourLines(showContourLines);
    }
}
