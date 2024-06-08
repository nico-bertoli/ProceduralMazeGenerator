using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// game object representation of a DataCell
/// </summary>
public class CellObj : MonoBehaviour
{
    //======================================== fields
    private DataCell dataCell;
    private WallObj[] walls = new WallObj[2];

    //======================================== methods
    /// <summary>
    /// Initializes DataCell
    /// </summary>
    /// <param name="_cell">DataCell associated to this object</param>
    public void Init(DataCell _cell) {
        dataCell = _cell;
        transform.position = new Vector3(dataCell.NPos, 0, -dataCell.MPos);

        for (int i = 0; i < DataCell.N_WALLS; i++)
            walls[i] = transform.GetChild(i).gameObject.GetComponent<WallObj>();

        UpdateExternalState();
        //when data cell changes its state, gameobject is updated
        dataCell.OnWallBuiltOrDestroyed += UpdateExternalState;
    }

    public void SetWallMeshesActive(bool _active) {
        foreach (WallObj wall in walls)
            wall.SetMeshActive(_active);
    }

    public void SetWallsWidht(float _width) {
        foreach (WallObj wall in walls)
            wall.SetWidth(_width);
    }

    /// <summary>
    /// Updates game object depending on dataCell state
    /// </summary>
    public void UpdateExternalState() {
        IReadOnlyList<bool> wallsState = dataCell.GetWallsState();
        for(int i = 0; i < wallsState.Count; i++)
            walls[i].gameObject.SetActive(wallsState[i]);
    }
}
