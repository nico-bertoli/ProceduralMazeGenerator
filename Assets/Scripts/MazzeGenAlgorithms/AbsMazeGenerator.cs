using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Edits a DataGrid to create a maze
/// </summary>
public abstract class AbsMazeGenerator : MonoBehaviour
{
    //======================================== fields
    /// <summary>
    /// Max wait between steps in live generation (seconds)
    /// </summary>
    const float LIVE_GEN_MAX_DELAY = 0.4f;
    protected float liveGenerationDelay;
    protected bool liveGeneration;

    //======================================== methods
    public enum eAlgorithms {
        DFSiterative,
        //DFSrecursive,     //REMOVED
        Willson,
        Kruskal
    }

    /// <summary>
    /// Transforms the given grid to create a maze
    /// </summary>
    /// <param name="_grid">DataGrid to edit</param>
    /// <param name="_startCell">DataCell to use as maze entrance</param>
    /// <param name="_liveGeneration">True = show live generation</param>
    /// <returns></returns>
    public IEnumerator GenerateMaze(DataGrid _grid, DataCell _startCell,bool _liveGeneration) {
        liveGeneration = _liveGeneration;
        
        //live generation always starts slowly
        liveGenerationDelay = LIVE_GEN_MAX_DELAY;

        Debug.Log("Maze generation starated");

        yield return StartCoroutine(GenerateMazeImpl(_grid, _startCell));

        Debug.Log("Maze generation completed");
    }
    
    /// <summary>
    /// sets how fast the maze is generated (works only with live generation)
    /// </summary>
    /// <param name="_speed">speed to set (0-100)</param>
    public void SetLiveGenerationSpeed(float _speed) {
        if (liveGeneration) {
            if (_speed < 0) _speed = 0;
            else if (_speed > 100) _speed = 100;
            liveGenerationDelay = LIVE_GEN_MAX_DELAY / 100 * Mathf.Abs(_speed - 100);
        }
    }

    /// <summary>
    /// Defines algorithm behaviour
    /// </summary>
    /// <param name="_grid"></param>
    /// <param name="_startCell"></param>
    /// <returns></returns>
    protected abstract IEnumerator GenerateMazeImpl(DataGrid _grid, DataCell _startCell);

}
