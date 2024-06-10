using UnityEngine;

public class CellObj : MonoBehaviour
{
    #region ============================================================================================= Private Fields
    [SerializeField] private WallObj topWall;
    [SerializeField] private WallObj rightWall;

    private DataCell dataCell;
    private WallObj[] walls;
    
    #endregion Private Fields
    #region ============================================================================================= Public Methods
    public void Init(DataCell _cell) {
        dataCell = _cell;
        transform.position = new Vector3(dataCell.PosN, 0, -dataCell.PosM);
        walls = new WallObj[] { topWall, rightWall };
        
        RefreshWallsActive();

        dataCell.OnWallBuiltOrDestroyed += RefreshWallsActive;
    }

    public void SetWallMeshesActive(bool active) {
        foreach (WallObj wall in walls)
            wall.SetMeshActive(active);
    }

    public void SetWallsWidth(float width) {
        foreach (WallObj wall in walls)
            wall.SetWidth(width);
    }
    
    #endregion Public Methods
    #region ============================================================================================ Private Methods
    
    private void RefreshWallsActive() {
        walls[0].gameObject.SetActive(dataCell.IsTopWallActive);
        walls[1].gameObject.SetActive(dataCell.IsRightWallActive);
    }
    
    #endregion Private Methods
}
