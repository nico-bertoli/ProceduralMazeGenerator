using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : Singleton<Settings>
{
    [field:SerializeField] public MazeGenerationSettings mazeGenerationSettings { get; private set; }
}
