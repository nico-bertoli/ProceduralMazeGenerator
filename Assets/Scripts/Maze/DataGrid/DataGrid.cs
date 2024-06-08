using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DataCell;

/// <summary>
/// Grid of squares with editable walls representation
/// </summary>
public class DataGrid {

    //======================================== fields
    /// <summary>
    /// Number of columns
    /// </summary>
    public int Ncol { get; private set; }

    /// <summary>
    /// Number of rows
    /// </summary>
    public int Nrows { get; private set; }

    /// <summary>
    /// Cells that make up the grid
    /// </summary>
    protected DataCell[,] cells;

    //======================================== methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_nRows">Number of rows</param>
    /// <param name="_nColumns">Number of columns</param>
    public DataGrid (int _nRows,int _nColumns) {

        Nrows = _nRows;
        Ncol = _nColumns;
        cells = new DataCell[Nrows,Ncol];

        for (int m = 0; m < _nRows; m++) {
            for (int n = 0; n < _nColumns; n++) {
                cells[m, n] = new DataCell(this, m, n);
            }
        }
    }

    /// <summary>
    /// Returns list of possible neighbours (excludes out of grid ones)
    /// </summary>
    /// <param name="_cell">Cell you want to get the neighbours</param>
    /// <returns></returns>
    public List<DataCell> GetPossibleNeighbours(DataCell _cell) {

        List<DataCell> possibleNeighs = new List<DataCell>();
        List<eDirection> directions = GetPossibleDirections(_cell);
        foreach (eDirection dir in directions)possibleNeighs.Add(GetNeighbourAtDir(_cell,dir));

        return possibleNeighs;
    }

    /// <summary>
    /// Returns neighbour cell at specified direction
    /// </summary>
    /// <param name="_cell"></param>
    /// <param name="_direction"></param>
    /// <returns></returns>
    public DataCell GetNeighbourAtDir(DataCell _cell, eDirection _direction) {
        return GetNeighbours(_cell)[(int)_direction];
    }

    /// <summary>
    /// Removes wall between specified cells
    /// </summary>
    /// <param name="_cell1"></param>
    /// <param name="_cell2"></param>
    public void RemoveWall(DataCell _cell1, DataCell _cell2) {
        if (_cell1.MPos < _cell2.MPos)
            _cell2.RemoveWall(eDirection.TOP);
        else if (_cell1.MPos > _cell2.MPos)
            _cell1.RemoveWall(eDirection.TOP);
        else if (_cell1.NPos > _cell2.NPos)
            _cell2.RemoveWall(eDirection.RIGHT);
        else
            _cell1.RemoveWall(eDirection.RIGHT);
    }

    /// <summary>
    /// Builds a wall between specified cells
    /// </summary>
    /// <param name="_cell1"></param>
    /// <param name="_cell2"></param>
    public void BuildWall(DataCell _cell1, DataCell _cell2) {
        if (_cell1.MPos < _cell2.MPos)
            _cell2.BuildWall(eDirection.TOP);
        else if (_cell1.MPos > _cell2.MPos)
            _cell1.BuildWall(eDirection.TOP);
        else if (_cell1.NPos > _cell2.NPos)
            _cell2.BuildWall(eDirection.RIGHT);
        else
            _cell1.BuildWall(eDirection.RIGHT);
    }

    /// <summary>
    /// Returns cell at specified coordinates
    /// </summary>
    /// <param name="m">Cell m index (distance from the top)</param>
    /// <param name="n">Cell n index (distance from left side)</param>
    /// <returns></returns>
    public DataCell GetCell(int m, int n) {
        return cells[m, n];
    }

    /// <summary>
    /// Returns list of possible directions accessible from the cell
    /// </summary>
    /// <param name="_cell"></param>
    /// <returns></returns>
    public List<eDirection> GetPossibleDirections(DataCell _cell) {
        List<eDirection> ris = DataCell.GetAllDirections();

        if (_cell.NPos == 0) ris.Remove(eDirection.LEFT);
        else if (_cell.NPos == Ncol - 1) ris.Remove(eDirection.RIGHT);

        if (_cell.MPos == 0) ris.Remove(eDirection.TOP);
        else if (_cell.MPos == Nrows - 1) ris.Remove(eDirection.BOTTOM);

        return ris;
    }

    /// <summary>
    /// Returns random direction accessible from the cell
    /// </summary>
    /// <param name="_cell"></param>
    /// <returns></returns>
    public eDirection GetRandomDirection(DataCell _cell) {
        List<eDirection> possibleDirections = GetPossibleDirections(_cell);
        return possibleDirections[Random.Range(0, possibleDirections.Count)];
    }

    /// <summary>
    /// Returns a random direction accessible from the cell, excluding received directions
    /// </summary>
    /// <param name="_cell"></param>
    /// <param name="_except">Directions that cannot be returned</param>
    /// <returns></returns>
    public eDirection? GetRandomDirection(DataCell _cell, eDirection[] _except) {
        List<eDirection> possibleDirections = GetPossibleDirections(_cell);
        foreach(eDirection impDir in _except)
            possibleDirections.Remove(impDir);

        if (possibleDirections.Count == 0) return null;
        return possibleDirections[Random.Range(0, possibleDirections.Count)];
    }

    /// <summary>
    /// Returns array of 4 neighbours (elements = null if there is no one)
    /// </summary>
    /// <param name="_cell">Cell you want to get the neighbours</param>
    /// <returns></returns>
    private DataCell[] GetNeighbours(DataCell _cell) {

        DataCell[] neighbours = new DataCell[4];
        // find top neigh
        if (_cell.MPos == 0) neighbours[0] = null;
        else neighbours[0] = (cells[_cell.MPos - 1, _cell.NPos]);
        // right
        if (_cell.NPos == Ncol - 1) neighbours[1] = null;
        else neighbours[1] = (cells[_cell.MPos, _cell.NPos + 1]);
        // bottom
        if (_cell.MPos == Nrows - 1) neighbours[2] = null;
        else neighbours[2] = (cells[_cell.MPos + 1, _cell.NPos]);
        // left
        if (_cell.NPos == 0) neighbours[3] = null;
        else neighbours[3] = (cells[_cell.MPos, _cell.NPos - 1]);

        return neighbours;
    }
}