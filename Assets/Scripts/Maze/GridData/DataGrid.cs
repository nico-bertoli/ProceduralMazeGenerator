using System;
using System.Collections.Generic;
using UnityEngine;
using static DataCell;
using Random = UnityEngine.Random;

public class DataGrid {
    
    public enum eDirection { TOP, RIGHT, BOTTOM, LEFT};
    
    #region ===================================================================================================== Fields
    public int ColumnsCount { get; }
    public int RowsCount { get; }
    
    private readonly DataCell[,] cells;

    #endregion  Fields
    #region ============================================================================================= Public Methods

    public Vector3 GetExitPosition()
    {
        return new Vector3(ColumnsCount-1, 0, -RowsCount+1);
    }
    
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

    public DataCell GetCentralCell()
    {
        int m = RowsCount / 2;
        int n = ColumnsCount / 2;
        return cells[m, n];
    }
    
    public void RemoveWall(DataCell cell1, DataCell cell2)
    {
        Debug.Assert(Mathf.Abs(cell1.PosM - cell2.PosM) == 1 ||  Mathf.Abs(cell1.PosN - cell2.PosN) == 1 , 
            $"{nameof(RemoveWall)} received not adjacent cells! {cell1}, {cell2}");
        
        Debug.Assert(cell1.Equals(cell2) == false, 
            $"{nameof(RemoveWall)} received same cell, wall couldn't be removed");
        
        if (cell1.PosM < cell2.PosM)
            cell2.IsTopWallActive = false;
        else if (cell1.PosM > cell2.PosM)
            cell1.IsTopWallActive = false;
        else if (cell1.PosN > cell2.PosN)
            cell2.IsRightWallActive = false;
        else
            cell1.IsRightWallActive = false;
    }
    
    public void BuildWall(DataCell cell1, DataCell cell2)
    {
        Debug.Assert(Mathf.Abs(cell1.PosM - cell2.PosM) == 1||  Mathf.Abs(cell1.PosN - cell2.PosN) == 1 , 
            $"{nameof(BuildWall)} received not adjacent cells! {cell1}, {cell2}");
        
        if (cell1.PosM < cell2.PosM)
            cell2.IsTopWallActive = true;
        else if (cell1.PosM > cell2.PosM)
            cell1.IsTopWallActive = true;
        else if (cell1.PosN > cell2.PosN)
            cell2.IsRightWallActive = true;
        else
            cell1.IsRightWallActive = true;
    }

    /// <summary>
    /// Returns cell at specified coordinates
    /// </summary>
    /// <param name="m">Cell m index (distance from the top)</param>
    /// <param name="n">Cell n index (distance from left side)</param>
    /// <returns></returns>
    public DataCell GetCell(int m, int n) =>  cells[m, n];

    //todo remove
    public eDirection? GetRandomNeighbourDirection (DataCell cell, eDirection[] preventDirections) {
        List<eDirection> possibleDirections = GetNeighboursDirections(cell);
        foreach(eDirection preventDirection in preventDirections)
            possibleDirections.Remove(preventDirection);

        if (possibleDirections.Count == 0) return null;
        return possibleDirections[Random.Range(0, possibleDirections.Count)];
    }
    
    public List<DataCell> GetNeighbours(DataCell cell){

        List<DataCell> possibleNeighbours = new List<DataCell>();
        List<eDirection> directions = GetNeighboursDirections(cell);
        foreach (eDirection dir in directions)
            possibleNeighbours.Add(GetNeighbourAtDir(cell,dir));

        return possibleNeighbours;
    }
    
    public DataCell GetNeighbourAtDir(DataCell cell, eDirection direction){
        switch (direction)
        {
            case eDirection.TOP:
                if (cell.PosM == 0)
                    return null;
                return cells[cell.PosM - 1, cell.PosN];
            
            case eDirection.BOTTOM:
                if (cell.PosM == RowsCount-1)
                    return null;
                return cells[cell.PosM + 1, cell.PosN];

            case eDirection.LEFT:
                if (cell.PosN == 0)
                    return null;
                return cells[cell.PosM, cell.PosN-1];
            
            case eDirection.RIGHT:
                if (cell.PosN == ColumnsCount-1)
                    return null;
                return cells[cell.PosM, cell.PosN+1];
            default:
                Debug.LogError($"direction: {direction} not recognized, returning null neighbour");
                return null;
        }
    }

    public List<eDirection> GetNeighboursDirections(DataCell cell) {
        List<eDirection> ris = GetAllDirections();

        if (cell.PosN == 0) ris.Remove(eDirection.LEFT);
        else if (cell.PosN == ColumnsCount - 1) ris.Remove(eDirection.RIGHT);

        if (cell.PosM == 0) ris.Remove(eDirection.TOP);
        else if (cell.PosM == RowsCount - 1) ris.Remove(eDirection.BOTTOM);

        return ris;
    }
    
    /// <summary>
    /// Returns the inverse direction of the given one
    /// </summary>
    /// <param name="_dir">Direction you want to get the inverse</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static eDirection GetInverseDirection(eDirection direction) {
        switch (direction) {
            case eDirection.TOP:return eDirection.BOTTOM;
            case eDirection.BOTTOM: return eDirection.TOP;
            case eDirection.LEFT: return eDirection.RIGHT;
            case eDirection.RIGHT: return eDirection.LEFT;
            default: throw new Exception($"{direction} not recognized");
        }
    }
    
    /// <summary>
    /// Returns a list containing all the possible directions
    /// </summary>
    /// <returns></returns>
    public static List<eDirection> GetAllDirections() {
        List<eDirection> allDir = new List<eDirection>();
        int directionsCount = Enum.GetValues(typeof(eDirection)).Length;
        
        for (int i = 0; i < directionsCount; i++)
            allDir.Add((eDirection)i);
        return allDir;
    }
    
    #endregion Public Methods
}