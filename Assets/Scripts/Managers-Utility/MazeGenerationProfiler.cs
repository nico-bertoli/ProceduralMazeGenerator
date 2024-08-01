using System.Collections.Generic;
using UnityEngine;

public class MazeGenerationProfiler : MonoBehaviour
{
#if UNITY_EDITOR

    [SerializeField] private VoxelMaze maze;

    private float generationStartTime;
    private float generationEndTime;
    
    private void Start()
    {
        maze.OnGenerationStarted += OnGenerationStarted;
        maze.OnMazeDataStructureGenerated += OnGenerationEnded;
        maze.OnVoxelMeshGenerated += OnMazeChunksGenerated;
    }

    private void OnGenerationStarted() => generationStartTime = Time.time;

    private void OnGenerationEnded()
    {   
        generationEndTime = Time.time;
        Debug.Log($"Maze structure generation required: {Time.time - generationStartTime} seconds");
    }

    private void OnMazeChunksGenerated()
    {
        Debug.Log($"Mesh generation required: {Time.time - generationEndTime} seconds");
        Debug.Log($"Total generation required: {Time.time - generationStartTime} seconds");
    }

#endif
}
