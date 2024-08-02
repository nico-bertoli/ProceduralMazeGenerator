using System.Collections;
using System.Threading;
using UnityEngine;

/// <summary>
/// Modifies a DataGrid to create a maze
/// </summary>
public abstract class AbsMazeGenStrategy
{
    public enum MazeGenStrategy
    {
        DFSiterative,
        Willson,
        Kruskal
    }

    #region =========================================================================================== Fields

    protected float liveGenerationDelay;
    protected bool isLiveGenerationEnabled;
    protected MonoBehaviour coroutiner;

    private float lastScreenRefreshTime;

    #endregion Fields
    #region =========================================================================================== Properties

    protected bool MustRefreshScreen =>
       Time.realtimeSinceStartup - lastScreenRefreshTime > genSettings.VoxelGenMaxTimeWithoutRefreshingScreen;

    private MazeGenSettings genSettings => Settings.Instance.MazeGenerationSettings;
   
    #endregion Properties
    #region ============================================================================================= PublicMethods

    public IEnumerator GenerateMaze(DataGrid grid, DataCell startCell, bool isLiveGenerationEnabled, MonoBehaviour coroutiner)
    {
        this.isLiveGenerationEnabled = isLiveGenerationEnabled;
        this.coroutiner = coroutiner;

        //live generation always starts at min speed
        liveGenerationDelay = genSettings.LiveGenMaxDelay;

        lastScreenRefreshTime = Time.realtimeSinceStartup;
        yield return coroutiner.StartCoroutine(GenerateMazeImplementation(grid, startCell));
    }
    
    public void SetLiveGenerationSpeed(float speed)
    {
        speed = Mathf.Clamp(speed, 0, 100);
        liveGenerationDelay = genSettings.LiveGenMaxDelay / 100 * Mathf.Abs(speed - 100);
    }

    #endregion PublicMethods
    #region ========================================================================================== Protected Methods

    protected IEnumerator RefreshScreenCor()
    {
        yield return null;
        lastScreenRefreshTime = Time.realtimeSinceStartup;
    }

    protected abstract IEnumerator GenerateMazeImplementation(DataGrid grid, DataCell startCell);

    #endregion Protected Methods
}
