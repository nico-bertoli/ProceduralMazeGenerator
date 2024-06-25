using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static AbsMazeGenerator;

public class UIManager : Singleton<UIManager>
{
    private MazeGenerationSettings mazeGenSettings => Settings.Instance.mazeGenerationSettings;
    
    #region ============================================================================================= Private Fields

    [Header("References")]
    [SerializeField] private Slider columnsSlider;
    [SerializeField] private TextMeshProUGUI widthText;
    [SerializeField] private Slider rowsSlider;
    [SerializeField] private TextMeshProUGUI heightText;
    [SerializeField] private Toggle liveGenToggle;
    [SerializeField] private TMP_Dropdown algorithmDropdown;
    [SerializeField] private Slider genSpeedSlider;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject playGameButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private LoadingText loadingText;
    
    private int nColumns;
    private int nRows;
    private bool isLiveGenerationActive;
    private eAlgorithms algorithm;

    #endregion Private Fields
    #region ============================================================================================= Public Methods
    
    public void ShowSettingsPanel() {
        gamePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    
    public void ShowLoadingGamePanel() {
        genSpeedSlider.gameObject.SetActive(false);
        playGameButton.SetActive(false);
        backButton.SetActive(false);
        loadingPanel.SetActive(true);
        SetLoadingPanelText("Loading");
    }

    public void SetLoadingPanelText(string text) => loadingText.Text = text;

    public void DisableLoadingPanel() {
        loadingPanel.gameObject.SetActive(false);
        backButton.SetActive(true);
    }

    public void PlayMaze() =>MainController.Instance.PlayMaze();

    #endregion Public Methods
    #region ============================================================================================ Monobehaviour
    
    private void Start() {
       
        MainController.Instance.Maze.OnMazeChunksGenerated += OnGridFinalMeshCreated;
        
        MainController.Instance.Maze.OnMazeChunksGenerated += OnMazeGridChunksGenerated;
        MainController.Instance.Maze.OnGenerationStarted += OnMazeGenerationStarted;

        loadingPanel.SetActive(false);
        
        liveGenToggle.isOn = isLiveGenerationActive = false;
    }
    
    private void Update() {
        RefreshGridSizeText();
        SendLiveGeneSpeedToGameController();
    }
    
    #endregion
    
    #region ============================================================================================ Private Methods
    
    private void ShowGenerationPanel() {
        
        if (isLiveGenerationActive)
            loadingText.Text = "Loading maze";
        else
            loadingText.Text = "Algorithm is working";

        loadingPanel.gameObject.SetActive(true);
        settingsPanel.SetActive(false);
        gamePanel.SetActive(true);
    }
    
    private void OnMazeGridChunksGenerated()
    {
        if(isLiveGenerationActive == false)
            DisableLoadingPanel();
    }

    private void OnMazeGenerationStarted()
    {
        if(isLiveGenerationActive)
            DisableLoadingPanel();
    }

    private void OnGridFinalMeshCreated()
    {
        playGameButton.SetActive(true);
        loadingPanel.gameObject.SetActive(false);
        genSpeedSlider.gameObject.SetActive(false);
    }

    private void RefreshGridSizeText()
    {
        if (isLiveGenerationActive)
        {
            nColumns = (int)(columnsSlider.value * (mazeGenSettings.LiveGenMaxSideCells - mazeGenSettings.MinSideCells) + mazeGenSettings.MinSideCells);
            nRows = (int)(rowsSlider.value * (mazeGenSettings.LiveGenMaxSideCells - mazeGenSettings.MinSideCells) + mazeGenSettings.MinSideCells);
        }
        else
        {
            nColumns = (int)(columnsSlider.value * (mazeGenSettings.NotLiveGenMaxSideCells - mazeGenSettings.MinSideCells) + mazeGenSettings.MinSideCells);
            nRows = (int)(rowsSlider.value * (mazeGenSettings.NotLiveGenMaxSideCells - mazeGenSettings.MinSideCells) + mazeGenSettings.MinSideCells);
        }

        widthText.text = "Columns: " + nColumns;
        heightText.text = "Rows: " + nRows;
    }

    private void SendLiveGeneSpeedToGameController() {
        if (isLiveGenerationActive)
            MainController.Instance.SetLiveGenerationSpeed(genSpeedSlider.value * 100);
    }

    #endregion Private Methods
    #region ============================================================================================ Signals
    
    public void Signal_ToggleLiveGeneration() {
        isLiveGenerationActive = !isLiveGenerationActive;
    }

    public void Signal_RefreshAlgorithm() {
        algorithm = (eAlgorithms)algorithmDropdown.value;
    }
    
    public void Signal_StartGeneration() {
        playGameButton.SetActive(false);
        if (isLiveGenerationActive) {
            genSpeedSlider.gameObject.SetActive(true);
            genSpeedSlider.value = Settings.Instance.mazeGenerationSettings.LiveGenerationStartingSpeedSliderValue;
        }
        else {
            genSpeedSlider.gameObject.SetActive(false);
        }

        ShowGenerationPanel();
        MainController.Instance.GenerateMaze(nRows, nColumns, isLiveGenerationActive, algorithm);
    }
    
    #endregion Signals
}
