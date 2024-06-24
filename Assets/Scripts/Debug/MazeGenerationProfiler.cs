using UnityEngine;

public class MazeGenerationProfiler : MonoBehaviour
{
    #if UNITY_EDITOR
    private float generationStartTime;
    private float generationEndTime;
    private void Start()
    {
        MainController.Instance.Maze.OnGenerationStarted += OnGenerationStarted;
        MainController.Instance.Maze.OnGenerationEnded += OnGenerationEnded;
        MainController.Instance.Maze.OnMazeChunksGenerated += OnMazeChunksGenerated;
    }

    private void OnGenerationStarted()
    {
        if (MainController.Instance.Maze.IsLiveGenerationActive)
            return;
        
        generationStartTime = Time.time;
    }

    private void OnGenerationEnded()
    {
        if (MainController.Instance.Maze.IsLiveGenerationActive)
            return;
        
        generationEndTime = Time.time;
        Debug.Log($"Generation took {Time.time - generationStartTime} seconds");
    }

    private void OnMazeChunksGenerated()
    {
        if (MainController.Instance.Maze.IsLiveGenerationActive)
            return;
        
        Debug.Log($"Maze mesh generation took {Time.time - generationEndTime} seconds");
    }
    #endif
}
