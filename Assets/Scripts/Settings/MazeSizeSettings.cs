using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MazeSizeSettings", fileName = "MazeSizeSettings")]
public class MazeSizeSettings : ScriptableObject
{
    [field:Header("Walls")]
    [field: SerializeField] public float WallsHeight { get; private set; } = 0.5f;
    [field: SerializeField] public float VoxelWallsWidth { get; private set; } = 0.1f;
    [field: SerializeField] public float LiveGenWallsWidth { get; private set; } = 0.4f;

    [field: Header("Size")]
    [field: SerializeField] public int MinSideCells { get; private set; } = 10;
    
    [field: Space]
    [field: SerializeField] public int LiveGenMaxSideCells { get; private set; } = 50;

    [field:Space]
    [field: SerializeField] public int VoxelGenMaxSideCellsDFS { get; private set; } = 2000;
    [field: SerializeField] public int VoxelGenMaxSideCellsKruskal { get; private set; } = 400;
    [field: SerializeField] public int VoxelGenMaxSideCellsWillson { get; private set; } = 200;
}
