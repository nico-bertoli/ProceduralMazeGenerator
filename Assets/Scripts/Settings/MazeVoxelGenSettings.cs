using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MazeVoxelGenSettings", fileName = "MazeVoxelGenSettings")]
public class MazeVoxelGenSettings : ScriptableObject
{
    [field: SerializeField] public float RefreshScreenEverySeconds { get; private set; } = 0.2f;
    [field: SerializeField] public int NumberOfCellsComposingChunk { get; private set; } = 60;
}
