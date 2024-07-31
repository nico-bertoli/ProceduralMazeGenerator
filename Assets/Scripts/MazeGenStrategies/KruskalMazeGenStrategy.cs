using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//algorithm: https://weblog.jamisbuck.org/2011/1/3/maze-generation-kruskal-s-algorithm

public class KruskalMazeGenStrategy : AbsMazeGenStrategy {
    
    /// <summary>
    /// Edge between two cells
    /// </summary>
    private class Edge {

        public DataCell cell1;
        public DataCell cell2;

        public Edge(DataCell cell1, DataCell cell2)
        {
            this.cell1 = cell1;
            this.cell2 = cell2;
        }
    }
    
    protected override IEnumerator GenerateMazeImplementation(DataGrid grid, DataCell startCell)
    {
        //get all edges
        List<Edge> unvisitedEdges = GetEdges(grid);

        float lastTimeFrameShown = Time.realtimeSinceStartup;

        //create a set for each cell containing the cell itself
        HashSet<DataCell>[,] sets = new HashSet<DataCell>[grid.RowsCount, grid.ColumnsCount];
        for (int m = 0; m < grid.RowsCount; m++)
        {
            for (int n = 0; n < grid.ColumnsCount; n++)
            {
                sets[m, n] = new HashSet<DataCell>();
                sets[m, n].Add(grid.GetCell(m, n));

                if(Time.realtimeSinceStartup - lastTimeFrameShown > 0.1f)
                    yield return null;
            }
        }

        //while there are unvisited cells
        while (unvisitedEdges.Count > 0)
        {
            //get a random edge
            int randomIndex = Random.Range(0, unvisitedEdges.Count);
            Edge randomEdge = unvisitedEdges[randomIndex];
            unvisitedEdges.RemoveAt(randomIndex);

            //get cells sets
            HashSet<DataCell> set1 = sets[randomEdge.cell1.PosM, randomEdge.cell1.PosN];
            HashSet<DataCell> set2 = sets[randomEdge.cell2.PosM, randomEdge.cell2.PosN];

            // if the sets are different
            if (!set2.Contains(randomEdge.cell1))
            {
                //put cells in same set
                set1.UnionWith(set2);
                foreach (DataCell cell in set1)
                    sets[cell.PosM, cell.PosN] = set1;

                //remove wall between cells
                grid.RemoveWall(randomEdge.cell1, randomEdge.cell2);
            }

            if (isLiveGenerationEnabled)
                yield return new WaitForSeconds(liveGenerationDelay);
            else if (Time.realtimeSinceStartup - lastTimeFrameShown > 0.1f)
            {
                yield return null;
                lastTimeFrameShown = Time.realtimeSinceStartup;
            }
               
        }
    }

    /// <summary>
    /// Returns all edges for the given grid
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    private List<Edge> GetEdges(DataGrid grid)
    {
        List<Edge> ris = new List<Edge>();

        for (int m = 0; m < grid.RowsCount; m++)
        {
            for (int n = 0; n < grid.ColumnsCount; n++)
            {
                if(n+1 < grid.ColumnsCount)
                    ris.Add(new Edge(grid.GetCell(m, n), grid.GetCell(m, n + 1)));
                if(m+1 < grid.RowsCount)
                    ris.Add(new Edge(grid.GetCell(m, n), grid.GetCell(m+1, n)));
            }
        }
        return ris;   
    }
}
