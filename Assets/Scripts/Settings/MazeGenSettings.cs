using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MazeGenSettings", fileName = "MazeGenSettings")]
public class MazeGenSettings : ScriptableObject
{
    [field:Header("Maze Size")]

    [field: SerializeField] public int MinSideCells { get; private set; } = 5;
    [field:SerializeField] public int LiveGenMaxSideCells { get; private set; } = 40;
    [field:SerializeField] public int VoxelGenDFSMaxSideCells { get; private set; } = 2000;
    [field: SerializeField] public int VoxelGenKruskalMaxSideCells { get; private set; } = 400;
    [field: SerializeField] public int VoxelGenWilsonMaxSideCells { get; private set; } = 200;


    [field:Header("Walls Size")]
    [field: SerializeField] public float LiveGenerationWallsWidth { get; private set; } = 0.4f;
    [field: SerializeField] public float InGameWallsWidth { get; private set; } = 0.4f;
    [field: SerializeField] public float WallsHeight { get; private set; } = 0.5f;
    
    
    [field:Header("Live Generation")]
    [field: SerializeField] public float LiveGenMaxDelay { get; private set; } = 0.4f;
    [field: SerializeField] public float LiveGenSpeedSliderValue { get; private set; } = 0.95f;

    [field: Header("Hidden Generation")]
    [field: SerializeField] public float VoxelGenMaxTimeWithoutRefreshingScreen { get; private set; } = 0.3f;

    [field:Header("Voxel Generation")]
    [field: SerializeField] public int VoxelChunkSize { get; private set; } = 20;
}
