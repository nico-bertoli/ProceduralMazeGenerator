using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//algorithm: https://weblog.jamisbuck.org/2010/12/27/maze-generation-recursive-backtracking

/// <summary>
/// Iterative implementation of randomized depth first search
/// </summary>
public class RandDfsIterMazeGenStrategy : AbsMazeGenStrategy
{
    private VisitedCells visitedCells;

    private class VisitedCells
    {
        bool[,] visitedCellsMatrix;
        public VisitedCells(int nRows, int nColumns) => visitedCellsMatrix = new bool[nRows, nColumns]; //default value is false
        public bool IsVisited(int mPos, int nPos) => visitedCellsMatrix[mPos, nPos];
        public bool IsVisited(DataCell cell) => visitedCellsMatrix[cell.PosM, cell.PosN];
        public void SetVisited(DataCell cell) => visitedCellsMatrix[cell.PosM, cell.PosN] = true;
    }

    protected override IEnumerator GenerateMazeImplementation(DataGrid grid, DataCell startCell)
    {
        visitedCells = new VisitedCells(grid.RowsCount, grid.ColumnsCount);

        //mark current cell as visited and add it to the stack
        visitedCells.SetVisited(startCell);
        Stack<DataCell> stack = new Stack<DataCell>();
        stack.Push(startCell);

        while (stack.Count > 0)
        {
            DataCell current = stack.Pop();
            List<DataCell> neighbours = GetUnvisitedNeighbours(grid, current);
            if (neighbours.Count > 0)
            {
                //get random unvisited neighbour
                DataCell randomNeigh = neighbours[Random.Range(0, neighbours.Count)];
                grid.RemoveWall(current, randomNeigh);
                visitedCells.SetVisited(randomNeigh);
                stack.Push(current);
                stack.Push(randomNeigh);

                if (isLiveGenerationEnabled)
                    yield return new WaitForSeconds(liveGenerationDelay);

                else if (MustRefreshScreen)
                    yield return coroutiner.StartCoroutine(RefreshScreenCor());
            }
        }
    }

    private List<DataCell> GetUnvisitedNeighbours(DataGrid grid, DataCell cell)
    {
        List<DataCell> unvisitedNeighbours = grid.GetNeighbours(cell);

        for (int i = unvisitedNeighbours.Count - 1; i >= 0; i--)
            if (visitedCells.IsVisited(unvisitedNeighbours[i]))
                unvisitedNeighbours.RemoveAt(i);

        return unvisitedNeighbours;
    }
}
