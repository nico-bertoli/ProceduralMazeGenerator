using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Iterative implementation of randomizedDFS
/// </summary>
public class DFSIterMazeGenerator : AbsDfsMazeGenerator {

    protected override IEnumerator GenerateMazeImplementation(DataGrid grid, DataCell startCell) {
        
        InitVisitedCells(grid.RowsCount, grid.ColumnsCount);
        //mark current cell as visited and add it to the stack
        visitedCells[startCell.PosM, startCell.PosN] = true;
        Stack<DataCell> stack = new Stack<DataCell>();
        stack.Push(startCell);

        while (stack.Count > 0) {
            DataCell current = stack.Pop ();
            List<DataCell> neighs = GetUnvisitedNeighbours(grid,current);
            if (neighs.Count > 0) {
                //get a random unvisited neighbour
                DataCell neigh = neighs[Random.Range(0, neighs.Count)];
                grid.RemoveWall(current, neigh);
                visitedCells[neigh.PosM, neigh.PosN] = true;
                stack.Push(current);
                stack.Push(neigh);

                if (isLiveGenerationEnabled)
                {
                    if (liveGenerationDelay == 0)
                    {
                        yield return null;
                    }
                    else
                        yield return new WaitForSeconds(liveGenerationDelay);
                }
                    
            }
        }
    }

}
