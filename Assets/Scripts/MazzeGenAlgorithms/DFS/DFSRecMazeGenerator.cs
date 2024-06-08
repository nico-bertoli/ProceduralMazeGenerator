using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Recursive implementation of randomized DFS
/// </summary>
public class DFSRecMazeGenerator : AbsDfsMazeGenerator {

    protected override IEnumerator GenerateMazeImpl(DataGrid _grid, DataCell _cell) {

        //set current cell as visited
        visitedCells[_cell.MPos, _cell.NPos] = true;

        if (liveGeneration)
            yield return new WaitForSeconds(liveGenerationDelay);

        List<DataCell> unvisitedNeighbours = getUnvisitedNeighbours(_grid, _cell);

        //while there are unvisited neighbours
        while (unvisitedNeighbours.Count > 0) {

            DataCell randUnvisitedNeigh = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
            _grid.RemoveWall(_cell, randUnvisitedNeigh);

            //recursion on neighbour
            yield return (StartCoroutine(GenerateMazeImpl(_grid, randUnvisitedNeigh)));

            unvisitedNeighbours = getUnvisitedNeighbours(_grid, _cell);
        }
    }

}
