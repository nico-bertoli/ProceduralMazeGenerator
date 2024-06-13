using UnityEngine;

public class MazeGenerationProfiler : MonoBehaviour
{
    #if UNITY_EDITOR
    private float generationStartTime;
    private float generationEndTime;
    private void Start()
    {
        GameController.Instance.Maze.OnGenerationStarted += OnGenerationStarted;
        GameController.Instance.Maze.OnGenerationEnded += OnGenerationEnded;
        GameController.Instance.Maze.OnMazeChunksGenerated += OnMazeChunksGenerated;
    }

    private void OnGenerationStarted()
    {
        generationStartTime = Time.time;
    }

    private void OnGenerationEnded()
    {
        generationEndTime = Time.time;
        Debug.Log($"Generation took {Time.time - generationStartTime} seconds");
    }

    private void OnMazeChunksGenerated()
    {
        Debug.Log($"Maze mesh generation took {Time.time - generationEndTime} seconds");
    }
    #endif
}
