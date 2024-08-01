using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using static GridDirections;

public class DataGrid
{
    #region ============================================================================================= Fields

    public readonly int ColumnsCount;
    public readonly int RowsCount;
    private readonly DataCell[,] cells;

    #endregion  Fields

    public DataGrid(int rowsCount, int columnsCount)
    {
        RowsCount = rowsCount;
        ColumnsCount = columnsCount;
        cells = new DataCell[RowsCount, ColumnsCount];

        for (int m = 0; m < rowsCount; m++)
            for (int n = 0; n < columnsCount; n++)
                cells[m, n] = new DataCell(this, m, n);
    }

    #region ============================================================================================= Public Methods

    //====================================================================== Get Stuff 
    public int GetShorterSideCellsCount() => ColumnsCount < RowsCount ? ColumnsCount : RowsCount;
    public Vector3 GetExitPosition() => new Vector3(ColumnsCount - 1, 0, -RowsCount + 1);
    public DataCell GetCell(int m, int n) => cells[m, n];
    public DataCell GetCentralCell()
    {
        int m = RowsCount / 2;
        int n = ColumnsCount / 2;
        return cells[m, n];
    }

    //====================================================================== Get Neighbours 
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

    public List<DataCell> GetAllNeighbours(DataCell cell)
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

    //====================================================================== Walls 
    public void RemoveWall(DataCell cell1, DataCell cell2)
    {
        if (AreCellsAdjacent(cell1, cell2) == false)
        {
            Debug.LogError($"{nameof(RemoveWall)} Received not adjacent cells ({cell1}, {cell2}), returning");
            return;
        }

        if (cell1.PosM < cell2.PosM)
            cell2.SetTopWallActive(false);
        else if (cell1.PosM > cell2.PosM)
            cell1.SetTopWallActive(false);
        else if (cell1.PosN > cell2.PosN)
            cell2.SetRightWallActive(false);
        else
            cell1.SetRightWallActive(false);
    }

    public void BuildWall(DataCell cell1, DataCell cell2)
    {
        if (AreCellsAdjacent(cell1, cell2) == false)
        {
            Debug.LogError($"{nameof(BuildWall)} Received not adjacent cells ({cell1}, {cell2}), returning");
            return;
        }

        if (cell1.PosM < cell2.PosM)
            cell2.SetTopWallActive(true);
        else if (cell1.PosM > cell2.PosM)
            cell1.SetTopWallActive(true);
        else if (cell1.PosN > cell2.PosN)
            cell2.SetRightWallActive(true);
        else
            cell1.SetRightWallActive(true);
    }

    #endregion Public Methods
    #region ============================================================================================= Private Methods
    private List<Directions> GetNeighboursDirections(DataCell cell)
    {
        List<Directions> result = GetAllDirections();

        if (cell.PosN == 0) result.Remove(Directions.Left);
        else if (cell.PosN == ColumnsCount - 1) result.Remove(Directions.Right);

        if (cell.PosM == 0) result.Remove(Directions.Up);
        else if (cell.PosM == RowsCount - 1) result.Remove(Directions.Down);

        return result;
    }

    private bool AreCellsAdjacent(DataCell cell1, DataCell cell2)
    {
        if (Mathf.Abs(cell1.PosM - cell2.PosM) == 1 &&  Mathf.Abs(cell1.PosN - cell2.PosN) == 0)
            return true;

        if (Mathf.Abs(cell1.PosN - cell2.PosN) == 1 && Mathf.Abs(cell1.PosM - cell2.PosM) == 0)
            return true;

        return false;
    }

    #endregion Private Methods
}