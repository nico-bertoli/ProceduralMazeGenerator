using UnityEngine;

public class MazeGenerationProfiler : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private Maze maze;
    private bool canShowDebugLog => maze.IsLiveGenerationActive == false;
    
    private float generationStartTime;
    private float generationEndTime;
    
    private void Start()
    {
        maze.OnGenerationStarted += OnGenerationStarted;
        maze.OnGenerationEnded += OnGenerationEnded;
        maze.OnMazeChunksGenerated += OnMazeChunksGenerated;
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
