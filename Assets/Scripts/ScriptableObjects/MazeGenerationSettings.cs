using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObjects/MazeGenerationSettings",fileName = "MazeGenerationSettings")]
public class MazeGenerationSettings : ScriptableObject
{
    [field:Header("Maze Size")]
    
    [field:SerializeField] public int LiveGenMaxSideCells { get; private set; } = 40;
    [field:SerializeField] public int NotLiveGenMaxSideCells { get; private set; } = 400;

    [field: SerializeField] public int MinSideCells { get; private set; } = 10;


    [field:Header("Live Generation")]
    [field: SerializeField] public float LiveGenerationMaxDelay { get; private set; } = 0.4f;
    
    [field:Header("Voxel Generation")]
    [field: SerializeField] public int VoxelChunkSize { get; private set; } = 20;
    
    
    [field:Header("Walls Width")]
    [field: SerializeField] public float LiveGenerationWallsWidth { get; private set; } = 0.4f;
    [field: SerializeField] public float IngameWallsWidth { get; private set; } = 0.4f;

}
