using UnityEngine;

/// <summary>
/// - Used to show live generation
/// - When escape phase starts, this is deleted and a more efficient maze is generated using voxel generator
/// </summary>
public class LiveGenerationGrid : MonoBehaviour
{
    #region ============================================================================================= Private Fields
    
    [Header("References")]
    [SerializeField] private CellObject cellObjectPrefab;
    [SerializeField] private MarginWallsGenerator marginWallsGenerator;
    [SerializeField] private PoolSpawner cellsPoolSpawner;

    private DataGrid dataGrid;
    private CellObject[,] cellObjs;
    
    private float liveGenWallsWidth => Settings.Instance.MazeGenerationSettings.LiveGenerationWallsWidth;
    
    #endregion Private Fields
    #region ============================================================================================= Public Methods
    
    public void Init(DataGrid grid)
    {
        dataGrid = grid;
        cellObjs = new CellObject[grid.RowsCount, grid.ColumnsCount];

        for (int m = 0; m < grid.RowsCount; m++)
        {
            for (int n = 0; n < grid.ColumnsCount; n++)
            {
                //todo use pooling instad of Init
                CellObject cellObject = cellsPoolSpawner.GetItem().GetComponent<CellObject>();
                cellObjs[m, n] = cellObject;
                cellObjs[m, n].Init(dataGrid.GetCell(m, n));
            }
        }

        SetWallsWidth(liveGenWallsWidth);
        marginWallsGenerator.InitMargins(dataGrid,liveGenWallsWidth);
    }
    
    public void Reset()
    {
        if (cellObjs == null)
            return;

        foreach (CellObject cell in cellObjs)
            cellsPoolSpawner.ReturnItem(cell.gameObject);

        cellObjs = null;
        marginWallsGenerator.Reset();
    }

    #endregion Public Methods
    #region ============================================================================================ Private Methods

    private void SetWallsWidth(float width)
    {
        for (int m = 0; m < dataGrid.RowsCount; m++)
            for (int n = 0; n < dataGrid.ColumnsCount; n++)
                cellObjs[m, n].SetWallsWidth(width);

        marginWallsGenerator.SetWallsWidth(dataGrid, width);
    }
   
    #endregion Private Methods
}
