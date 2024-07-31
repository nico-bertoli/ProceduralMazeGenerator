using System.Collections;
using System.Threading;
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

   

    #region =========================================================================================== Fields

    protected float liveGenerationDelay;
    protected bool isLiveGenerationEnabled;
    protected MonoBehaviour coroutiner;

    private float lastScreenRefreshTime;

    #endregion Fields
    #region =========================================================================================== Properties

    protected bool MustRefreshScreen =>
       Time.realtimeSinceStartup - lastScreenRefreshTime > genSettings.MaxTimeWithoutRefreshingScreenDuringHiddenGen;

    private MazeGenerationSettings genSettings => Settings.Instance.MazeGenerationSettings;
   
    #endregion Properties
    #region ============================================================================================= Public Methods

    public IEnumerator GenerateMaze(DataGrid grid, DataCell startCell, bool isLiveGenerationEnabled, MonoBehaviour coroutiner)
    {
        this.isLiveGenerationEnabled = isLiveGenerationEnabled;
        this.coroutiner = coroutiner;

        //live generation always starts at min speed
        liveGenerationDelay = genSettings.LiveGenerationMaxDelay;

        lastScreenRefreshTime = Time.realtimeSinceStartup;
        yield return coroutiner.StartCoroutine(GenerateMazeImplementation(grid, startCell));
    }
    
    public void SetLiveGenerationSpeed(float speed)
    {
        speed = Mathf.Clamp(speed, 0, 100);
        liveGenerationDelay = genSettings.LiveGenerationMaxDelay / 100 * Mathf.Abs(speed - 100);
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
