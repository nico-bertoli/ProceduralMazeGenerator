using System;
using UnityEngine;
using static AbsMazeGenStrategy;

public class LiveGenMaze : VoxelMaze
{
    protected override bool IsLiveGenerationEnabled => true;
    public event Action OnLiveGenerationMeshGenerated;

    [SerializeField] private LiveGenerationGrid liveGenGrid;

    public void SetLiveGenerationSpeed(float speed) => mazeGenStrategy?.SetLiveGenerationSpeed(speed);

    protected override void Start()
    {
        base.Start();
        SceneManager.Instance.OnEscapeMazePhaseStarted += OnEscapeMazePhaseStarted;
    }

    public override void Reset()
    {
        base.Reset();
        liveGenGrid.Reset();
    }

    private void OnEscapeMazePhaseStarted()
    {
        liveGenGrid.Reset();
        voxelGenerator.CreateGrid(dataGrid);
    }

    protected override void TemplateGenerateMaze_GenerationCompleted() => OnLiveGenerationMeshGenerated?.Invoke();

    protected override DataGrid TemplateGenerateMaze_GetDataGrid(int nRows, int nColumns)
    {
        DataGrid dataGrid = base.TemplateGenerateMaze_GetDataGrid(nRows,nColumns);
        liveGenGrid.Init(dataGrid);
        return dataGrid;
    }
}
