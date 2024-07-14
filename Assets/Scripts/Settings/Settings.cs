using UnityEngine;

public class Settings : Singleton<Settings>
{
    [field:SerializeField] public MazeGenerationSettings MazeGenerationSettings { get; private set; }
    [field:SerializeField] public PlayerSettings PlayerSettings { get; private set; }
}
