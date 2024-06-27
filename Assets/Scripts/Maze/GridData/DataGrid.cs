using System;
using System.Collections.Generic;
using UnityEngine;
using static DataCell;
using Random = UnityEngine.Random;

public class DataGrid {
    //public enum eDirection { TOP, RIGHT, BOTTOM, LEFT };
    public enum Direction { Up, Right, Down, Left};
    
    #region ===================================================================================================== Fields
    public int ColumnsCount { get; }
    public int RowsCount { get; }
    public int CellsCount => ColumnsCount * RowsCount;
    
    private readonly DataCell[,] cells;

    #endregion  Fields
    #region ============================================================================================= Public Methods

    public int GetShorterSideCellsCount() => ColumnsCount < RowsCount ? ColumnsCount : RowsCount;

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
    public Direction? GetRandomNeighbourDirection (DataCell cell, Direction[] preventDirections)
    {
        Debug.Assert(preventDirections.Length > 0, $"{nameof(GetRandomNeighbourDirection)} called passing empty {nameof(preventDirections)} list!" +
            $"Consider using method version without this parameter");

        List<Direction> possibleDirections = GetNeighboursDirections(cell);

        if (preventDirections.Length > 0)
        {
            foreach (Direction preventDirection in preventDirections)
                possibleDirections.Remove(preventDirection);
        }
       
        if (possibleDirections.Count == 0)
            return null;

        return possibleDirections[Random.Range(0, possibleDirections.Count)];
    }

    public Direction GetRandomNeighbourDirection(DataCell cell)
    {
        List<Direction> possibleDirections = GetNeighboursDirections(cell);
        return possibleDirections[Random.Range(0, possibleDirections.Count)];
    }


    public List<DataCell> GetNeighbours(DataCell cell){

        List<DataCell> possibleNeighbours = new List<DataCell>();
        List<Direction> directions = GetNeighboursDirections(cell);
        foreach (Direction dir in directions)
            possibleNeighbours.Add(GetNeighbourAtDirection(cell,dir));

        return possibleNeighbours;
    }
    
    public DataCell GetNeighbourAtDirection(DataCell cell, Direction direction){
        switch (direction)
        {
            case Direction.Up:
                if (cell.PosM == 0)
                    return null;
                return cells[cell.PosM - 1, cell.PosN];
            
            case Direction.Down:
                if (cell.PosM == RowsCount-1)
                    return null;
                return cells[cell.PosM + 1, cell.PosN];

            case Direction.Left:
                if (cell.PosN == 0)
                    return null;
                return cells[cell.PosM, cell.PosN-1];
            
            case Direction.Right:
                if (cell.PosN == ColumnsCount-1)
                    return null;
                return cells[cell.PosM, cell.PosN+1];
            default:
                Debug.LogError($"direction: {direction} not recognized, returning null neighbour");
                return null;
        }
    }

    public List<Direction> GetNeighboursDirections(DataCell cell) {
        List<Direction> ris = GetAllDirections();

        if (cell.PosN == 0) ris.Remove(Direction.Left);
        else if (cell.PosN == ColumnsCount - 1) ris.Remove(Direction.Right);

        if (cell.PosM == 0) ris.Remove(Direction.Up);
        else if (cell.PosM == RowsCount - 1) ris.Remove(Direction.Down);

        return ris;
    }
    
    /// <summary>
    /// Returns the inverse direction of the given one
    /// </summary>
    /// <param name="_dir">Direction you want to get the inverse</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Direction GetInverseDirection(Direction direction) {
        switch (direction) {
            case Direction.Up:return Direction.Down;
            case Direction.Down: return Direction.Up;
            case Direction.Left: return Direction.Right;
            case Direction.Right: return Direction.Left;
            default: throw new Exception($"{direction} not recognized");
        }
    }
    
    /// <summary>
    /// Returns a list containing all the possible directions
    /// </summary>
    /// <returns></returns>
    public static List<Direction> GetAllDirections() {
        List<Direction> allDir = new List<Direction>();
        int directionsCount = Enum.GetValues(typeof(Direction)).Length;
        
        for (int i = 0; i < directionsCount; i++)
            allDir.Add((Direction)i);
        return allDir;
    }
    
    #endregion Public Methods
}