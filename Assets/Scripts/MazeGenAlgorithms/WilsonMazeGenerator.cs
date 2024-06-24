using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DataGrid;

//algorithm: https://weblog.jamisbuck.org/2011/1/20/maze-generation-wilson-s-algorithm.html

public class WilsonMazeGenerator : AbsMazeGenerator {
    
    private class Step {
        public DataCell cell;
        public eDirection direction;

        public Step(DataCell _cell, eDirection _direction) {
            cell = _cell;
            direction = _direction;
        }
    }

    protected override IEnumerator GenerateMazeImplementation(DataGrid grid, DataCell startCell) {

        HashSet<DataCell> finalTree = new HashSet<DataCell>();
        finalTree.Add(startCell);

        //cells out of final tree
        HashSet<DataCell> outOfTree = new HashSet<DataCell>();
        for (int m = 0; m < grid.RowsCount; m++) {
            for (int n = 0; n < grid.ColumnsCount; n++) {
                if(grid.GetCell(m,n).Equals(startCell) == false)
                    outOfTree.Add(grid.GetCell(m, n));
            }
        }

        //while there are not connected cells...
        while (outOfTree.Count > 0) {

            //fin a random walk
            List<Step> rWalk = new List<Step>();
            yield return RandomWalk(grid, finalTree, outOfTree, rWalk);

            //add random walk cells to the final tree (and remove them from out of tree set)
            for(int i = 0;i < rWalk.Count;i++){
                finalTree.Add(rWalk[i].cell);
                outOfTree.Remove(rWalk[i].cell);

                // optimization for non live generation: walls are edited only after a complete random walk is found
                if (!isLiveGenerationEnabled && i!= rWalk.Count-1)
                    grid.RemoveWall(rWalk[i].cell, grid.GetNeighbourAtDir(rWalk[i].cell, rWalk[i].direction));
            }
        }
    }

    /// <summary>
    /// Returns a random walk from the greed to the give final tree
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="finalTree">Cells in final tree</param>
    /// <param name="outOfTree">Cells out of tree</param>
    /// <param name="resRandomWalk">Random walk list to edit</param>
    /// <returns></returns>
    private IEnumerator RandomWalk(DataGrid grid, HashSet<DataCell> finalTree, HashSet<DataCell> outOfTree, List<Step> resRandomWalk) {

        //finding random cell out of the final tree
        DataCell randomCell = outOfTree.ElementAt(Random.Range(0, outOfTree.Count));

        //getting a random direction accessible from the cell
        List<eDirection> possibleDirections = grid.GetNeighboursDirections(randomCell);
        eDirection randomDir = possibleDirections[Random.Range(0, possibleDirections.Count)];

        //adding the step to the path
        resRandomWalk.Add(new Step(randomCell, randomDir));

        while (true) {

            Step previousStep = resRandomWalk[resRandomWalk.Count - 1];
            DataCell newCell = grid.GetNeighbourAtDir(previousStep.cell, previousStep.direction);

            // get new random direction (direction of the previous cel is excluded)
            eDirection preventedDirection = GetInverseDirection(previousStep.direction);
            eDirection? newDirection = grid.GetRandomNeighbourDirection(newCell, new eDirection[] { preventedDirection });
            
            bool foundLoop = false;

            // if the new step creates a loop in path, the loop is cut
            for (int i = 0; i < resRandomWalk.Count; i++) {
                if (resRandomWalk[i].cell == newCell) {
                    foundLoop = true;
                    for (int j = resRandomWalk.Count - 1; j > i; j--) {

                        if (isLiveGenerationEnabled)
                            grid.BuildWall(resRandomWalk[j].cell, resRandomWalk[j - 1].cell);
                        resRandomWalk.RemoveAt(j);
                    }
                    if(newDirection != null)
                        resRandomWalk[i].direction = (eDirection)newDirection;
                    break;
                }
            }
            //otherwise, the new step is added to randomwalk
            if (!foundLoop) {
                Debug.Assert(newDirection != null,"newDirection was null generating random walk, this shouldn't happen!");
                Step newStep = new Step(newCell, (eDirection)newDirection);
                resRandomWalk.Add(newStep);
                if (isLiveGenerationEnabled) {
                    grid.RemoveWall(previousStep.cell, newStep.cell);
                    yield return new WaitForSeconds(liveGenerationDelay);
                }
            }

            //if random walk reached final tree, the random walk can be returned
            if (finalTree.Contains(newCell))
                break;
        }
    }
}
