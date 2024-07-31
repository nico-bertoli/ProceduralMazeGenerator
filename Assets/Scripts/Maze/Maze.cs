using System;
using System.Collections;
using UnityEngine;
using static AbsMazeGenerator;

public class Maze : MonoBehaviour {

    #region ============================================================================================== Public Events

    public event Action OnLiveGenerationMeshGenerated;
    public event Action OnMazeFinalMeshGenerated;
    public event Action OnGenerationStarted;
    public event Action OnMazeDataStructureGenerated;
    
    #endregion Public Events
    #region ========================================================================================== Public Properties
    
    public bool IsLiveGenerationActive { get; private set; }
    
    #endregion Public Properties
    #region ============================================================================================= Private Fields

    /// <summary>
    /// Used for live generation
    /// </summary>
    [SerializeField] private LiveGenerationGrid liveGenGrid;
    
    /// <summary>
    /// Used for not live generation
    /// </summary>
    [SerializeField] private VoxelGenerator voxelGenerator;

    private DataGrid dataGrid;
    private AbsMazeGenerator mazeGenerator;
    
    #endregion Fields
    #region ============================================================================================= Public Methods
    
    public Vector3 GetCentralCellPosition() 
    {
        var centralCell = dataGrid.GetCentralCell();
        return new Vector3(centralCell.PosN, 0, -centralCell.PosM);
    }

    public void Generate(int nRows, int nColumns, bool showLiveGeneration, eAlgorithms algorithm)
    {
        StartCoroutine(GenerateCor(nRows,nColumns,showLiveGeneration,algorithm));
    }
    
    public void SetLiveGenerationSpeed(float speed)
    {
        if (mazeGenerator)
            mazeGenerator.SetLiveGenerationSpeed(speed);
    }

    public void Reset()
    {
        if (mazeGenerator != null)
            Destroy(mazeGenerator.gameObject);
        
        liveGenGrid.Reset();
        voxelGenerator.Reset();
    }

    public Vector3 GetExitPosition()
    {
        return dataGrid.GetExitPosition();
    }
    
    
    #endregion Public Methods
    
    #region ============================================================================================ Private Methods
    
    private void Start()
    {
        voxelGenerator.OnMeshGenerated += OnMazeFinalMeshGenerated;
        SceneManager.Instance.OnEscapeMazePhaseStarted += OnEscapeMazePhaseStarted;
    }
    
    private IEnumerator GenerateCor(int nRows, int nColumns, bool showLiveGeneration, eAlgorithms algorithm)
    {
        yield return null;
        
        IsLiveGenerationActive = showLiveGeneration;

        dataGrid = new DataGrid(nRows, nColumns);

        if (IsLiveGenerationActive)
            liveGenGrid.Init(dataGrid);

        mazeGenerator = CreateMazeGenerator(algorithm);
        OnGenerationStarted?.Invoke();
        
        DataCell startCell = dataGrid.GetCentralCell();
        yield return StartCoroutine(mazeGenerator.GenerateMaze(dataGrid, startCell, showLiveGeneration));
        OnMazeDataStructureGenerated?.Invoke();
        ShowGrid();
    }

    private void OnEscapeMazePhaseStarted()
    {
        if (IsLiveGenerationActive)
        {
            liveGenGrid.Reset();
            voxelGenerator.CreateGrid(dataGrid);
        }
    }
    
    private void ShowGrid() {
        if (IsLiveGenerationActive == false)
            voxelGenerator.CreateGrid(dataGrid);
        else
            OnLiveGenerationMeshGenerated?.Invoke();
    }

    private AbsMazeGenerator CreateMazeGenerator(eAlgorithms algorithm)
    {
        AbsMazeGenerator res;
        switch (algorithm)
        {
            case eAlgorithms.DFSiterative:
                res = new GameObject().AddComponent<RandDfsIterMazeGenerator>();
                res.gameObject.name = "DFSIter Maze Generator";
                break;
            case eAlgorithms.Willson:
                res = new GameObject().AddComponent<WilsonMazeGenerator>();
                res.gameObject.name = "Wilson Maze Generator";
                break;
            case eAlgorithms.Kruskal:
                res = new GameObject().AddComponent<KruskalMazeGenerator>();
                res.gameObject.name = "Kruskal Maze Generator";
                break;
            default:
                Debug.LogError($"{algorithm} not recognized! {nameof(CreateMazeGenerator)} failed");
                return null;
        }
        return res;
    }

    #endregion Private Methods
}
