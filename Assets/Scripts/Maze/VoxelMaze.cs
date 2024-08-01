using System;
using System.Collections;
using UnityEngine;
using static AbsMazeGenStrategy;

public class VoxelMaze : MonoBehaviour {

    protected virtual bool IsLiveGenerationEnabled => false;

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

    public event Action OnGenerationStarted;

    #endregion Public Events
    #region ============================================================================================= Fields

    //--- protected
    [SerializeField] protected VoxelGenerator voxelGenerator;
    protected DataGrid dataGrid;

    //--- private
    protected AbsMazeGenStrategy mazeGenStrategy;
    #endregion Fields
    #region ============================================================================================= Public Methods
    
    public Vector3 GetCentralCellPosition() 
    {
        var centralCell = dataGrid.GetCentralCell();
        return new Vector3(centralCell.PosN, 0, -centralCell.PosM);
    }

    public void Generate(int nRows, int nColumns, bool showLiveGeneration, MazeGenStrategy eMazeGenStategy)
    {
        StartCoroutine(GenerateMazeCor(nRows,nColumns,eMazeGenStategy));
    }

    public virtual void Reset() => voxelGenerator.Reset();

    public Vector3 GetExitPosition() => dataGrid.GetExitPosition();

    #endregion Public Methods
    #region ============================================================================================ Hooks
    protected virtual DataGrid Hook_CreateDataGrid(int nRows, int nColumns) => new DataGrid(nRows, nColumns);
    protected virtual void Hook_GenerationCompleted() => voxelGenerator.CreateGrid(dataGrid);

    #endregion Hooks
    #region ============================================================================================ Private Methods

    protected virtual void Start() => voxelGenerator.OnMeshGenerated += CallMeshGeneratedEvent;

    private void CallMeshGeneratedEvent()
    {
        if (enabled == false)
            return;
        OnVoxelMeshGenerated?.Invoke();
    }

    private IEnumerator GenerateMazeCor(int nRows, int nColumns, MazeGenStrategy eStrategy)
    {

        yield return null;

        dataGrid = Hook_CreateDataGrid(nRows,nColumns);

        mazeGenStrategy = GetStrategyFromEnum(eStrategy);

        OnGenerationStarted?.Invoke();
        
        DataCell startCell = dataGrid.GetCentralCell();

        yield return Coroutiner.Instance.StartCoroutine(mazeGenStrategy.GenerateMaze(dataGrid, startCell, IsLiveGenerationEnabled, Coroutiner.Instance));

        OnMazeDataStructureGenerated?.Invoke();



        Hook_GenerationCompleted();
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
    #endregion Private Methods
}