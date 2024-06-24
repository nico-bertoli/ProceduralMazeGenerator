using System;
using System.Collections;
using UnityEngine;
using static AbsMazeGenerator;

public class Maze : MonoBehaviour {
    
    #region ============================================================================================== Public Events
    
    public Action OnMazeChunksGenerated;
    public Action OnGenerationStarted;
    public Action OnGenerationEnded;
    
    #endregion Public Events
    #region ========================================================================================== Public Properties
    
    public bool IsLiveGenerationActive { get; private set; }
    
    #endregion Public Properties
    #region ============================================================================================= Private Fields

    [SerializeField] private LiveGenerationGrid liveGenGrid;
    [SerializeField] private VoxelGenerator voxelGenerator;

    private DataGrid dataGrid;
    private AbsMazeGenerator mazeGenerator;
    
    #endregion Fields
    #region ============================================================================================= Public Methods

    public Vector3 GetCentralCellPosition() 
    {
        var centralCell = dataGrid.GetCentralCell();
        //for how maze is setup z component must be negative
        return new Vector3(centralCell.PosN, 0, -centralCell.PosM);
    }
    
    public IEnumerator Generate(int nRows, int nColumns, bool showLiveGeneration, eAlgorithms algorithm) {

        IsLiveGenerationActive = showLiveGeneration;

        dataGrid = new DataGrid(nRows, nColumns);

        if (IsLiveGenerationActive)
            liveGenGrid.Init(dataGrid);

        InstantiateMazeGenerator(algorithm);
        OnGenerationStarted?.Invoke();
        DataCell startCell = dataGrid.GetCentralCell();
        yield return StartCoroutine(mazeGenerator.GenerateMaze(dataGrid, startCell, showLiveGeneration));
        OnGenerationEnded?.Invoke();
        
        ShowGrid();
    }

    public void SetLiveGenerationSpeed(float speed) {
        if (mazeGenerator)
            mazeGenerator.SetLiveGenerationSpeed(speed);
    }

    public void Reset() {
        if (mazeGenerator != null)
            Destroy(mazeGenerator.gameObject);
        
        liveGenGrid.Reset();
        voxelGenerator.Reset();
    }

    public Vector3 GetExitPosition()
    {
        return dataGrid.GetExitPosition();
    }

    private bool isCullingEnabled = false;
    
    
    #endregion Public Methods
    
    #region ============================================================================================ Private Methods

    private void ShowGrid() {
        if (!IsLiveGenerationActive) {
            voxelGenerator.OnMeshGenerated += OnMazeChunksGenerated;
            
            UIManager.Instance.SetLoadingPanelText("Loading maze");
            // StartCoroutine(gridObj.Init(dataGrid));
            voxelGenerator.CreateGrid(dataGrid);
        }
        else
        {
            OnMazeChunksGenerated?.Invoke();
        }
    }

    private void InstantiateMazeGenerator(eAlgorithms algorithm) {
        switch (algorithm) {
            case eAlgorithms.DFSiterative:
                mazeGenerator = new GameObject().AddComponent<RandDfsIterMazeGenerator>();
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

    #endregion Private Methods
}
