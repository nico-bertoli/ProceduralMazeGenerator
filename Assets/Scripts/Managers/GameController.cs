using System.Collections;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    #region ============================================================================================= Private Fields

    [Header("Settings")]
    [SerializeField] private float gameWallsSize = 0.04f;
    
    [field: Header("References")]
    [field:SerializeField] public Maze Maze { get; private set; }
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject exitObj;

    /// <summary>
    /// Used during maze preview
    /// </summary>
    [SerializeField] private TopDownCamera topDownCamera;
    [SerializeField] private GameObject gameObjects;
    [SerializeField] private GameObject mazeGenerationObjects;

    #endregion Private Fields
    #region ============================================================================================= Public Methods
    
    public void GenerateMaze(int nRows,int nColumns, bool showLiveGeneration, AbsMazeGenerator.eAlgorithms algorithm) {

        // Debug.Log($"generating maze, rows:{nRows}, columns:{nColumns}, algorithm:{algorithm} live generation:{showLiveGeneration} ");

        topDownCamera.CenterPosition(Vector3.zero, nRows, nColumns);
        topDownCamera.AdjustCameraSize(nRows, nColumns);

        StartCoroutine(Maze.Generate(nRows, nColumns, showLiveGeneration, algorithm));
    }
    
    public void Reset() {
        Maze.Reset();
        SetGameMode(false);
        UIManager.Instance.ShowSettingsPanel();
    }
    
    public void SetLiveGenerationSpeed(float speed) => Maze.SetLiveGenerationSpeed(speed);

    public void QuitGame() => Application.Quit();

    public void PlayMaze() => StartCoroutine(PlayMazeCor());

    #endregion Public Methods
    #region ============================================================================================ Private Methods
    
    private void Start() {
        gameObjects.SetActive(false);
        topDownCamera.gameObject.SetActive(true);
        exitObj.GetComponent<Exit>().OnExitReached += Reset;
    }

    private IEnumerator PlayMazeCor() {
        UIManager.Instance.ShowLoadingGamePanel();

        yield return StartCoroutine(Maze.SetWallsSize(gameWallsSize));

        SetGameMode(true);

        playerObj.transform.position = new Vector3(transform.position.x, playerObj.transform.position.y, transform.position.z);
        playerObj.transform.forward = -transform.forward;

        exitObj.transform.position = Maze.GetExitPosition();

        yield return StartCoroutine(Maze.EnableCooling(playerObj));
        UIManager.Instance.DisableLoadingPanel();
    }

    private void SetGameMode(bool active) {
        gameObjects.SetActive(active);
        mazeGenerationObjects.SetActive(active == false);
    }
    
    #endregion Private Methods
}
