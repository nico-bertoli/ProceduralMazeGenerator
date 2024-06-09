using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DataCell
{
    //======================================== fields
    /// <summary>
    /// M position of the cell (distance from top)
    /// </summary>
    public int MPos { get; private set; }

    /// <summary>
    /// N position of the cell (distance from left margin)
    /// </summary>
    public int NPos { get; private set; }

    /// <summary>
    /// Possible directions you can move to from a cell
    /// </summary>
    public enum eDirection { TOP, RIGHT, BOTTOM, LEFT, N_DIRECTIONS};

    /// <summary>
    /// Number of walls handled by each cell
    /// </summary>
    public const int N_WALLS = 2;

    public Action OnWallBuiltOrDestroyed;

    /// <summary>
    /// Flags indicating if each wall is enabled
    /// </summary>
    protected bool[] walls = new bool[N_WALLS];

    /// <summary>
    /// Grid object this cell is associated to
    /// </summary>
    private DataGrid grid;

    //======================================== methods
    public DataCell(DataGrid _grid, int _m, int _n) {
        NPos = _n;
        MPos = _m;
        grid = _grid;
        SetAllWallsActive(true);    //most of maze generation algorithms start with a grid with walls active
    }

    /// <summary>
    /// Returns a list containing all the possible directions
    /// </summary>
    /// <returns></returns>
    public static List<eDirection> GetAllDirections() {
        List<eDirection> allDir = new List<eDirection>();
        for (int i = 0; i < (int)eDirection.N_DIRECTIONS; i++)
            allDir.Add((eDirection)i);
        return allDir;
    }

    /// <summary>
    /// Returns the inverse direction of the given one
    /// </summary>
    /// <param name="_dir">Direction you want to get the inverse</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static eDirection GetInverseDirection(eDirection _dir) {
        switch (_dir) {
            case eDirection.TOP:return eDirection.BOTTOM;
            case eDirection.BOTTOM: return eDirection.TOP;
            case eDirection.LEFT: return eDirection.RIGHT;
            case eDirection.RIGHT: return eDirection.LEFT;
        }
        throw new Exception("Cell.GetInverseDirection: input not recognized");
    }
    
    /// <summary>
    /// Disables wall in the given direction
    /// </summary>
    /// <param name="_direction"></param>
    public void RemoveWall(eDirection _direction) {
        walls[(int)_direction] = false;
        if(OnWallBuiltOrDestroyed!=null)OnWallBuiltOrDestroyed();
    }
    /// <summary>
    /// Enables wall in the given direction
    /// </summary>
    /// <param name="_direction"></param>
    public void BuildWall(eDirection _direction) {
        walls[(int)_direction] = true;
        if (OnWallBuiltOrDestroyed != null) OnWallBuiltOrDestroyed();
    }

    /// <summary>
    /// Enables/disables all walls
    /// </summary>
    /// <param name="_active"></param>
    public void SetAllWallsActive(bool _active) {
        for(int i = 0; i<N_WALLS; i++)
            walls[i] = _active;
    }

    /// <summary>
    /// Returns array indicating if each wall is enabled/disabled
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<bool> GetWallsState() {
        return walls;
    }

    public override string ToString() {
        return "[" + MPos.ToString() + "," + NPos + "]";
    }

    // ------------------------------------------ hash set

    public override int GetHashCode() {
        return MPos * grid.ColumnsCount + NPos;
    }
    public override bool Equals(object _obj) {
        DataCell otherCell = _obj as DataCell;
        if (otherCell == null || otherCell.NPos != NPos || otherCell.MPos != MPos) return false;
        return true;
    }
}
