using System;
using JetBrains.Annotations;
using UnityEngine;

public class SceneManager:Singleton<SceneManager>
{
    public event Action OnPlayModeActivated;
    private enum GamePhase
    {
        MazeGeneration,
        EscapeMaze
    }

    #region ============================================================================================= Private Fields

    [Header("References")]
    [SerializeField] private Maze Maze;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject exitObj;

    [SerializeField] private TopDownCamera mazeGenerationCamera;
    [SerializeField] private GameObject escapePhaseObjectsContainer;
    [SerializeField] private GameObject generationPhaseObjectsContainer;

    #endregion Private Fields
    #region ============================================================================================= Public Methods
    
    public void ShowMazeGeneration(int nRows,int nColumns, bool showLiveGeneration, AbsMazeGenerator.eAlgorithms algorithm) {
        Vector3 mazeTopLeftPosition = new Vector3(-0.5f, 0f, 0.5f);
        mazeGenerationCamera.LookAtRectangularObject(mazeTopLeftPosition, nRows, nColumns);
        Maze.Generate(nRows, nColumns, showLiveGeneration, algorithm);
    }

    public void PlayMaze() {
        
        UIManager.Instance.ShowLoadingGamePanel();
        OnPlayModeActivated?.Invoke();
        
        EnableObjects(GamePhase.EscapeMaze);
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
        escapePhaseObjectsContainer.SetActive(false);
        mazeGenerationCamera.gameObject.SetActive(true);
        exitObj.GetComponent<TriggerDetector>().OnTriggerEnterCalled += Signal_Reset;
    }

    private void EnableObjects(GamePhase gamePhase) {
        escapePhaseObjectsContainer.SetActive(gamePhase == GamePhase.EscapeMaze);
        generationPhaseObjectsContainer.SetActive(gamePhase == GamePhase.MazeGeneration);
    }
    
    #endregion Private Methods
    #region ============================================================================================ Private Methods
    
    [UsedImplicitly]
    public void Signal_Reset() {
        Maze.Reset();
        EnableObjects(GamePhase.MazeGeneration);
        UIManager.Instance.ShowSettingsPanel();
    }
    
    #endregion Signals
}
