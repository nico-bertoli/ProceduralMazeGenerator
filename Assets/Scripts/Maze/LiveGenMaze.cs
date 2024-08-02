using System;
using UnityEngine;

public class LiveGenMaze : VoxelMaze
{
    public event Action OnLiveGenerationMeshGenerated;

    protected override bool IsLiveGenerationEnabled => true;

    [SerializeField] private LiveGenerationGrid liveGenGrid;

    #region ============================================================================================== Public Methods

    public void SetLiveGenerationSpeed(float speed) => mazeGenStrategy?.SetLiveGenerationSpeed(speed);
    public override void Reset()
    {
        base.Reset();
        liveGenGrid.Reset();
        SceneManager.Instance.OnEscapeMazePhaseStarted -= OnEscapeMazePhaseStarted;
    }

    #endregion Public Methods
    #region ============================================================================================== Hooks Override
    protected override void Hook_GenerationEndPhase() => OnLiveGenerationMeshGenerated?.Invoke();

    protected override DataGrid Hook_CreateDataGrid(int nRows, int nColumns)
    {
        DataGrid dataGrid = base.Hook_CreateDataGrid(nRows, nColumns);
        liveGenGrid.Init(dataGrid);
        return dataGrid;
    }
    #endregion Hooks Override
    #region ============================================================================================== Private Methods

    private void Start()
    {
        OnMazeDataStructureGenerated += () => SceneManager.Instance.OnEscapeMazePhaseStarted += OnEscapeMazePhaseStarted;
    }

    private void OnEscapeMazePhaseStarted()
    {
        liveGenGrid.Reset();
        GenerateVoxel();
    }

    #endregion Private Methods
}
