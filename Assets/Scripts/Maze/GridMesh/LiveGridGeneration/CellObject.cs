using UnityEngine;

public class CellObject : MonoBehaviour
{
    #region ============================================================================================= Private Fields
    [SerializeField] private WallObject topWall;
    [SerializeField] private WallObject rightWall;

    private DataCell dataCell;
    
    #endregion Private Fields
    #region ============================================================================================= Public Methods
    public void Init(DataCell cell)
    {
        dataCell = cell;
        transform.position = new Vector3(dataCell.PosN, 0, -dataCell.PosM);
        
        RefreshWallsActive();
        dataCell.OnWallBuiltOrDestroyed += RefreshWallsActive;
    }

    public void SetWallsWidth(float width)
    {
        SetWallWidth(topWall, width);
        SetWallWidth(rightWall, width);
    }
    
    #endregion Public Methods
    #region ============================================================================================ Private Methods
    
    private void RefreshWallsActive() {
        topWall.gameObject.SetActive(dataCell.IsTopWallActive);
        rightWall.gameObject.SetActive(dataCell.IsRightWallActive);
    }

    private void SetWallWidth(WallObject wall, float width)
    {
        wall.SetWidth(width);
        wall.SetLength(1f + width);
    }

    #endregion Private Methods
}
