using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI
/// </summary>
public class UIManager : Singleton<UIManager>
{
    //======================================== fields

    private int nColumns;
    private int nRows;
    private bool liveGeneration;
    private AbsMazeGenerator.eAlgorithms algorithm;
    private int maxCells = 255;
    private const int minCells = 10;

    [Header("References")]
    [SerializeField] Slider columnsSlider;
    [SerializeField] TextMeshProUGUI widthText;
    [SerializeField] Slider rowsSlider;
    [SerializeField] TextMeshProUGUI heightText;
    [SerializeField] Toggle liveGenToggle;
    [SerializeField] TMP_Dropdown algorithmDropdown;
    [SerializeField] Slider genSpeedSlider;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject playGameButton;
    [SerializeField] GameObject backButton;
    [SerializeField] LoadingPanel loadingPanel;

    //======================================== methods

    /// <summary>
    /// Enables/disables live generation
    /// </summary>
    public void ToggleLiveGeneration() {
        liveGeneration = !liveGeneration;
        if (liveGeneration) maxCells = 80;
        else maxCells = 255;
    }

    /// <summary>
    /// Sets maze generation algorithm
    /// </summary>
    public void UpdateAlgorithm() {
        algorithm = (AbsMazeGenerator.eAlgorithms)algorithmDropdown.value;
    }

    /// <summary>
    /// Makes maze generation begin
    /// </summary>
    public void StartGeneration() {
        playGameButton.SetActive(false);
        if (liveGeneration) {
            genSpeedSlider.gameObject.SetActive(true);
            genSpeedSlider.value = 0;
        }
        else {
            genSpeedSlider.gameObject.SetActive(false);
        }

        ShowGenerationPanel();
        GameController.Instance.GenerateMaze(nRows, nColumns, liveGeneration, algorithm);
    }

    /// <summary>
    /// Shows maze generation settings panel
    /// </summary>
    public void ShowSettingsPanel() {
        gamePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Shows maze generation / overview panel (and enables loading panel)
    /// </summary>
    public void ShowGenerationPanel() {
        
        if (liveGeneration)
            loadingPanel.Text = "Loading maze";
        else
            loadingPanel.Text = "Algorithm is working";

        loadingPanel.gameObject.SetActive(true);
        settingsPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void ShowLoadingGamePanel() {
        genSpeedSlider.gameObject.SetActive(false);
        playGameButton.SetActive(false);
        backButton.SetActive(false);
        loadingPanel.gameObject.SetActive(true);
        loadingPanel.Text = "Loading";
    }

    public void DisableLoadingPanel() {
        loadingPanel.gameObject.SetActive(false);
        backButton.SetActive(true);
    }

    public void SetLoadingPanelText(string _text) {
        loadingPanel.Text = _text;
    }

    private void Start() {
        liveGenToggle.isOn = liveGeneration = false;
        GameController.Instance.GetMaze().OnGenerationComplete += () => {
            playGameButton.SetActive(true);
            loadingPanel.gameObject.SetActive(false);
            genSpeedSlider.gameObject.SetActive(false);
        };
    }

    private void Update() {
        RefreshGridSizeText();
        SendLiveGeneSpeedToGameController();
    }

    /// <summary>
    /// Refresh grid size text depending on sliders value
    /// </summary>
    private void RefreshGridSizeText() {
        nColumns = (int)(columnsSlider.value * (maxCells - minCells) + minCells);
        nRows = (int)(rowsSlider.value * (maxCells - minCells) + minCells);

        widthText.text = "Columns: " + nColumns;
        heightText.text = "Rows: " + nRows;
    }

    /// <summary>
    /// Sets game controller live gen speed depending on gen speed slider
    /// </summary>
    private void SendLiveGeneSpeedToGameController() {
        if (liveGeneration)
            GameController.Instance.SetLiveGenerationSpeed(genSpeedSlider.value * 100);
    }

    public void PlayMaze() {
        GameController.Instance.PlayMaze();
    }
}
