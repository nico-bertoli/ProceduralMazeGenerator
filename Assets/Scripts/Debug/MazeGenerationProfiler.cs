using UnityEngine;

public class MazeGenerationProfiler : MonoBehaviour
{
    #if UNITY_EDITOR
    private bool showDebugLog => MainController.Instance.Maze.IsLiveGenerationActive == false;
    
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
        if (showDebugLog == false)
            return;
        
        generationStartTime = Time.time;
    }

    private void OnGenerationEnded()
    {
        if (showDebugLog == false)
            return;
        
        generationEndTime = Time.time;
        Debug.Log($"Generation took {Time.time - generationStartTime} seconds");
    }

    private void OnMazeChunksGenerated()
    {
        if (showDebugLog == false)
            return;
        
        Debug.Log($"Maze mesh generation took {Time.time - generationEndTime} seconds");
        Debug.Log($"Total generation took {Time.time - generationStartTime} seconds");
    }
    #endif
}
