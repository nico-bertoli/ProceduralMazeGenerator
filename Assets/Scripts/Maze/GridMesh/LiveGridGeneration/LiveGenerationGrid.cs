using System;
using UnityEngine;

/// <summary>
/// Game Object representing a DataGrid
/// </summary>
public class LiveGenerationGrid : MonoBehaviour
{
    #region ============================================================================================= Private Fields
    
    [Header("References")]
    [SerializeField] private CellObject cellObjectPrefab;
    [SerializeField] private MarginWallsGenerator marginWallsGenerator;


    private DataGrid dataGrid;
    private CellObject[,] cellObjs;
    private GameObject meshesContainer;
    private GameObject chunksContainer;
    
    private float liveGenWallsWidth => Settings.Instance.mazeGenerationSettings.LiveGenerationWallsWidth;
    
    #endregion Private Fields
    #region ============================================================================================= Public Methods
    
    public void Init(DataGrid grid) {
        
        Reset();
        dataGrid = grid;
        cellObjs = new CellObject[grid.RowsCount, grid.ColumnsCount];

        for (int m = 0; m < grid.RowsCount; m++) {
            for (int n = 0; n < grid.ColumnsCount; n++) {
                CellObject cellObject = Instantiate(cellObjectPrefab, transform).GetComponent<CellObject>();
                cellObjs[m, n] = cellObject;
                cellObjs[m, n].Init(dataGrid.GetCell(m, n));
            }
        }
        SetWallsWidth(liveGenWallsWidth);
        marginWallsGenerator.InitMargins(dataGrid,liveGenWallsWidth);
        
        SetCellsActive(true);
        SetWallsMeshesActive(true);
    }

    public void SetWallsWidth(float width) {

        for (int m = 0; m < dataGrid.RowsCount; m++) {
            for (int n = 0; n < dataGrid.ColumnsCount; n++) {
                cellObjs[m, n].SetWallsWidth(width);
            }
        }

        marginWallsGenerator.SetWallsWidth(dataGrid, width);
    }

    #endregion Public Methods
    #region ============================================================================================ Private Methods
    
    private void Start()
    {
        GameController.Instance.OnGameModeActive += () => SetWallsWidth(Settings.Instance.mazeGenerationSettings.IngameWallsWidth);
    }
    
    private void Reset()
    {
        if (cellObjs == null)
            return; 
        
        foreach (CellObject cell in cellObjs)
            Destroy(cell.gameObject);

        marginWallsGenerator.Reset();
    }
    
    private void SetCellsActive(bool setActive) {
        for (int m = 0; m < dataGrid.RowsCount; m++) 
            for (int n = 0; n < dataGrid.ColumnsCount; n++) 
                cellObjs[m, n].gameObject.SetActive(setActive);
    }
    
    private void SetWallsMeshesActive(bool setActive) {
        for (int m = 0; m < dataGrid.RowsCount; m++)
            for (int n = 0; n < dataGrid.ColumnsCount; n++) 
                cellObjs[m, n].SetWallMeshesActive(setActive);
    }
    #endregion Private Methods
}
