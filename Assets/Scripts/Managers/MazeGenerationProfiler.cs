using UnityEngine;

public class MazeGenerationProfiler : MonoBehaviour
{
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
        Debug.Log($"Generation took {Time.time - generationEndTime} seconds");
    }

    private void OnMazeChunksGenerated()
    {
        Debug.Log($"Maze mesh generation took {Time.time - generationEndTime} seconds");
    }
}
