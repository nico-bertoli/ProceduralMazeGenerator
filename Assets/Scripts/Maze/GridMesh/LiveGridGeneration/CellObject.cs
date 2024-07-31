using UnityEngine;

public class CellObject : MonoBehaviour
{
    #region ============================================================================================= Private Fields
    [SerializeField] private WallObject topWall;
    [SerializeField] private WallObject rightWall;

    private DataCell dataCell;
    private WallObject[] walls;
    
    #endregion Private Fields
    #region ============================================================================================= Public Methods
    public void Init(DataCell _cell)
    {
        dataCell = _cell;
        transform.position = new Vector3(dataCell.PosN, 0, -dataCell.PosM);
        walls = new WallObject[] { topWall, rightWall };
        
        RefreshWallsActive();

        dataCell.OnWallBuiltOrDestroyed += RefreshWallsActive;
    }

    public void SetWallMeshesActive(bool active)
    {
        foreach (WallObject wall in walls)
            wall.SetMeshActive(active);
    }

    public void SetWallsWidth(float width)
    {
        foreach (WallObject wall in walls)
        {
            wall.SetWidth(width);
            wall.SetLength(1f + width);
        }   
    }
    
    #endregion Public Methods
    #region ============================================================================================ Private Methods
    
    private void RefreshWallsActive() {
        walls[0].gameObject.SetActive(dataCell.IsTopWallActive);
        walls[1].gameObject.SetActive(dataCell.IsRightWallActive);
    }
    
    #endregion Private Methods
}
