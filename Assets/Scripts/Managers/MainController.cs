using System;
using JetBrains.Annotations;
using UnityEngine;

public class MainController:Singleton<MainController>
{
    public event Action OnPlayModeActivated;
    
    #region ============================================================================================= Fields
    
    [field: Header("References")]
    [field:SerializeField] public Maze Maze { get; private set; }
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject exitObj;

    [SerializeField] private TopDownCamera mazeGenerationCamera;
    [SerializeField] private GameObject gameObjectsContainer;
    [SerializeField] private GameObject mazeGenerationObjectsContainer;

    #endregion Fields
    #region ============================================================================================= Public Methods
    
    public void GenerateMaze(int nRows,int nColumns, bool showLiveGeneration, AbsMazeGenerator.eAlgorithms algorithm) {

        Vector3 mazeTopLeftPosition = new Vector3(-0.5f, 0f, 0.5f);
        mazeGenerationCamera.LookAtRectangularObject(mazeTopLeftPosition, nRows, nColumns);
        Maze.Generate(nRows, nColumns, showLiveGeneration, algorithm);

    }

    public void SetLiveGenerationSpeed(float speed) => Maze.SetLiveGenerationSpeed(speed);

    public void PlayMaze() {
        
        UIManager.Instance.ShowLoadingGamePanel();
        OnPlayModeActivated?.Invoke();
        
        SetGameMode(true);
        SetupPlayerPosition();
        SetupExitPosition();
        
        UIManager.Instance.DisableLoadingPanel();
    }

    #endregion Public Methods
    #region ============================================================================================ Private Methods

    private void SetupPlayerPosition()
    {
        Vector3 mazeCentralPos = Maze.GetCentralCellPosition();
        playerObj.transform.position = new Vector3(mazeCentralPos.x, playerObj.transform.position.y,mazeCentralPos.z);
        playerObj.transform.forward = Vector3.forward;
    }
    
    private void SetupExitPosition() => exitObj.transform.position = Maze.GetExitPosition();
    
    private void Start() {
        gameObjectsContainer.SetActive(false);
        mazeGenerationCamera.gameObject.SetActive(true);
        exitObj.GetComponent<TriggerDetector>().OnTriggerEnterCalled += Signal_Reset;
    }

    private void SetGameMode(bool active) {
        gameObjectsContainer.SetActive(active);
        mazeGenerationObjectsContainer.SetActive(active == false);
    }
    
    #endregion Private Methods
    #region ============================================================================================ Private Methods
    
    [UsedImplicitly]
    public void Signal_Reset() {
        Maze.Reset();
        SetGameMode(false);
        UIManager.Instance.ShowSettingsPanel();
    }
    
    [UsedImplicitly]
    public void Signal_QuitGame() => Application.Quit();
    
    #endregion Signals
}
