using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Modifies a DataGrid to create a maze
/// </summary>
public abstract class AbsMazeGenerator : MonoBehaviour
{
    public enum eAlgorithms {
        DFSiterative,
        //DFSrecursive,     //REMOVED
        Willson,
        Kruskal
    }
    
    #region =========================================================================================== Private Fields
    
    private const float LIVE_GEN_MAX_DELAY = 0.4f;
    private float debug_generationStartTime;
    
    #endregion Private Fields
    #region =========================================================================================== Protected Fields
    
    protected float liveGenerationDelay;
    protected bool isLiveGenerationEnabled;
    
    #endregion Protected Fields
    
    #region ============================================================================================= Public Methods


    public IEnumerator GenerateMaze(DataGrid grid, DataCell startCell,bool isLiveGenerationEnabled) {
        this.isLiveGenerationEnabled = isLiveGenerationEnabled;
        
        //live generation always starts at min speed
        liveGenerationDelay = LIVE_GEN_MAX_DELAY;
        
        debug_generationStartTime = Time.time;

        yield return StartCoroutine(GenerateMazeImplementation(grid, startCell));
    }
    
    /// <summary>
    /// Sets generation step delay between 0 and max delay
    /// </summary>
    /// <param name="speed">Generation speed [0-100]</param>
    public void SetLiveGenerationSpeed(float speed) {
        if (isLiveGenerationEnabled)
        {
            speed = Mathf.Clamp(speed, 0, 100);
            liveGenerationDelay = LIVE_GEN_MAX_DELAY / 100 * Mathf.Abs(speed - 100);
        }
    }
    
    #endregion PublicMethods
    
    #region ========================================================================================== Protected Methods

    protected abstract IEnumerator GenerateMazeImplementation(DataGrid grid, DataCell startCell);
    
    #endregion Protected Methods
}
