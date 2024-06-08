using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    //======================================== fields

    [Header("Settings")]
    [SerializeField] float gameWallsSize = 0.04f;

    [Header("References")]
    [SerializeField] Maze maze;
    [SerializeField] GameObject playerObj;
    [SerializeField] GameObject exitObj;
    /// <summary>
    /// Camera used in the genration / maze overview phase
    /// </summary>
    [SerializeField] TopDownCamera topDownCamera;
    /// <summary>
    /// Container of play-mode objects
    /// </summary>
    [SerializeField] GameObject gameObjectsContainer;
    /// <summary>
    /// Container of creation objects
    /// </summary>
    [SerializeField] GameObject creationObjectsContainer;

    //======================================== methods
    /// <summary>
    /// Starts maze generation
    /// </summary>
    /// <param name="_nRows">Maze number of rows</param>
    /// <param name="_nColumns">Maze number of columns</param>
    /// <param name="_showGeneration">If true, show how algorithm works generating the maze slowly</param>
    /// <param name="_algorithm">Maze generation algorithm to use</param>
    /// <returns></returns>
    /// 
    public void GenerateMaze(int _nRows,int _nColumns, bool _showGeneration, AbsMazeGenerator.eAlgorithms _algorithm) {

        Debug.Log("generate maze called with rows:" + _nRows + ", columns: " + _nColumns + ", algorithm:" + _algorithm + " live generation: " + _showGeneration);

        topDownCamera.AdjustCameraPosition(Vector3.zero, _nRows, _nColumns);
        topDownCamera.AdjustCameraSize(_nRows, _nColumns);

        StartCoroutine(maze.Generate(_nRows, _nColumns, _showGeneration, _algorithm));
    }
    
    public void SetLiveGenerationSpeed(float _speed) {
        maze.SetLiveGenerationSpeed(_speed);
    }

    /// <summary>
    /// Resets the generation
    /// </summary>
    public void Reset() {
        maze.Reset();
        setGameMode(false);
        UIManager.Instance.ShowSettingsPanel();
    }

    public void QuitGame() {
        Application.Quit();
    }

    /// <summary>
    /// Starts game mode
    /// </summary>
    public void PlayMaze() {
        StartCoroutine(playMazeCor());
    }

    private void Start() {
        maze.OnGridInitComplete += UIManager.Instance.DisableLoadingPanel;
        gameObjectsContainer.SetActive(false);
        topDownCamera.gameObject.SetActive(true);
        exitObj.GetComponent<Exit>().OnExitReached += Reset;
    }

    private IEnumerator playMazeCor() {
        UIManager.Instance.ShowLoadingGamePanel();

        yield return StartCoroutine(maze.SetWallsSize(gameWallsSize));

        setGameMode(true);

        playerObj.transform.position = new Vector3(transform.position.x, playerObj.transform.position.y, transform.position.z);
        playerObj.transform.forward = -transform.forward;

        exitObj.transform.position = maze.GetExitPosition();

        yield return StartCoroutine(maze.EnableCooling(playerObj));
        UIManager.Instance.DisableLoadingPanel();
    }

    private void setGameMode(bool _active) {
        gameObjectsContainer.SetActive(_active);
        creationObjectsContainer.SetActive(!_active);
    }

    public Maze GetMaze() { return maze; }
}
