using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AbsMazeGenerator;

public class UIManager : Singleton<UIManager>
{
    #region ============================================================================================= Private Fields

    [Header("Settings")]
    [SerializeField] private int liveGenerationMaxCells = 80;
    [SerializeField] private int notLiveGenerationMaxCells = 260;
    
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
    [SerializeField] private LoadingPanel loadingPanel;
    
    private int nColumns;
    private int nRows;
    private bool liveGeneration;
    private eAlgorithms algorithm;
    private int maxCells = 255;
    private const int minCells = 10;
    
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
        loadingPanel.gameObject.SetActive(true);
        loadingPanel.Text = "Loading";
    }

    public void DisableLoadingPanel() {
        loadingPanel.gameObject.SetActive(false);
        backButton.SetActive(true);
    }

    public void SetLoadingPanelText(string text) {
        loadingPanel.Text = text;
    }
    
    public void PlayMaze() =>GameController.Instance.PlayMaze();

    #endregion Public Methods
    #region ============================================================================================ Monobehaviour
    
    private void Start() {
       
        GameController.Instance.Maze.OnMazeChunksGenerated += OnGridFinalMeshCreated;
        
        GameController.Instance.Maze.OnMazeChunksGenerated += OnMazeGridChunksGenerated;
        GameController.Instance.Maze.OnGenerationStarted += OnMazeGenerationStarted;
        
        liveGenToggle.isOn = liveGeneration = false;
    }
    
    private void Update() {
        RefreshGridSizeText();
        SendLiveGeneSpeedToGameController();
    }
    
    #endregion
    
    #region ============================================================================================ Private Methods
    
    private void ShowGenerationPanel() {
        
        if (liveGeneration)
            loadingPanel.Text = "Loading maze";
        else
            loadingPanel.Text = "Algorithm is working";

        loadingPanel.gameObject.SetActive(true);
        settingsPanel.SetActive(false);
        gamePanel.SetActive(true);
    }
    
    private void OnMazeGridChunksGenerated()
    {
        if(liveGeneration == false)
            DisableLoadingPanel();
    }

    private void OnMazeGenerationStarted()
    {
        if(liveGeneration)
            DisableLoadingPanel();
    }

    private void OnGridFinalMeshCreated()
    {
        playGameButton.SetActive(true);
        loadingPanel.gameObject.SetActive(false);
        genSpeedSlider.gameObject.SetActive(false);
    }

    private void RefreshGridSizeText() {
        nColumns = (int)(columnsSlider.value * (maxCells - minCells) + minCells);
        nRows = (int)(rowsSlider.value * (maxCells - minCells) + minCells);

        widthText.text = "Columns: " + nColumns;
        heightText.text = "Rows: " + nRows;
    }

    private void SendLiveGeneSpeedToGameController() {
        if (liveGeneration)
            GameController.Instance.SetLiveGenerationSpeed(genSpeedSlider.value * 100);
    }

    #endregion Private Methods
    #region ============================================================================================ Signals
    
    public void Signal_ToggleLiveGeneration() {
        liveGeneration = !liveGeneration;
        maxCells = liveGeneration ? liveGenerationMaxCells : notLiveGenerationMaxCells;
    }
    
    public void Signal_RefreshAlgorithm() {
        algorithm = (eAlgorithms)algorithmDropdown.value;
    }
    
    public void Signal_StartGeneration() {
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
    
    #endregion Signals
}
