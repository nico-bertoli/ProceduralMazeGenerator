using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Maze : MonoBehaviour {

    /// <summary>
    /// Grid that shows live generation (slower)
    /// </summary>
    [SerializeField] GameObject editableGridPref;
    /// <summary>
    /// Grid that hides live generation (faster)
    /// </summary>
    [SerializeField] GameObject nonEditableGridPref;

    AbsGridObj gridObj;
    DataGrid dataGrid;
    AbsMazeGenerator mazeGenerator;

    /// <summary>
    /// called after maze generation is totally completed
    /// </summary>
    public Action OnGenerationComplete;
    ///// <summary>
    ///// called after grid generation is completed
    /// </summary>
    public Action OnGridInitComplete;
    /// <summary>
    /// Indicates if the user decided to show live generation
    /// </summary>
    private bool showGeneration;

    public IEnumerator Generate(int _nRows, int _nColumns, bool _showGeneration, AbsMazeGenerator.eAlgorithms _algorithm) {

        showGeneration = _showGeneration;

        yield return StartCoroutine(instantiateGridObj(_nRows,_nColumns));

        instantiateMazeGenerator(_algorithm);
        
        yield return StartCoroutine(mazeGenerator.GenerateMaze(dataGrid, dataGrid.GetCell(0, 0), _showGeneration));

        ShowGrid();
    }

    public void SetLiveGenerationSpeed(float _speed) {
        if (mazeGenerator)
            mazeGenerator.SetLiveGenerationSpeed(_speed);
    }

    public void Reset() {
        if (mazeGenerator) Destroy(mazeGenerator.gameObject);
        if (gridObj) Destroy(gridObj.gameObject);
    }

    public IEnumerator SetWallsSize(float _size) {
        yield return StartCoroutine(gridObj.SetWallsWithd(_size));
        yield return StartCoroutine(gridObj.CombineMeshes());
    }

    public Vector3 GetExitPosition() {
        return gridObj.GetBottomRightCellPos();
    }

    public IEnumerator EnableCooling(GameObject _coolingObject) {
        yield return StartCoroutine(gridObj.EnableCulling(_coolingObject));
    }

    private void ShowGrid() {
        if (!showGeneration) {
            gridObj.OnInitCompleted += OnGenerationComplete;
            UIManager.Instance.SetLoadingPanelText("Loading maze");
            StartCoroutine(gridObj.Init(dataGrid));
        }

        else {
            StartCoroutine(gridObj.CombineMeshes());
            OnGenerationComplete();
        }

    }

    private void instantiateMazeGenerator(AbsDfsMazeGenerator.eAlgorithms _algorithm) {
        switch (_algorithm) {
            case AbsMazeGenerator.eAlgorithms.DFSiterative:
                mazeGenerator = new GameObject().AddComponent<DFSIterMazeGenerator>();
                mazeGenerator.gameObject.name = "DFSIter Maze Generator";
                break;
            case AbsMazeGenerator.eAlgorithms.Willson:
                mazeGenerator = new GameObject().AddComponent<WilsonMazeGenerator>();
                mazeGenerator.gameObject.name = "Wilson Maze Generator";
                break;
            case AbsMazeGenerator.eAlgorithms.Kruskal:
                mazeGenerator = new GameObject().AddComponent<KruskalMazeGenerator>();
                mazeGenerator.gameObject.name = "Kruskal Maze Generator";
                break;
        }
    }

    private IEnumerator instantiateGridObj(int _nRows,int _nColumns) {
        if (showGeneration)
            gridObj = Instantiate(editableGridPref).GetComponent<EditableGridObj>();
        else
            gridObj = Instantiate(nonEditableGridPref).GetComponent<NonEditableGridObj>();

        dataGrid = new DataGrid(_nRows, _nColumns);

        gridObj.OnInitCompleted += OnGridInitComplete;

        if (showGeneration)
            yield return StartCoroutine(gridObj.Init(dataGrid));
    }
}
