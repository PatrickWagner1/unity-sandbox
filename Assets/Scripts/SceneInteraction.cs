using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneInteraction : MonoBehaviour
{

    /// menu canvas
    private Canvas menuCanvas;

    /// menu group canvas
    private CanvasGroup menuCanvasGroup;

    /// start sandbox button in menu canvas
    private GameObject startSandboxButton;

    /// close menu button in menu canvas
    private Button closeMenuButton;

    /// options canvas
    private Canvas optionsCanvas;

    /// <summary>
    /// Initialize the menu and option objects.
    /// </summary>
    void Start()
    {
        GameObject menuObject = GameObject.Find("Menu");
        this.menuCanvas = menuObject.GetComponent<Canvas>();
        this.menuCanvasGroup = menuObject.GetComponent<CanvasGroup>();

        this.optionsCanvas = GameObject.Find("Options").GetComponent<Canvas>();

        this.startSandboxButton = GameObject.Find("StartSandboxButton");

        this.closeMenuButton = GameObject.Find("CloseMenuButton").GetComponent<Button>();

        this.closeMenuButton.enabled = false;
        this.optionsCanvas.enabled = false;
    }

    /// <summary>
    /// Update method for close operation.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && this.closeMenuButton.enabled)
        {
            this.closeMainMenu();
        }
    }

    /// <summary>
    /// Loads the main menu.
    /// </summary>
    public void openMainMenu()
    {
        this.menuCanvasGroup.blocksRaycasts = true;

        Slider seedSlider = GameObject.Find("SeedSlider").GetComponent<Slider>();
        seedSlider.value = TerrainObject.seed;

        Slider roughSlider = GameObject.Find("RoughnessSlider").GetComponent<Slider>();
        roughSlider.value = TerrainObject.rough;

        Toggle showContourLinesToggle = GameObject.Find("ShowContourLinesToggleMenu")
            .GetComponent<Toggle>();

        showContourLinesToggle.isOn = TerrainObject.showContourLines;

        this.closeMenuButton.enabled = true;
        this.startSandboxButton.SetActive(true);

        this.menuCanvas.enabled = true;
        this.optionsCanvas.enabled = false;
    }

    /// <summary>
    /// Closes the main menu.
    /// </summary>
    public void closeMainMenu()
    {
        Image menuTerrainImage = GameObject.Find("MenuTerrainImage").GetComponent<Image>();
        menuTerrainImage.enabled = false;

        Toggle showContourLinesToggle = GameObject.Find("ShowContourLinesToggleOptions")
            .GetComponent<Toggle>();

        showContourLinesToggle.isOn = TerrainObject.showContourLines;

        this.menuCanvas.enabled = false;
        this.optionsCanvas.enabled = true;
    }

    /// <summary>
    /// Starts the sandbox asynchron.
    /// </summary>
    public void StartUnitySandobx()
    {
        StartCoroutine(this.loadSandbox());
    }

    /// <summary>
    /// Asynchron method for loading the sandbox.
    /// </summary>
    private IEnumerator loadSandbox()
    {
        this.menuCanvasGroup.blocksRaycasts = false;
        this.startSandboxButton.SetActive(false);

        TerrainObject terrain = GameObject.Find("Plane").GetComponent<TerrainObject>();
        yield return null;
        terrain.generateMeshHeights();

        this.closeMainMenu();
        yield return null;
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
