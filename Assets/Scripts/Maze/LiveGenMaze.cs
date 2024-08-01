using System;
using UnityEngine;

public class LiveGenMaze : VoxelMaze
{
    protected override bool IsLiveGenerationEnabled => true;
    public event Action OnLiveGenerationMeshGenerated;

    [SerializeField] private LiveGenerationGrid liveGenGrid;

    public void SetLiveGenerationSpeed(float speed) => mazeGenStrategy?.SetLiveGenerationSpeed(speed);

    public override void Reset()
    {
        base.Reset();
        liveGenGrid.Reset();
    }

    private void OnEscapeMazePhaseStarted()
    {
        liveGenGrid.Reset();
        voxelGenerator.CreateGrid(dataGrid);
        SceneManager.Instance.OnEscapeMazePhaseStarted -= OnEscapeMazePhaseStarted;
    }

    protected override void Hook_GenerationCompleted() => OnLiveGenerationMeshGenerated?.Invoke();

    protected override DataGrid Hook_CreateDataGrid(int nRows, int nColumns)
    {
        DataGrid dataGrid = base.Hook_CreateDataGrid(nRows,nColumns);
        liveGenGrid.Init(dataGrid);
        return dataGrid;
    }

    protected override void Hook_GenerationStarted()
    {
        base.Hook_GenerationStarted();
        SceneManager.Instance.OnEscapeMazePhaseStarted += OnEscapeMazePhaseStarted;
    }
}
