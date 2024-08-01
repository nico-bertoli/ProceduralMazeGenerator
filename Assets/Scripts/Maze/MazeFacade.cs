using System;
using System.Collections;
using UnityEngine;
using static AbsMazeGenStrategy;

public class MazeFacade : MonoBehaviour {

    #region ============================================================================================== Public Events

    /// <summary>
    /// - Hidden generation mode -> called at the end of maze generation
    /// - Live generation mode -> called when play button is pressed
    /// </summary>
    public event Action OnVoxelMeshGenerated;

    /// <summary>
    /// Called when maze structure has been defined by the maze gen strategy, but the final mesh is not already created
    /// </summary>
    public event Action OnMazeDataStructureGenerated;

    public event Action OnLiveGenerationMeshGenerated;
    public event Action OnGenerationStarted;

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

    public void Generate(int nRows, int nColumns, bool showLiveGeneration, MazeGenStrategy eMazeGenStategy)
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
        voxelGenerator.OnMeshGenerated += OnVoxelMeshGenerated;
        SceneManager.Instance.OnEscapeMazePhaseStarted += OnEscapeMazePhaseStarted;
    }
    
    private IEnumerator GenerateCor(int nRows, int nColumns, bool showLiveGeneration, MazeGenStrategy eStrategy)
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

    private AbsMazeGenStrategy GetStrategyFromEnum(MazeGenStrategy enumStrategy)
    {
        switch (enumStrategy)
        {
            case MazeGenStrategy.DFSiterative:
                return new RandDfsIterMazeGenStrategy();
            case MazeGenStrategy.Willson:
                return new WillsonMazeGenStrategy();
            case MazeGenStrategy.Kruskal:
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