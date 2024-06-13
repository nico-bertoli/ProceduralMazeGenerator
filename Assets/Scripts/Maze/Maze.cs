using System;
using System.Collections;
using UnityEngine;
using static AbsMazeGenerator;

public class Maze : MonoBehaviour {
    
    #region ============================================================================================== Public Events
    
    public Action OnMazeChunksGenerated;
    
    public Action OnGenerationStarted;
    public Action OnGenerationEnded;
    #endregion
    #region ============================================================================================= Private Fields

    [SerializeField] private GameObject liveGenerationGridPrototype;
    [SerializeField] private GameObject notLiveGenerationGridPrototype;

    private AbsGridObj gridObj;
    private DataGrid dataGrid;
    private AbsMazeGenerator mazeGenerator;
    
    private bool isLiveGenerationActive;
    
    #endregion Fields
    #region ============================================================================================= Public Methods
    
    public IEnumerator Generate(int nRows, int nColumns, bool showLiveGeneration, eAlgorithms algorithm) {

        isLiveGenerationActive = showLiveGeneration;

        yield return StartCoroutine(InstantiateGridObj(nRows,nColumns));

        InstantiateMazeGenerator(algorithm);
        
        OnGenerationStarted?.Invoke();
        yield return StartCoroutine(mazeGenerator.GenerateMaze(dataGrid, dataGrid.GetCell(0, 0), showLiveGeneration));
        OnGenerationEnded?.Invoke();
        
        ShowGrid();
    }

    public void SetLiveGenerationSpeed(float speed) {
        if (mazeGenerator)
            mazeGenerator.SetLiveGenerationSpeed(speed);
    }

    public void Reset() {
        if (mazeGenerator) Destroy(mazeGenerator.gameObject);
        if (gridObj) Destroy(gridObj.gameObject);
    }

    public IEnumerator SetWallsSize(float size) {
        yield return StartCoroutine(gridObj.SetWallsWidth(size));
        yield return StartCoroutine(gridObj.GenerateChunks());
    }

    public Vector3 GetExitPosition() {
        return gridObj.GetBottomRightCellPos();
    }

    public IEnumerator EnableCooling(GameObject coolingObject) {
        yield return StartCoroutine(gridObj.EnableCulling(coolingObject));
    }
    
    #endregion Public Methods
    
    #region ============================================================================================ Private Methods

    private void ShowGrid() {
        if (!isLiveGenerationActive) {
            // gridObj.OnGridFinalMeshCreated += OnGridFinalMeshCreated;
            UIManager.Instance.SetLoadingPanelText("Loading maze");
            StartCoroutine(gridObj.Init(dataGrid));
        }
        else {
            StartCoroutine(gridObj.GenerateChunks());
            // OnGridFinalMeshCreated?.Invoke();
        }
    }

    private void InstantiateMazeGenerator(eAlgorithms algorithm) {
        switch (algorithm) {
            case eAlgorithms.DFSiterative:
                mazeGenerator = new GameObject().AddComponent<DFSIterMazeGenerator>();
                mazeGenerator.gameObject.name = "DFSIter Maze Generator";
                break;
            case eAlgorithms.Willson:
                mazeGenerator = new GameObject().AddComponent<WilsonMazeGenerator>();
                mazeGenerator.gameObject.name = "Wilson Maze Generator";
                break;
            case eAlgorithms.Kruskal:
                mazeGenerator = new GameObject().AddComponent<KruskalMazeGenerator>();
                mazeGenerator.gameObject.name = "Kruskal Maze Generator";
                break;
            default:
                Debug.LogError($"{algorithm} not recognized! {nameof(InstantiateMazeGenerator)} failed");
                break;
        }
    }

    private IEnumerator InstantiateGridObj(int nRows,int nColumns){
        if (isLiveGenerationActive)
            gridObj = Instantiate(liveGenerationGridPrototype).GetComponent<LiveGenerationGrid>();
        else
            gridObj = Instantiate(notLiveGenerationGridPrototype).GetComponent<NotLiveGenerationGrid>();

        gridObj.OnGridChunksGenerated += OnMazeChunksGenerated;

        dataGrid = new DataGrid(nRows, nColumns);

        if (isLiveGenerationActive)
            yield return StartCoroutine(gridObj.Init(dataGrid));
    }

    #endregion Private Methods
}
