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
        if (GameController.Instance.Maze.IsLiveGenerationActive)
            return;
        
        generationStartTime = Time.time;
    }

    private void OnGenerationEnded()
    {
        if (GameController.Instance.Maze.IsLiveGenerationActive)
            return;
        
        generationEndTime = Time.time;
        Debug.Log($"Generation took {Time.time - generationStartTime} seconds");
    }

    private void OnMazeChunksGenerated()
    {
        if (GameController.Instance.Maze.IsLiveGenerationActive)
            return;
        
        Debug.Log($"Maze mesh generation took {Time.time - generationEndTime} seconds");
    }
    #endif
}
