using UnityEngine;

public class MazeGenerationProfiler : MonoBehaviour
{
#if UNITY_EDITOR
    private bool canShowDebugLog => MainController.Instance.Maze.IsLiveGenerationActive == false;
    
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
        if (canShowDebugLog == false)
            return;
        
        generationStartTime = Time.time;
    }

    private void OnGenerationEnded()
    {
        if (canShowDebugLog == false)
            return;
        
        generationEndTime = Time.time;
        Debug.Log($"Maze structure generation required: {Time.time - generationStartTime} seconds");
    }

    private void OnMazeChunksGenerated()
    {
        if (canShowDebugLog == false)
            return;
        
        Debug.Log($"Mesh generation required: {Time.time - generationEndTime} seconds");
        Debug.Log($"Total generation required: {Time.time - generationStartTime} seconds");
    }
#endif
}
