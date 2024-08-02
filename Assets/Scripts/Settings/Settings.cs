using UnityEngine;

public class Settings : Singleton<Settings>
{
    [field:SerializeField] public MazeGenSettings MazeGenerationSettings { get; private set; }
    [field:SerializeField] public PlayerSettings PlayerSettings { get; private set; }
}
