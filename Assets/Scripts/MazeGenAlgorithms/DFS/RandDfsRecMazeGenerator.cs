using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Recursive implementation of randomized DFS
/// </summary>
public class RandDfsRecMazeGenerator : AbsRandDfsMazeGenerator {

    protected override IEnumerator GenerateMazeImplementation(DataGrid grid, DataCell startCell)
    {

        InitVisitedCells(grid.RowsCount, grid.ColumnsCount);
        
        //set current cell as visited
        visitedCells[startCell.PosM, startCell.PosN] = true;

        if (isLiveGenerationEnabled)
            yield return new WaitForSeconds(liveGenerationDelay);

        List<DataCell> unvisitedNeighbours = GetUnvisitedNeighbours(grid, startCell);

        //while there are unvisited neighbours
        while (unvisitedNeighbours.Count > 0)
        {
            DataCell randUnvisitedNeigh = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
            grid.RemoveWall(startCell, randUnvisitedNeigh);

            //recursion on neighbour
            yield return (StartCoroutine(GenerateMazeImplementation(grid, randUnvisitedNeigh)));

            unvisitedNeighbours = GetUnvisitedNeighbours(grid, startCell);
        }
    }

}
