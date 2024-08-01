using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DataGrid
{
    public enum Directions { Up, Right, Down, Left};
    
    #region ===================================================================================================== Fields
    public readonly int ColumnsCount;
    public readonly int RowsCount;
    private readonly DataCell[,] cells;

    #endregion  Fields
    #region ============================================================================================= Public Methods

    public int GetShorterSideCellsCount() => ColumnsCount < RowsCount ? ColumnsCount : RowsCount;

    public Vector3 GetExitPosition() => new Vector3(ColumnsCount - 1, 0, -RowsCount + 1);

    public DataGrid (int rowsCount,int columnsCount)
    {
        RowsCount = rowsCount;
        ColumnsCount = columnsCount;
        cells = new DataCell[RowsCount,ColumnsCount];

        for (int m = 0; m < rowsCount; m++)
            for (int n = 0; n < columnsCount; n++)
                cells[m, n] = new DataCell(this, m, n);
    }

    public DataCell GetCentralCell()
    {
        int m = RowsCount / 2;
        int n = ColumnsCount / 2;
        return cells[m, n];
    }
    
    public void RemoveWall(DataCell cell1, DataCell cell2)
    {
        if(AreCellsAdjacent(cell1, cell2) == false)
        {
            Debug.LogError($"{nameof(RemoveWall)} Received not adjacent cells ({cell1}, {cell2}), returning");
            return;
        }

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
        if (AreCellsAdjacent(cell1, cell2) == false)
        {
            Debug.LogError($"{nameof(BuildWall)} Received not adjacent cells ({cell1}, {cell2}), returning");
            return;
        }

        if (cell1.PosM < cell2.PosM)
            cell2.IsTopWallActive = true;
        else if (cell1.PosM > cell2.PosM)
            cell1.IsTopWallActive = true;
        else if (cell1.PosN > cell2.PosN)
            cell2.IsRightWallActive = true;
        else
            cell1.IsRightWallActive = true;
    }

    public DataCell GetCell(int m, int n) => cells[m, n];

    public Directions? GetRandomNeighbourDirection (DataCell cell, Directions[] preventDirections)
    {
        Debug.Assert(preventDirections.Length > 0 && preventDirections != null,
            $"{nameof(GetRandomNeighbourDirection)} called passing null or empty {nameof(preventDirections)} list!");

        List<Directions> possibleDirections = GetNeighboursDirections(cell);

        if (preventDirections.Length > 0)
        {
            foreach (Directions preventDirection in preventDirections)
                possibleDirections.Remove(preventDirection);
        }
       
        if (possibleDirections.Count == 0)
            return null;

        return possibleDirections[Random.Range(0, possibleDirections.Count)];
    }

    public Directions GetRandomNeighbourDirection(DataCell cell)
    {
        List<Directions> possibleDirections = GetNeighboursDirections(cell);
        return possibleDirections[Random.Range(0, possibleDirections.Count)];
    }

    public List<DataCell> GetNeighbours(DataCell cell)
    {
        List<DataCell> possibleNeighbours = new List<DataCell>();
        List<Directions> directions = GetNeighboursDirections(cell);
        foreach (Directions dir in directions)
            possibleNeighbours.Add(GetNeighbourAtDirection(cell,dir));

        return possibleNeighbours;
    }
    
    public DataCell GetNeighbourAtDirection(DataCell cell, Directions direction)
    {
        switch (direction)
        {
            case Directions.Up:
                if (cell.PosM == 0)
                    return null;
                return cells[cell.PosM - 1, cell.PosN];
            
            case Directions.Down:
                if (cell.PosM == RowsCount-1)
                    return null;
                return cells[cell.PosM + 1, cell.PosN];

            case Directions.Left:
                if (cell.PosN == 0)
                    return null;
                return cells[cell.PosM, cell.PosN-1];
            
            case Directions.Right:
                if (cell.PosN == ColumnsCount-1)
                    return null;
                return cells[cell.PosM, cell.PosN+1];
            default:
                Debug.LogError($"direction: {direction} not recognized, returning null neighbour");
                return null;
        }
    }

    public List<Directions> GetNeighboursDirections(DataCell cell)
    {
        List<Directions> ris = GetAllDirections();

        if (cell.PosN == 0) ris.Remove(Directions.Left);
        else if (cell.PosN == ColumnsCount - 1) ris.Remove(Directions.Right);

        if (cell.PosM == 0) ris.Remove(Directions.Up);
        else if (cell.PosM == RowsCount - 1) ris.Remove(Directions.Down);

        return ris;
    }
    
    /// <summary>
    /// Returns the inverse direction of the given one
    /// </summary>
    /// <param name="_dir">Direction you want to get the inverse</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Directions GetInverseDirection(Directions direction)
    {
        switch (direction)
        {
            case Directions.Up:return Directions.Down;
            case Directions.Down: return Directions.Up;
            case Directions.Left: return Directions.Right;
            case Directions.Right: return Directions.Left;
            default: throw new Exception($"{direction} not recognized");
        }
    }
    
    /// <summary>
    /// Returns a list containing all the possible directions
    /// </summary>
    /// <returns></returns>
    public static List<Directions> GetAllDirections()
    {
        List<Directions> allDir = new List<Directions>();
        int directionsCount = Enum.GetValues(typeof(Directions)).Length;
        
        for (int i = 0; i < directionsCount; i++)
            allDir.Add((Directions)i);
        return allDir;
    }

    #endregion Public Methods

    private bool AreCellsAdjacent(DataCell cell1, DataCell cell2)
    {
        if (Mathf.Abs(cell1.PosM - cell2.PosM) == 1 || Mathf.Abs(cell1.PosN - cell2.PosN) == 1)
            return false;
        if (cell1.Equals(cell2))
            return false;

        return true;
    }
}