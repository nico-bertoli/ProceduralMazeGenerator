using UnityEngine;

public class MazeGenerationProfiler : MonoBehaviour
{
#if UNITY_EDITOR

    //-------------- Fields
    [SerializeField] private VoxelMaze voxelMaze;

    private float generationStartTime;
    private float generationEndTime;
    
    //-------------- Methods
    private void Start()
    {
        voxelMaze.OnGenerationStarted += OnGenerationStarted;
        voxelMaze.OnMazeDataStructureGenerated += OnGenerationEnded;
        voxelMaze.OnVoxelMeshGenerated += OnMazeChunksGenerated;
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
