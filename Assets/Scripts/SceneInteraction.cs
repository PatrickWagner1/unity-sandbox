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
}
