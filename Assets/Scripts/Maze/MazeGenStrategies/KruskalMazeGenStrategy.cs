using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//algorithm: https://weblog.jamisbuck.org/2011/1/3/maze-generation-kruskal-s-algorithm

public class KruskalMazeGenStrategy : AbsMazeGenStrategy
{
    /// <summary>
    /// Edge between two cells
    /// </summary>
    private class Edge
    {
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
        List<Edge> unvisitedEdges = GetAllGridEdges(grid);
        HashSet<DataCell>[,] sets = GetSetForEachCellContainingItself(grid);

        while (unvisitedEdges.Count > 0)
        {
            int randomIndex = Random.Range(0, unvisitedEdges.Count);
            Edge randomEdge = unvisitedEdges[randomIndex];
            unvisitedEdges.RemoveAt(randomIndex);

            var (set1, set2) = GetEdgeCellsSets(randomEdge);

            // if the sets are different
            if (set2.Contains(randomEdge.cell1)==false)
            {
                //put cells in same set
                set1.UnionWith(set2);
                foreach (DataCell cell in set1)
                    sets[cell.PosM, cell.PosN] = set1;

                //remove wall between cells
                grid.RemoveWall(randomEdge.cell1, randomEdge.cell2);

                if (isLiveGenerationEnabled)
                    yield return new WaitForSeconds(liveGenerationDelay);
            }
            if (isLiveGenerationEnabled == false && MustRefreshScreen)
                yield return coroutiner.StartCoroutine(RefreshScreenCor());
        }

        //------------- local

        (HashSet<DataCell>, HashSet<DataCell>) GetEdgeCellsSets(Edge edge) => 
            (sets[edge.cell1.PosM, edge.cell1.PosN], sets[edge.cell2.PosM, edge.cell2.PosN]);
    }

    private HashSet<DataCell>[,] GetSetForEachCellContainingItself(DataGrid dataGrid)
    {
        HashSet<DataCell>[,] sets = new HashSet<DataCell>[dataGrid.RowsCount, dataGrid.ColumnsCount];
        for (int m = 0; m < dataGrid.RowsCount; m++)
            for (int n = 0; n < dataGrid.ColumnsCount; n++)
                sets[m, n] = new HashSet<DataCell> {dataGrid.GetCell(m, n) };
           
        return sets;
    }

    private List<Edge> GetAllGridEdges(DataGrid grid)
    {
        List<Edge> edges = new List<Edge>();

        for (int m = 0; m < grid.RowsCount; m++)
        {
            for (int n = 0; n < grid.ColumnsCount; n++)
            {
                if(n+1 < grid.ColumnsCount)
                    edges.Add(new Edge(grid.GetCell(m, n), grid.GetCell(m, n + 1)));
                if(m+1 < grid.RowsCount)
                    edges.Add(new Edge(grid.GetCell(m, n), grid.GetCell(m+1, n)));
            }
        }
        return edges;   
    }
}
