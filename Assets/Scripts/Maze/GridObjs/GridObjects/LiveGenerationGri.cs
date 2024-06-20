using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Game Object representing a DataGrid
/// </summary>
public class LiveGenerationGri : MonoBehaviour
{
    #region ============================================================================================= Private Fields

    [Header ("Settings")]
    [SerializeField] private float wallsStartingWidth = 0.4f;
    
    [Header("References")]
    [SerializeField] protected MeshesCombiner meshesCombiner;
    [SerializeField] private GameObject marginWallPrefab;
    [SerializeField] private CellObject cellObjectPrefab;
    
    private const int CHUNK_SIZE = 20;
    
    private WallObj leftMargin;
    private WallObj bottomMargin;
    private DataGrid dataGrid;
    private CellObject[,] cellObjs;
    private GameObject meshesContainer;
    private GameObject chunksContainer;
    
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
        SetWallsWidth(wallsStartingWidth);
        InitMargins();
        
        SetCellsActive(true);
        SetWallsMeshesActive(true);
    }
    
    private void Reset()
    {
        if (cellObjs == null)
            return; 
        
        foreach (CellObject cell in cellObjs)
            Destroy(cell.gameObject);
        
        if(leftMargin != null)
            Destroy(leftMargin.gameObject);
        
        if(bottomMargin != null)
            Destroy(bottomMargin.gameObject);
    }

    public void SetWallsWidth(float width) {

        for (int m = 0; m < dataGrid.RowsCount; m++) {
            for (int n = 0; n < dataGrid.ColumnsCount; n++) {
                cellObjs[m, n].SetWallsWidth(width);
            }
        }
        if (leftMargin && bottomMargin) {
            leftMargin.SetWidth(width);
            bottomMargin.SetWidth(width);
            leftMargin.SetLength(dataGrid.RowsCount);
            bottomMargin.SetLength(dataGrid.ColumnsCount);
        }
    }

    #endregion Public Methods
    #region ============================================================================================ Private Methods

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
    
    private void InitMargins() {
        leftMargin = Instantiate(marginWallPrefab).GetComponent<WallObj>();
        bottomMargin = Instantiate(marginWallPrefab).GetOrAddComponent<WallObj>();

        leftMargin.gameObject.name = "Left margin";
        bottomMargin.gameObject.name = "Bottom margin";

        leftMargin.gameObject.transform.position = new Vector3(
            -0.5f,
            transform.position.y,
            -dataGrid.RowsCount / 2f + 0.5f);

        bottomMargin.gameObject.transform.position = new Vector3(
            dataGrid.ColumnsCount / 2f - 0.5f,
            transform.position.y,
            -dataGrid.RowsCount + 0.5f);

        bottomMargin.SetWidth(wallsStartingWidth);
        leftMargin.SetWidth(wallsStartingWidth);
        bottomMargin.SetLength(dataGrid.ColumnsCount);
        leftMargin.SetLength(dataGrid.RowsCount);

        leftMargin.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

        leftMargin.transform.parent = bottomMargin.transform.parent = transform;
        bottomMargin.gameObject.SetActive(true);
        leftMargin.gameObject.SetActive(true);
        leftMargin.SetMeshActive(true);
        bottomMargin.SetMeshActive(true);
    }

    //todo copy this for mesh chunks
    private void GenerateChunks(GameObject chunksParent)
    {
        int ChunksCountM = dataGrid.RowsCount / CHUNK_SIZE;
        int ChunksCountN = dataGrid.ColumnsCount / CHUNK_SIZE;
        
        for (int gridM = 0; gridM <= ChunksCountM; gridM++) {
            for (int grinN = 0; grinN <= ChunksCountN; grinN++) {
                
                //create new chunk
                GameObject chunk = new GameObject("Chunk[" + gridM + "," + grinN + "]");
                chunk.transform.parent = chunksParent.transform;
                chunk.transform.position = new Vector3(grinN * CHUNK_SIZE +CHUNK_SIZE/ 2f, 0, -(gridM* CHUNK_SIZE + CHUNK_SIZE/ 2f));
                chunk.isStatic = true;

                //make cells child of new chunk
                for (int chunkM = 0; chunkM < CHUNK_SIZE; chunkM++) {
                    for (int chunkN = 0; chunkN < CHUNK_SIZE; chunkN++) {

                        int m = gridM * CHUNK_SIZE + chunkM;
                        int n = grinN * CHUNK_SIZE + chunkN;

                        if (m >= dataGrid.RowsCount || n >= dataGrid.ColumnsCount) break;
                        cellObjs[m, n].transform.parent = chunk.transform;
                    }
                }
                chunk.SetActive(false);
            }
        }
    }

    #endregion Private Methods
}
