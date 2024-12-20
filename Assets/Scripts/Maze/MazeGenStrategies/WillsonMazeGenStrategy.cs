using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GridDirections;

//algorithm: https://weblog.jamisbuck.org/2011/1/20/maze-generation-wilson-s-algorithm.html
public class WillsonMazeGenStrategy : AbsMazeGenStrategy
{  
    private class Step
    {
        public readonly DataCell Cell;
        public Directions Direction;

        public Step(DataCell cell, Directions direction)
        {
            Cell = cell;
            Direction = direction;
        }
    }

    protected override IEnumerator GenerateMazeImplementation(DataGrid dataGrid, DataCell startingCell)
    {
        HashSet<DataCell> finalTreeCells = new HashSet<DataCell>() {startingCell};

        //cells out of final tree
        HashSet<DataCell> cellsNotInFinalTree = new HashSet<DataCell>();
        for (int m = 0; m < dataGrid.RowsCount; m++)
            for (int n = 0; n < dataGrid.ColumnsCount; n++)
                cellsNotInFinalTree.Add(dataGrid.GetCell(m, n));

        cellsNotInFinalTree.Remove(startingCell);

        List<Step> firstRandomWalk = new List<Step>();
        yield return Coroutiner.Instance.StartCoroutine(GetFirstRandomWalkCor(dataGrid, startingCell, firstRandomWalk));
        MergeRandomWalkInFinalTree(dataGrid, finalTreeCells, cellsNotInFinalTree, firstRandomWalk);

        //while there are not connected cells...
        while (cellsNotInFinalTree.Count > 0)
        {
            //fin a random walk
            List<Step> randomWalk = new List<Step>();

            //yields for coroutine launching it during current frame
            IEnumerator coroutineToCallDuringFrame = GetRandomWalkCor(dataGrid, finalTreeCells, cellsNotInFinalTree, randomWalk);
            while (coroutineToCallDuringFrame.MoveNext())
                yield return coroutineToCallDuringFrame.Current;

            MergeRandomWalkInFinalTree(dataGrid,finalTreeCells,cellsNotInFinalTree, randomWalk);
        }
    }

    private void MergeRandomWalkInFinalTree(DataGrid dataGrid, HashSet<DataCell> finalTreeCells, HashSet<DataCell> notInFinalTreeCells, List<Step> randomWalk)
    {
        //add random walk cells to the final tree (and remove them from out of tree set)
        for (int i = 0; i < randomWalk.Count; i++)
        {
            finalTreeCells.Add(randomWalk[i].Cell);
            notInFinalTreeCells.Remove(randomWalk[i].Cell);

            // optimization for non live generation: walls are edited only after a complete random walk is found
            if (!isLiveGenerationEnabled && i != randomWalk.Count - 1)
                dataGrid.RemoveWall(randomWalk[i].Cell, dataGrid.GetNeighbourAtDirection(randomWalk[i].Cell, randomWalk[i].Direction));
        }
    }

    /// <summary>
    /// Generates the first random from the starting cell, this incerases the probability for the next random walk to find the final tree.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="startingCell"></param>
    /// <param name="outRandomWalk"></param>
    /// <returns></returns>
    private IEnumerator GetFirstRandomWalkCor(DataGrid grid, DataCell startingCell, List<Step> outRandomWalk)
    {
        Debug.Assert(outRandomWalk.Count == 0, $"{nameof(GetRandomWalkCor)} received {nameof(outRandomWalk)} should be empty, but it isn't!");

        int firstRandomWalkLength = grid.GetShorterSideCellsCount()-3;
        HashSet<DataCell> visitedCells = new HashSet<DataCell>() { startingCell };
        Directions allwaysPreventedDirection = (Directions)Random.Range(0,System.Enum.GetValues(typeof(Directions)).Length);

        Directions randomDirection = (Directions)grid.GetRandomNeighbourDirection(startingCell, new Directions[] { allwaysPreventedDirection });
        outRandomWalk.Add(new Step(startingCell, randomDirection));

        for (int i = 0; i< firstRandomWalkLength; i++)
        {
            Step previousStep = outRandomWalk[outRandomWalk.Count - 1];
            DataCell newCell = grid.GetNeighbourAtDirection(previousStep.Cell, previousStep.Direction);

            // get new random direction (direction of the previous cel is excluded)
            Directions previousCellDirection = GetInverseDirection(previousStep.Direction);

            //cannot be null
            Directions newDirection = (Directions)grid.GetRandomNeighbourDirection(newCell, new Directions[] { previousCellDirection, allwaysPreventedDirection } );

            Step newStep = new Step(newCell, newDirection);
            outRandomWalk.Add(newStep);
            visitedCells.Add(newCell);

            if (isLiveGenerationEnabled)
            {
                grid.RemoveWall(previousStep.Cell, newStep.Cell);
                yield return new WaitForSeconds(liveGenerationDelay);
            }
        }
    }
    
    //iterators cannot use out parameters -_-
    private IEnumerator GetRandomWalkCor (DataGrid grid, HashSet<DataCell> finalTreeCells, HashSet<DataCell> notInFinalTreeCells, List<Step> outRandomWalk)
    {
        Debug.Assert(outRandomWalk.Count == 0, $"{nameof(GetRandomWalkCor)} received {nameof(outRandomWalk)} should be empty, but it isn't!");

        DataCell randomStartingCell = notInFinalTreeCells.ElementAt(Random.Range(0, notInFinalTreeCells.Count));
        Directions randomDirection = grid.GetRandomNeighbourDirection(randomStartingCell);

        outRandomWalk.Add(new Step(randomStartingCell, randomDirection));

        while (true)
        {
            Step previousStep = outRandomWalk[outRandomWalk.Count - 1];
            DataCell newCell = grid.GetNeighbourAtDirection(previousStep.Cell, previousStep.Direction);

            // get new random direction (direction of the previous cel is excluded)
            Directions previousCellDirection = GetInverseDirection(previousStep.Direction);
            Directions newDirection = (Directions)grid.GetRandomNeighbourDirection(newCell, new Directions[] { previousCellDirection });
            
            bool foundLoop = false;

            // if the new step creates a loop in path, the loop is cut
            for (int i = 0; i < outRandomWalk.Count; i++)
            {
                if (outRandomWalk[i].Cell == newCell)
                {
                    foundLoop = true;
                    for (int j = outRandomWalk.Count - 1; j > i; j--)
                    {

                        if (isLiveGenerationEnabled)
                            grid.BuildWall(outRandomWalk[j].Cell, outRandomWalk[j - 1].Cell);
                        outRandomWalk.RemoveAt(j);
                    }
                    outRandomWalk[i].Direction = newDirection;
                    break;
                }
            }

            //otherwise, the new step is added to randomwalk
            if (!foundLoop)
            {
                Step newStep = new Step(newCell, (Directions)newDirection);
                outRandomWalk.Add(newStep);

                if (isLiveGenerationEnabled)
                {
                    grid.RemoveWall(previousStep.Cell, newStep.Cell);
                    yield return new WaitForSeconds(liveGenerationDelay);
                }
                else if (MustRefreshScreen)
                    yield return coroutiner.StartCoroutine(RefreshScreenCor());
            }

            //if random walk reached the final tree, can be returned
            if (finalTreeCells.Contains(newCell))
                break;
        }
    }
}
