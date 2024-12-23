using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AbsMazeGenStrategy;

public class UIManager : Singleton<UIManager>
{
    private MazeSizeSettings mazeGenSettings => Settings.Instance.MazeSettings;
    
    #region ============================================================================================= Private Fields

    [Header("UI References")]
    [SerializeField] private Slider columnsSlider;
    [SerializeField] private Slider rowsSlider;
    [SerializeField] private Slider genSpeedSlider;
    [Space]
    [SerializeField] private TextMeshProUGUI widthText;
    [SerializeField] private TextMeshProUGUI heightText;
    [Space]
    [SerializeField] private GameObject btnStartEscape;
    [SerializeField] private GameObject btnBack;
    [Space]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject loadingPanel;
    [Space]
    [SerializeField] private Toggle liveGenToggle;
    [SerializeField] private TMP_Dropdown algorithmDropdown;

    [Header("Other References")]
    [SerializeField] private VoxelMaze voxelMaze;
    [SerializeField] private LiveGenMaze liveGenMaze;

    private int nColumns;
    private int nRows;
    private bool isLiveGenerationActive;
    private MazeGenStrategy mazeGenStrategy;

    #endregion Private Fields
    #region ============================================================================================= Public Methods
    
    public void ShowSettingsPanel()
    {
        gamePanel.SetActive(false);
        settingsPanel.SetActive(true);
        btnStartEscape.gameObject.SetActive(false);
        genSpeedSlider.gameObject.SetActive(false);
    }

    #endregion Public Methods
    #region ============================================================================================ Private Methods
    private void Start()
    {
        voxelMaze.OnMazeDataStructureGenerated += () => loadingPanel.SetActive(false);
        voxelMaze.OnVoxelMeshGenerated += OnMazeFinalMeshGenerated;

        liveGenMaze.OnMazeDataStructureGenerated += () => loadingPanel.SetActive(false);
        liveGenMaze.OnVoxelMeshGenerated += OnMazeFinalMeshGenerated;
        liveGenMaze.OnLiveGenerationMeshGenerated += OnLiveGenerationMeshGenerated;
        liveGenMaze.OnGenerationStarted += () => Signal_UpdateLiveGenerationSpeed();

        loadingPanel.SetActive(false);
        liveGenToggle.isOn = false;
        isLiveGenerationActive = false;

        ShowSettingsPanel();
    }

    private void Update()
    {
        RefreshGridPossibleSize();
    }

    private void ShowGenerationPanel()
    {
        if(isLiveGenerationActive==false)
            loadingPanel.SetActive(true);

        settingsPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    private void OnMazeFinalMeshGenerated()
    {
        if(isLiveGenerationActive == false)
            btnStartEscape.SetActive(true);
    }

    private void OnLiveGenerationMeshGenerated()
    {
        btnStartEscape.SetActive(true);
        genSpeedSlider.gameObject.SetActive(false);
    }

    private void RefreshGridPossibleSize()
    {
        int mazeMaxSideCells = GetCurrentAlgorithmMaxSideCells();

        float nColumnsRow = columnsSlider.value * (mazeMaxSideCells - mazeGenSettings.MinSideCells) + mazeGenSettings.MinSideCells;
        float nRowsRow = rowsSlider.value * (mazeMaxSideCells - mazeGenSettings.MinSideCells) + mazeGenSettings.MinSideCells;

        nColumns = (int)(Mathf.Round(nColumnsRow / 5f) * 5);
        nRows = (int)(Mathf.Round(nRowsRow / 5f) * 5);

        widthText.text = "Columns: " + nColumns;
        heightText.text = "Rows: " + nRows;
    }

    private int GetCurrentAlgorithmMaxSideCells()
    {
        if (isLiveGenerationActive == false)
        {
            switch (mazeGenStrategy)
            {
                case MazeGenStrategy.DFSiterative:
                    return mazeGenSettings.VoxelGenMaxSideCellsDFS;
                case MazeGenStrategy.Willson:
                    return mazeGenSettings.VoxelGenMaxSideCellsWillson;
                case MazeGenStrategy.Kruskal:
                    return mazeGenSettings.VoxelGenMaxSideCellsKruskal;
                default:
                    Debug.LogError($"current algorithm not recognized: {mazeGenStrategy}");
                    return -1;
            }
        }
        else
            return Settings.Instance.MazeSettings.LiveGenMaxSideCells;
    }

    #endregion Private Methods
    #region ============================================================================================ Signals

    [UsedImplicitly]
    public void Signal_UpdateLiveGenerationSpeed() => liveGenMaze.SetLiveGenerationSpeed(genSpeedSlider.value * 100);

    [UsedImplicitly]
    public void Signal_StartEscapePhase()
    {
        btnStartEscape.SetActive(false);
        SceneManager.Instance.PlayMaze();
    }

    [UsedImplicitly]
    public void Signal_ToggleLiveGeneration()
    {
        isLiveGenerationActive = !isLiveGenerationActive;
        RefreshGridPossibleSize();
    }

    [UsedImplicitly]
    public void Signal_RefreshAlgorithm()
    {
        mazeGenStrategy = (MazeGenStrategy)algorithmDropdown.value;
        RefreshGridPossibleSize();
    }

    [UsedImplicitly]
    public void Signal_StartGeneration()
    {
        if (isLiveGenerationActive)
        {
            genSpeedSlider.gameObject.SetActive(true);
            genSpeedSlider.value = Settings.Instance.LiveGenSettings.DefaultSpeedSliderValue;
        }

        ShowGenerationPanel();
        SceneManager.Instance.ShowMazeGeneration(nRows, nColumns, isLiveGenerationActive, mazeGenStrategy);
    }

    [UsedImplicitly]
    public void Signal_QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quitting application");
#endif
        Application.Quit();
    }

    [UsedImplicitly]
    public void Signal_ShowGenerationSettings()
    {
        ShowSettingsPanel();
        SceneManager.Instance.ResetScene();
    }

    #endregion Signals
}
