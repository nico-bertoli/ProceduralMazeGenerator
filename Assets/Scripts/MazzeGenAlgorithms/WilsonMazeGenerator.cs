using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

//algorithm: https://weblog.jamisbuck.org/2011/1/20/maze-generation-wilson-s-algorithm.html

/// <summary>
/// Applies Wilson's Algorithm to the grid to generate a maze.
/// </summary>
public class WilsonMazeGenerator : AbsMazeGenerator {

    protected override IEnumerator GenerateMazeImpl(DataGrid _grid, DataCell _startCell) {

        HashSet<DataCell> finalTree = new HashSet<DataCell>();
        finalTree.Add(_startCell);

        //cells out of final tree
        HashSet<DataCell> outOfTree = new HashSet<DataCell>();
        for (int m = 0; m < _grid.Nrows; m++)
            for (int n = 0; n < _grid.Ncol; n++)
                outOfTree.Add(_grid.GetCell(m, n));

        outOfTree.Remove(_startCell);

        //while there are not connected cells...
        while (outOfTree.Count > 0) {

            //fin a random walk
            List<Step> rWalk = new List<Step>();
            yield return randomWalk(_grid, finalTree, outOfTree, rWalk);

            //add the final walk cells to the final tree (and remove them from out of tree set)
            for(int i = 0;i < rWalk.Count;i++){
                finalTree.Add(rWalk[i].cell);
                outOfTree.Remove(rWalk[i].cell);

                // optimization for non live generation: walls are edited only after a complete randomwalk is found
                if (!liveGeneration && i!= rWalk.Count-1)
                    _grid.RemoveWall(rWalk[i].cell, _grid.GetNeighbourAtDir(rWalk[i].cell, rWalk[i].direction));
            }
        }
    }

    /// <summary>
    /// Returns a randomwalk from the greed to the give final tree
    /// </summary>
    /// <param name="_grid"></param>
    /// <param name="_finalTree">Cells in final tree</param>
    /// <param name="_outOfTree">Cells out of tree</param>
    /// <param name="_resRandomWalk">Random walk list to edit</param>
    /// <returns></returns>
    private IEnumerator randomWalk(DataGrid _grid, HashSet<DataCell> _finalTree, HashSet<DataCell> _outOfTree, List<Step> _resRandomWalk) {

        //finding random cell out of the final tree
        DataCell randomCell = _outOfTree.ElementAt<DataCell>(Random.Range(0, _outOfTree.Count));

        //getting a random direction accessible from the cell
        DataCell.eDirection randomDir = _grid.GetRandomDirection(randomCell);

        //adding the step to the path
        _resRandomWalk.Add(new Step(randomCell, randomDir));

        while (true) {

            Step previousStep = _resRandomWalk[_resRandomWalk.Count - 1];
            //get new cell
            DataCell newCell = _grid.GetNeighbourAtDir(previousStep.cell, previousStep.direction);

            // get new random direction (direction of the previous cel is excluded, don't want to turn around)
            DataCell.eDirection? newDir;
            DataCell.eDirection impDir = DataCell.GetInverseDirection(previousStep.direction);
            newDir = _grid.GetRandomDirection(newCell, new DataCell.eDirection[] { impDir });
            
            bool foundLoop = false;

            //todo: this easy fix could be made better
            //don't care about null new direction cause if it happens there is no new step, to prevent errors i'm setting it to a random direction
            if (newDir == null) newDir = DataCell.eDirection.TOP;

            // if the new step creates a loop in path, the loop is cut
            for (int i = 0; i < _resRandomWalk.Count; i++) {
                if (_resRandomWalk[i].cell == newCell) {
                    foundLoop = true;
                    for (int j = _resRandomWalk.Count - 1; j > i; j--) {

                        if (liveGeneration)
                            _grid.BuildWall(_resRandomWalk[j].cell, _resRandomWalk[j - 1].cell);
                        _resRandomWalk.RemoveAt(j);
                    }
                    _resRandomWalk[i].direction = (DataCell.eDirection)newDir;
                    break;
                }
            }
            //otherwise, the new step is added to randomwalk
            if (!foundLoop) {
                Step newStep = new Step(newCell, (DataCell.eDirection)newDir);
                _resRandomWalk.Add(newStep);
                if (liveGeneration) {
                    _grid.RemoveWall(previousStep.cell, newStep.cell);
                    yield return new WaitForSeconds(liveGenerationDelay);
                }
            }

            //if random walk reached final tree, the random walk can be returned
            if (_finalTree.Contains(newCell))
                break;
        }
    }

    private class Step {
        public DataCell cell;
        public DataCell.eDirection direction;

        public Step(DataCell _cell, DataCell.eDirection _direction) {
            cell = _cell;
            direction = _direction;
        }
    }
}
