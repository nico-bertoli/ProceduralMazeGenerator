using System.Collections.Generic;

//algorithm: https://weblog.jamisbuck.org/2010/12/27/maze-generation-recursive-backtracking

public abstract class AbsRandDfsMazeGenStrategy : AbsMazeGenStrategy {

    #region =========================================================================================== Fields
    
    /// <summary>
    /// Matrix indicating if each cell has been visited
    /// </summary>
    protected static bool[,] visitedCells;
    
    #endregion Fields
    
    #region ========================================================================================== Methods
    
    protected List<DataCell> GetUnvisitedNeighbours(DataGrid grid, DataCell cell)
    {
        List<DataCell> unvisitedNeighbours = grid.GetNeighbours(cell);

        for(int i = unvisitedNeighbours.Count-1; i >= 0; i--)
            if (visitedCells[unvisitedNeighbours[i].PosM, unvisitedNeighbours[i].PosN])
                unvisitedNeighbours.RemoveAt(i);

        return unvisitedNeighbours;
    }

    protected void InitVisitedCells(int nRows, int nColumns)
    {
        visitedCells = new bool[nRows, nColumns];
        for (int m = 0; m < nRows; m++)
            for (int n = 0; n < nColumns; n++)
                visitedCells[m, n] = false;
    }
    
    #endregion Methods
}
