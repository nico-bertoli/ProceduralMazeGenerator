using System.Collections;
using UnityEngine;

/// <summary>
/// Modifies a DataGrid to create a maze
/// </summary>
public abstract class AbsMazeGenStrategy
{
    public enum eMazeGenStrategy
    {
        DFSiterative,
        Willson,
        Kruskal
    }

    #region =========================================================================================== Protected Fields

    protected float liveGenerationDelay;
    protected bool isLiveGenerationEnabled;

    #endregion Protected Fields
    #region =========================================================================================== Private Properties

    private float liveGenerationMaxDelay => Settings.Instance.MazeGenerationSettings.LiveGenerationMaxDelay;

    #endregion Private Properties
    #region ============================================================================================= Public Methods

    public IEnumerator GenerateMaze(DataGrid grid, DataCell startCell, bool isLiveGenerationEnabled)
    {
        this.isLiveGenerationEnabled = isLiveGenerationEnabled;

        //live generation always starts at min speed
        liveGenerationDelay = liveGenerationMaxDelay;

        yield return Coroutiner.Instance.StartCoroutine(GenerateMazeImplementation(grid, startCell));
    }
    
    public void SetLiveGenerationSpeed(float speed)
    {
        speed = Mathf.Clamp(speed, 0, 100);
        liveGenerationDelay = liveGenerationMaxDelay / 100 * Mathf.Abs(speed - 100);
    }
    
    #endregion PublicMethods
    #region ========================================================================================== Protected Methods

    protected abstract IEnumerator GenerateMazeImplementation(DataGrid grid, DataCell startCell);

    #endregion Protected Methods
}
