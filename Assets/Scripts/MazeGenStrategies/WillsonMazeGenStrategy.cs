using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DataGrid;

//algorithm: https://weblog.jamisbuck.org/2011/1/20/maze-generation-wilson-s-algorithm.html
public class WillsonMazeGenStrategy : AbsMazeGenStrategy
{  
    private class Step
    {
        public DataCell cell;
        public Direction direction;

        public Step(DataCell cell, Direction direction)
        {
            this.cell = cell;
            this.direction = direction;
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
            finalTreeCells.Add(randomWalk[i].cell);
            notInFinalTreeCells.Remove(randomWalk[i].cell);

            // optimization for non live generation: walls are edited only after a complete random walk is found
            if (!isLiveGenerationEnabled && i != randomWalk.Count - 1)
                dataGrid.RemoveWall(randomWalk[i].cell, dataGrid.GetNeighbourAtDirection(randomWalk[i].cell, randomWalk[i].direction));
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
        int firstRandomWalkLength = grid.GetShorterSideCellsCount()-3;
        HashSet<DataCell> visitedCells = new HashSet<DataCell>() { startingCell };
        Direction allwaysPreventedDirection = (Direction)Random.Range(0,System.Enum.GetValues(typeof(Direction)).Length);

        Direction randomDirection = (Direction)grid.GetRandomNeighbourDirection(startingCell, new Direction[] { allwaysPreventedDirection });
        outRandomWalk.Add(new Step(startingCell, randomDirection));

        for (int i = 0; i< firstRandomWalkLength; i++)
        {
            Step previousStep = outRandomWalk[outRandomWalk.Count - 1];
            DataCell newCell = grid.GetNeighbourAtDirection(previousStep.cell, previousStep.direction);

            // get new random direction (direction of the previous cel is excluded)
            Direction previousCellDirection = GetInverseDirection(previousStep.direction);

            //cannot be null
            Direction newDirection = (Direction)grid.GetRandomNeighbourDirection(newCell, new Direction[] { previousCellDirection, allwaysPreventedDirection } );

            Step newStep = new Step(newCell, (Direction)newDirection);
            outRandomWalk.Add(newStep);
            visitedCells.Add(newCell);

            if (isLiveGenerationEnabled)
            {
                grid.RemoveWall(previousStep.cell, newStep.cell);
                yield return new WaitForSeconds(liveGenerationDelay);
            }
        }
    }
    
    //iterators cannot use out parameters -_-
    private IEnumerator GetRandomWalkCor (DataGrid grid, HashSet<DataCell> finalTreeCells, HashSet<DataCell> notInFinalTreeCells, List<Step> outRandomWalk)
    {
        Debug.Assert(outRandomWalk.Count == 0, $"{nameof(GetRandomWalkCor)} received not empty {nameof(outRandomWalk)}!");

        DataCell randomStartingCell = notInFinalTreeCells.ElementAt(Random.Range(0, notInFinalTreeCells.Count));
        Direction randomDirection = grid.GetRandomNeighbourDirection(randomStartingCell);

        outRandomWalk.Add(new Step(randomStartingCell, randomDirection));

        while (true)
        {
            Step previousStep = outRandomWalk[outRandomWalk.Count - 1];
            DataCell newCell = grid.GetNeighbourAtDirection(previousStep.cell, previousStep.direction);

            // get new random direction (direction of the previous cel is excluded)
            Direction previousCellDirection = GetInverseDirection(previousStep.direction);
            Direction? newDirection = grid.GetRandomNeighbourDirection(newCell, new Direction[] { previousCellDirection });
            
            bool foundLoop = false;

            // if the new step creates a loop in path, the loop is cut
            for (int i = 0; i < outRandomWalk.Count; i++)
            {
                if (outRandomWalk[i].cell == newCell)
                {
                    foundLoop = true;
                    for (int j = outRandomWalk.Count - 1; j > i; j--)
                    {

                        if (isLiveGenerationEnabled)
                            grid.BuildWall(outRandomWalk[j].cell, outRandomWalk[j - 1].cell);
                        outRandomWalk.RemoveAt(j);
                    }
                    if(newDirection != null)
                        outRandomWalk[i].direction = (Direction)newDirection;
                    break;
                }
            }

            //otherwise, the new step is added to randomwalk
            if (!foundLoop)
            {
                Debug.Assert(newDirection != null,"newDirection was null generating random walk, this shouldn't happen!");
                Step newStep = new Step(newCell, (Direction)newDirection);
                outRandomWalk.Add(newStep);

                if (isLiveGenerationEnabled)
                {
                    grid.RemoveWall(previousStep.cell, newStep.cell);
                    yield return new WaitForSeconds(liveGenerationDelay);
                }
                else if (MustRefreshScreen)
                    yield return coroutiner.StartCoroutine(RefreshScreenCor());
            }

            //if random walk reached final tree, the random walk can be returned
            if (finalTreeCells.Contains(newCell))
                break;
        }
    }
}
