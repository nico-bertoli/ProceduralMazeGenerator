using System.Collections;
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

    private LiveGenSettings liveGenSettings => Settings.Instance.LiveGenSettings;

    protected bool MustRefreshScreen =>
       Time.realtimeSinceStartup - lastScreenRefreshTime > Settings.Instance.MazeGenerationSettings.RefreshScreenEverySeconds;
    #endregion Properties
    #region ============================================================================================= PublicMethods

    public IEnumerator GenerateMaze(DataGrid grid, DataCell startCell, bool isLiveGenerationEnabled, MonoBehaviour coroutiner)
    {
        this.isLiveGenerationEnabled = isLiveGenerationEnabled;
        this.coroutiner = coroutiner;

        lastScreenRefreshTime = Time.realtimeSinceStartup;
        yield return coroutiner.StartCoroutine(GenerateMazeImplementation(grid, startCell));
    }

    public void SetLiveGenerationSpeed(float speed)
    {
        speed = Mathf.Clamp(speed, 0, 100);
        liveGenerationDelay = liveGenSettings.MaxStepDelay / 100 * Mathf.Abs(speed - 100);
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
