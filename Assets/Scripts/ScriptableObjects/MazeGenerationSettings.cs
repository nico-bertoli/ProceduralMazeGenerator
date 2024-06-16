using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObjects/MazeGenerationSettings",fileName = "MazeGenerationSettings")]
public class MazeGenerationSettings : ScriptableObject
{
    [field:SerializeField] public int LiveGenerationMaxSideCells { get; private set; } = 40;
    [field:SerializeField] public int NotLiveGenerationMaxCells { get; private set; } = 400;

    [field: SerializeField] public int MinCells { get; private set; } = 10;


}
