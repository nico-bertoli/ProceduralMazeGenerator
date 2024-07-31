using System;
using System.Collections;
using UnityEngine;
using static AbsMazeGenStrategy;

public class MazeFacade : MonoBehaviour {

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

    [SerializeField] private LiveGenerationGrid liveGenGrid;
    [SerializeField] private VoxelGenerator voxelGenerator;

    private DataGrid dataGrid;
    private AbsMazeGenStrategy mazeGenStrategy;
    
    #endregion Fields
    #region ============================================================================================= Public Methods
    
    public Vector3 GetCentralCellPosition() 
    {
        var centralCell = dataGrid.GetCentralCell();
        return new Vector3(centralCell.PosN, 0, -centralCell.PosM);
    }

    public void Generate(int nRows, int nColumns, bool showLiveGeneration, eMazeGenStrategy eMazeGenStategy)
    {
        StartCoroutine(GenerateCor(nRows,nColumns,showLiveGeneration,eMazeGenStategy));
    }
    
    public void SetLiveGenerationSpeed(float speed) => mazeGenStrategy?.SetLiveGenerationSpeed(speed);

    public void Reset()
    {    
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
    
    private IEnumerator GenerateCor(int nRows, int nColumns, bool showLiveGeneration, eMazeGenStrategy eStrategy)
    {
        yield return null;
        
        IsLiveGenerationActive = showLiveGeneration;

        dataGrid = new DataGrid(nRows, nColumns);

        if (IsLiveGenerationActive)
            liveGenGrid.Init(dataGrid);

        mazeGenStrategy = GetStrategyFromEnum(eStrategy);

        OnGenerationStarted?.Invoke();
        
        DataCell startCell = dataGrid.GetCentralCell();

        yield return Coroutiner.Instance.StartCoroutine(mazeGenStrategy.GenerateMaze(dataGrid, startCell, showLiveGeneration, Coroutiner.Instance));

        OnMazeDataStructureGenerated?.Invoke();

        if (IsLiveGenerationActive == false)
            voxelGenerator.CreateGrid(dataGrid);
        else
            OnLiveGenerationMeshGenerated?.Invoke();
    }

    private AbsMazeGenStrategy GetStrategyFromEnum(eMazeGenStrategy enumStrategy)
    {
        switch (enumStrategy)
        {
            case eMazeGenStrategy.DFSiterative:
                return new RandDfsIterMazeGenStrategy();
            case eMazeGenStrategy.Willson:
                return new WillsonMazeGenStrategy();
            case eMazeGenStrategy.Kruskal:
                return new KruskalMazeGenStrategy();

            default:
                Debug.LogError($"Maze generation strategy enum value not recognized: {enumStrategy}");
                return null;
        }
    }

    private void OnEscapeMazePhaseStarted()
    {
        if (IsLiveGenerationActive)
        {
            liveGenGrid.Reset();
            voxelGenerator.CreateGrid(dataGrid);
        }
    }

    #endregion Private Methods
}
