using System;
using System.Collections;
using UnityEngine;
using static AbsMazeGenerator;

public class Maze : MonoBehaviour {
    
    #region ============================================================================================== Public Events
    
    public Action OnGenerationComplete;
    public Action OnGridInitComplete;
    
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
    
    public IEnumerator Generate(int nRows, int nColumns, bool showLiveGeneration, AbsMazeGenerator.eAlgorithms algorithm) {

        isLiveGenerationActive = showLiveGeneration;

        yield return StartCoroutine(InstantiateGridObj(nRows,nColumns));

        InstantiateMazeGenerator(algorithm);
        
        yield return StartCoroutine(mazeGenerator.GenerateMaze(dataGrid, dataGrid.GetCell(0, 0), showLiveGeneration));

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
        yield return StartCoroutine(gridObj.SetWallsWithd(size));
        yield return StartCoroutine(gridObj.CombineMeshes());
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
            gridObj.OnInitCompleted += OnGenerationComplete;
            UIManager.Instance.SetLoadingPanelText("Loading maze");
            StartCoroutine(gridObj.Init(dataGrid));
        }

        else {
            StartCoroutine(gridObj.CombineMeshes());
            OnGenerationComplete();
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

    private IEnumerator InstantiateGridObj(int _nRows,int _nColumns) {
        if (isLiveGenerationActive)
            gridObj = Instantiate(liveGenerationGridPrototype).GetComponent<LiveGenerationGrid>();
        else
            gridObj = Instantiate(notLiveGenerationGridPrototype).GetComponent<NotLiveGenerationGrid>();

        dataGrid = new DataGrid(_nRows, _nColumns);

        gridObj.OnInitCompleted += OnGridInitComplete;

        if (isLiveGenerationActive)
            yield return StartCoroutine(gridObj.Init(dataGrid));
    }
    
    #endregion Private Methods
}
