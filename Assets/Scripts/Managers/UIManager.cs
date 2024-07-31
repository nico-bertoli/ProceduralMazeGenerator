using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AbsMazeGenerator;

public class UIManager : Singleton<UIManager>
{
    private MazeGenerationSettings mazeGenSettings => Settings.Instance.MazeGenerationSettings;
    
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
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Maze maze;
    
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

    public void SetLoadingPanelText(string text) => loadingText.text = text;

    public void DisableLoadingPanel() {
        loadingPanel.gameObject.SetActive(false);
        backButton.SetActive(true);
    }

    public void PlayMaze() =>SceneManager.Instance.PlayMaze();

    #endregion Public Methods
    #region ============================================================================================ Monobehaviour
    
    private void Start() {
       
        maze.OnMazeChunksGenerated += OnGridFinalMeshCreated;
        maze.OnMazeChunksGenerated += OnMazeGridChunksGenerated;
        maze.OnGenerationStarted += OnMazeGenerationStarted;

        loadingPanel.SetActive(false);
        
        liveGenToggle.isOn = isLiveGenerationActive = false;
    }
    
    private void Update() {
        RefreshGridPossibleSize();
        SendLiveGeneSpeedToGameController();
    }
    
    #endregion
    
    #region ============================================================================================ Private Methods
    
    private void ShowGenerationPanel() {
        
        if (isLiveGenerationActive)
            loadingText.text = "Loading maze";
        else
            loadingText.text = "Algorithm is working";

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

    private void RefreshGridPossibleSize()
    {
        int mazeMaxSideCells = GetCurrentMaxSideCells();

        float nColumnsRow = columnsSlider.value * (mazeMaxSideCells - mazeGenSettings.MinSideCells) + mazeGenSettings.MinSideCells;
        float nRowsRow = rowsSlider.value * (mazeMaxSideCells - mazeGenSettings.MinSideCells) + mazeGenSettings.MinSideCells;

        nColumns = (int)(Mathf.Round(nColumnsRow / 5f) * 5);
        nRows = (int)(Mathf.Round(nRowsRow / 5f) * 5);

        widthText.text = "Columns: " + nColumns;
        heightText.text = "Rows: " + nRows;
    }

    private int GetCurrentMaxSideCells()
    {
        if (isLiveGenerationActive == false)
        {
            switch (algorithm)
            {
                case eAlgorithms.DFSiterative:
                    return mazeGenSettings.NotLiveGenDFSMaxSideCells;
                case eAlgorithms.Willson:
                    return mazeGenSettings.NotLiveGenWilsonMaxSideCells;
                case eAlgorithms.Kruskal:
                    return mazeGenSettings.NotLiveGenKruskalMaxSideCells;
                default:
                    Debug.LogError($"current algorithm not recognized: {algorithm}");
                    return -1;
            }
        }
        else
        {
            return mazeGenSettings.LiveGenMaxSideCells;
        }
    }

    private void SendLiveGeneSpeedToGameController() {
        if (isLiveGenerationActive)
            SceneManager.Instance.SetLiveGenerationSpeed(genSpeedSlider.value * 100);
    }

    #endregion Private Methods
    #region ============================================================================================ Signals
    
    public void Signal_ToggleLiveGeneration() {
        isLiveGenerationActive = !isLiveGenerationActive;
        RefreshGridPossibleSize();
    }

    public void Signal_RefreshAlgorithm() {
        algorithm = (eAlgorithms)algorithmDropdown.value;
        RefreshGridPossibleSize();
    }
    
    public void Signal_StartGeneration() {
        playGameButton.SetActive(false);
        if (isLiveGenerationActive) {
            genSpeedSlider.gameObject.SetActive(true);
            genSpeedSlider.value = mazeGenSettings.LiveGenerationStartingSpeedSliderValue;
        }
        else {
            genSpeedSlider.gameObject.SetActive(false);
        }

        ShowGenerationPanel();
        SceneManager.Instance.GenerateMaze(nRows, nColumns, isLiveGenerationActive, algorithm);
    }
    
    #endregion Signals
}
