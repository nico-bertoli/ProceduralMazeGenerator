using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Iterative implementation of randomizedDFS
/// </summary>
public class RandDfsIterMazeGenerator : AbsRandDfsMazeGenerator {

    protected override IEnumerator GenerateMazeImplementation(DataGrid grid, DataCell startCell)
    {
        float lastTimeFrameShown = Time.realtimeSinceStartup;

        InitVisitedCells(grid.RowsCount, grid.ColumnsCount);
        
        //mark current cell as visited and add it to the stack
        visitedCells[startCell.PosM, startCell.PosN] = true;
        Stack<DataCell> stack = new Stack<DataCell>();
        stack.Push(startCell);

        while (stack.Count > 0)
        {
            DataCell current = stack.Pop ();
            List<DataCell> neighbours = GetUnvisitedNeighbours(grid,current);
            if (neighbours.Count > 0)
            {
                //get random unvisited neighbour
                DataCell neigh = neighbours[Random.Range(0, neighbours.Count)];
                grid.RemoveWall(current, neigh);
                visitedCells[neigh.PosM, neigh.PosN] = true;
                stack.Push(current);
                stack.Push(neigh);

                if (isLiveGenerationEnabled)
                {
                    yield return new WaitForSeconds(liveGenerationDelay);
                }
                else if (Time.realtimeSinceStartup - lastTimeFrameShown > 0.1f)
                {
                    yield return null;
                    lastTimeFrameShown = Time.realtimeSinceStartup;
                }
                  
            }
        }
    }

}
