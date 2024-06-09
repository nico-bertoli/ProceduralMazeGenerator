using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DataCell;

/// <summary>
/// Grid of squares with editable walls representation
/// </summary>
public class DataGrid {
    #region ===================================================================================================== Fields
    public int ColumnsCount { get; }
    public int RowsCount { get; }
    
    private readonly DataCell[,] cells;
    
    #endregion  Fields
    #region ============================================================================================= Public Methods
    
    public DataGrid (int rowsCount,int columnsCount) {

        RowsCount = rowsCount;
        ColumnsCount = columnsCount;
        cells = new DataCell[RowsCount,ColumnsCount];

        for (int m = 0; m < rowsCount; m++) {
            for (int n = 0; n < columnsCount; n++) {
                cells[m, n] = new DataCell(this, m, n);
            }
        }
    }
    
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
    public DataCell GetCell(int m, int n) =>  cells[m, n];

        //todo remove
    public eDirection GetRandomNeighbourDirection(DataCell _cell) {
        List<eDirection> possibleDirections = GetNeighboursDirections(_cell);
        return possibleDirections[Random.Range(0, possibleDirections.Count)];
    }

    //todo remove
    public eDirection? GetRandomNeighbourDirection (DataCell _cell, eDirection[] _except) {
        List<eDirection> possibleDirections = GetNeighboursDirections(_cell);
        foreach(eDirection impDir in _except)
            possibleDirections.Remove(impDir);

        if (possibleDirections.Count == 0) return null;
        return possibleDirections[Random.Range(0, possibleDirections.Count)];
    }
    
    public List<DataCell> GetNeighbours(DataCell _cell){

        List<DataCell> possibleNeighs = new List<DataCell>();
        List<eDirection> directions = GetNeighboursDirections(_cell);
        foreach (eDirection dir in directions)possibleNeighs.Add(GetNeighbourAtDir(_cell,dir));

        return possibleNeighs;
    }
    
    public DataCell GetNeighbourAtDir(DataCell cell, eDirection direction){
        switch (direction)
        {
            case eDirection.TOP:
                if (cell.MPos == 0)
                    return null;
                return cells[cell.MPos - 1, cell.NPos];
            
            case eDirection.BOTTOM:
                if (cell.MPos == RowsCount-1)
                    return null;
                return cells[cell.MPos + 1, cell.NPos];

            case eDirection.LEFT:
                if (cell.NPos == 0)
                    return null;
                return cells[cell.MPos, cell.NPos-1];
            
            case eDirection.RIGHT:
                if (cell.NPos == ColumnsCount-1)
                    return null;
                return cells[cell.MPos, cell.NPos+1];
            default:
                Debug.LogError($"direction: {direction} not recognized, returning null neighbour");
                return null;
        }
    }
    
    #endregion Public Methods
    #region ============================================================================================ Private Methods

    private List<eDirection> GetNeighboursDirections(DataCell _cell) {
        List<eDirection> ris = GetAllDirections();

        if (_cell.NPos == 0) ris.Remove(eDirection.LEFT);
        else if (_cell.NPos == ColumnsCount - 1) ris.Remove(eDirection.RIGHT);

        if (_cell.MPos == 0) ris.Remove(eDirection.TOP);
        else if (_cell.MPos == RowsCount - 1) ris.Remove(eDirection.BOTTOM);

        return ris;
    }

    #endregion Private Methods
}