using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// game object representation of a DataCell
/// </summary>
public class CellObj : MonoBehaviour
{
    [SerializeField] private WallObj topWall;
    [SerializeField] private WallObj rightWall;
    //======================================== fields
    private DataCell dataCell;
    private WallObj[] walls;
    

    //======================================== methods
    /// <summary>
    /// Initializes DataCell
    /// </summary>
    /// <param name="_cell">DataCell associated to this object</param>
    public void Init(DataCell _cell) {
        dataCell = _cell;
        transform.position = new Vector3(dataCell.PosN, 0, -dataCell.PosM);
        walls = new WallObj[] { topWall, rightWall };
        
        UpdateExternalState();

        dataCell.OnWallBuiltOrDestroyed += UpdateExternalState;
    }

    public void SetWallMeshesActive(bool active) {
        foreach (WallObj wall in walls)
            wall.SetMeshActive(active);
    }

    public void SetWallsWidht(float width) {
        foreach (WallObj wall in walls)
            wall.SetWidth(width);
    }

    /// <summary>
    /// Updates game object depending on dataCell state
    /// </summary>
    private void UpdateExternalState() {
        Debug.Log("updating external state");
        walls[0].gameObject.SetActive(dataCell.IsTopWallActive);
        walls[1].gameObject.SetActive(dataCell.IsRightWallActive);
    }
}
