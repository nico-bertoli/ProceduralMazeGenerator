using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//algorithm: https://weblog.jamisbuck.org/2010/12/27/maze-generation-recursive-backtracking

public abstract class AbsDfsMazeGenerator : AbsMazeGenerator {

    //======================================== fields
    /// <summary>
    /// Matrix of flags indicating if each cell has been visited
    /// </summary>
    protected static bool[,] visitedCells;

    //======================================== methods
    protected override IEnumerator GenerateMazeImpl(DataGrid _grid, DataCell _startCell) {
        initVisitedCells(_grid.Nrows, _grid.Ncol);
        yield return null;
    }

    /// <summary>
    /// Returns unvisited neighbours of the cell
    /// </summary>
    /// <param name="_grid"></param>
    /// <param name="_cell"></param>
    /// <returns></returns>
    protected List<DataCell> getUnvisitedNeighbours(DataGrid _grid, DataCell _cell) {
        List<DataCell> unvisitedNeighbours = _grid.GetPossibleNeighbours(_cell);

        for(int i = unvisitedNeighbours.Count-1; i >= 0; i--) {
            if (visitedCells[unvisitedNeighbours[i].MPos, unvisitedNeighbours[i].NPos]) unvisitedNeighbours.RemoveAt(i);
        }
        return unvisitedNeighbours;
    }

    /// <summary>
    /// Initializes visited cells grid
    /// </summary>
    /// <param name="_nrows"></param>
    /// <param name="_ncol"></param>
    private void initVisitedCells(int _nrows, int _ncol) {
        visitedCells = new bool[_nrows, _ncol];
        for (int m = 0; m < _nrows; m++)
            for (int n = 0; n < _ncol; n++)
                visitedCells[m, n] = false;
    }

}
