using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//algorithm: https://weblog.jamisbuck.org/2011/1/3/maze-generation-kruskal-s-algorithm

public class KruskalMazeGenerator : AbsMazeGenerator {
        protected override IEnumerator GenerateMazeImplementation(DataGrid grid, DataCell startCell) {

        //get all edges
        List<Edge> unvisitedEdges = getEdges(grid);

        //create a set for each cell containing the cell itself
        HashSet<DataCell>[,] sets = new HashSet<DataCell>[grid.RowsCount, grid.ColumnsCount];
        for (int m = 0; m < grid.RowsCount; m++) {
            for (int n = 0; n < grid.ColumnsCount; n++) {
                sets[m, n] = new HashSet<DataCell>();
                sets[m, n].Add(grid.GetCell(m, n));
            }
            yield return null;
        }

        int yieldIndex = 0;
        //while there are unvisited cells
        while (unvisitedEdges.Count > 0) {

            //get a random edge
            int randomID = Random.Range(0, unvisitedEdges.Count);
            Edge randomEdge = unvisitedEdges[randomID];
            unvisitedEdges.RemoveAt(randomID);

            //get cells sets
            HashSet<DataCell> set1 = sets[randomEdge.cell1.PosM, randomEdge.cell1.PosN];
            HashSet<DataCell> set2 = sets[randomEdge.cell2.PosM, randomEdge.cell2.PosN];

            // if the sets are different
            if (!set2.Contains(randomEdge.cell1)) {

                //put cells in same set
                set1.UnionWith(set2);
                foreach (DataCell cell in set1)
                    sets[cell.PosM, cell.PosN] = set1;

                //remove wall between cells
                grid.RemoveWall(randomEdge.cell1, randomEdge.cell2);

                if (isLiveGenerationEnabled)
                    yield return new WaitForSeconds(liveGenerationDelay);
            }

            if(yieldIndex%500==0)yield return null;
            yieldIndex++;
        }
    }

    /// <summary>
    /// Returns all edges for the given grid
    /// </summary>
    /// <param name="_grid"></param>
    /// <returns></returns>
    private List<Edge> getEdges(DataGrid _grid) {

        List<Edge> ris = new List<Edge>();

        for (int m = 0; m < _grid.RowsCount; m++)
            for (int n = 0; n < _grid.ColumnsCount; n++) {
                if(n+1 < _grid.ColumnsCount)
                ris.Add(new Edge(_grid.GetCell(m, n), _grid.GetCell(m, n + 1)));
                if(m+1 < _grid.RowsCount)
                ris.Add(new Edge(_grid.GetCell(m, n), _grid.GetCell(m+1, n)));
            }

        return ris;   
    }

    /// <summary>
    /// Edge between two cells
    /// </summary>
    private class Edge {

        public DataCell cell1;
        public DataCell cell2;

        public Edge(DataCell _cell1, DataCell _cell2) {
            cell1 = _cell1;
            cell2 = _cell2;
        }
    }

}
