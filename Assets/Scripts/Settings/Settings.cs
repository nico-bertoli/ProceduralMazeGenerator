using UnityEngine;

public class Settings : Singleton<Settings>
{
    [field: SerializeField] public MazeSizeSettings MazeSettings { get; private set; }
    [field:SerializeField] public MazeVoxelGenSettings MazeGenerationSettings { get; private set; }
    [field: SerializeField] public LiveGenSettings LiveGenSettings { get; private set; }
    [field:SerializeField] public PlayerSettings PlayerSettings { get; private set; }
}
