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
    }

    #endregion Public Methods
    #region ============================================================================================== Hooks Override
    protected override void Hook_GenerationCompleted() => OnLiveGenerationMeshGenerated?.Invoke();

    protected override DataGrid Hook_CreateDataGrid(int nRows, int nColumns)
    {
        DataGrid dataGrid = base.Hook_CreateDataGrid(nRows, nColumns);
        liveGenGrid.Init(dataGrid);
        return dataGrid;
    }
    #endregion Hooks Override
    #region ============================================================================================== Private Methods

    protected override void Start()
    {
        base.Start();
        SceneManager.Instance.OnEscapeMazePhaseStarted += OnEscapeMazePhaseStarted;
    }


    private void OnEscapeMazePhaseStarted()
    {
        if (enabled == false)
            return;

        liveGenGrid.Reset();
        voxelGenerator.CreateGrid(dataGrid);
    }

    #endregion Private Methods
}
