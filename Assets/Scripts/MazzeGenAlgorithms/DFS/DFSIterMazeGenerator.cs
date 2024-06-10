using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Iterative implementation of randomizedDFS
/// </summary>
public class DFSIterMazeGenerator : AbsDfsMazeGenerator {

    protected override IEnumerator GenerateMazeImpl(DataGrid _grid, DataCell _cell) {

        yield return StartCoroutine(base.GenerateMazeImpl(_grid, _cell));

        //mark current cell as visited and add it to the stack
        visitedCells[_cell.PosM, _cell.PosN] = true;
        Stack<DataCell> stack = new Stack<DataCell>();
        stack.Push(_cell);

        while (stack.Count > 0) {
            DataCell current = stack.Pop ();
            List<DataCell> neighs = getUnvisitedNeighbours(_grid,current);
            if (neighs.Count > 0) {
                //get a random unvisited neighbour
                DataCell neigh = neighs[Random.Range(0, neighs.Count)];
                _grid.RemoveWall(current, neigh);
                visitedCells[neigh.PosM, neigh.PosN] = true;
                stack.Push(current);
                stack.Push(neigh);

                if (liveGeneration)
                    yield return new WaitForSeconds(liveGenerationDelay);
            }
        }
    }

}
