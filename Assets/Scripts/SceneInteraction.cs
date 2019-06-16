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

    public void OnContourLinesChanged(bool showContourLines)
    {
        DiamondSquareGenerator.setShowContourLines(showContourLines);
    }
}
