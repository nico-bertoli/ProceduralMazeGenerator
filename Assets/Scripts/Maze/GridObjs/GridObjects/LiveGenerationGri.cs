using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Game Object representing a DataGrid
/// </summary>
public class LiveGenerationGri : MonoBehaviour
{
    public Action OnGridChunksGenerated;

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

    public IEnumerator Init(DataGrid grid) {
        
        Reset();
        dataGrid = grid;
        cellObjs = new CellObject[grid.RowsCount, grid.ColumnsCount];

        for (int m = 0; m < grid.RowsCount; m++) {
            for (int n = 0; n < grid.ColumnsCount; n++) {
                CellObject cellObject = Instantiate(cellObjectPrefab, transform).GetComponent<CellObject>();
                cellObjs[m, n] = cellObject;
                cellObjs[m, n].Init(dataGrid.GetCell(m, n));
            }
            yield return null;
        }
        yield return StartCoroutine(SetWallsWidth(wallsStartingWidth));
        InitMargins();
        
        yield return StartCoroutine(SetCellsActive(true));
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
    
    // public IEnumerator GenerateChunks() {
    //     
    //     ResetMeshesContainer();
    //     yield return StartCoroutine(SetCellsActive(true));
    //
    //     if (chunksContainer == null) {
    //         InitChunksContainer();
    //         yield return GenerateChunks(chunksContainer);
    //     }
    //
    //     for (int i = 0; i < chunksContainer.transform.childCount; i++) {
    //         GameObject chunk = chunksContainer.transform.GetChild(i).gameObject;
    //         GameObject combinedMeshes = meshesCombiner.CombineMeshes(chunk);
    //         combinedMeshes.name = ("Mesh");
    //         combinedMeshes.transform.parent = meshesContainer.transform;
    //         yield return null;
    //     }
    //     SetWallsMeshesActive(false);
    //     
    //     OnGridChunksGenerated?.Invoke();
    // }

    public IEnumerator SetWallsWidth(float width) {

        for (int m = 0; m < dataGrid.RowsCount; m++) {
            for (int n = 0; n < dataGrid.ColumnsCount; n++) {
                cellObjs[m, n].SetWallsWidth(width);
            }
            yield return null;
        }
        if (leftMargin && bottomMargin) {
            leftMargin.SetWidth(width);
            bottomMargin.SetWidth(width);
            leftMargin.SetLength(dataGrid.RowsCount);
            bottomMargin.SetLength(dataGrid.ColumnsCount);
        }
    }
    
    public Vector3 GetBottomRightCellPos() {
        return new Vector3(
            transform.position.x + dataGrid.ColumnsCount - 1, 
            transform.position.y, 
            transform.position.z - dataGrid.RowsCount + 1
            );
    }

    #endregion Public Methods
    #region ========================================================================================== Protected Methods
    
    protected IEnumerator SetCellsActive(bool setActive) {
        for (int m = 0; m < dataGrid.RowsCount; m++) {
            for (int n = 0; n < dataGrid.ColumnsCount; n++) {
                cellObjs[m, n].gameObject.SetActive(setActive);
            }
            yield return null;
        }
    }
    
    protected void SetWallsMeshesActive(bool setActive) {
        for (int m = 0; m < dataGrid.RowsCount; m++) {
            for (int n = 0; n < dataGrid.ColumnsCount; n++) {
                cellObjs[m, n].SetWallMeshesActive(setActive);
            }
        }
    }
    
    #endregion Protected Methods
    #region ============================================================================================ Private Methods

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

    /// <summary>
    /// Separates cells in chunks, depending on CHUNK_SIZE
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateChunks(GameObject chunksParent)
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
            yield return null;
        }
    }

    private void InitChunksContainer() {
        chunksContainer = new GameObject();
        chunksContainer.name = "Chunks container";
        chunksContainer.transform.parent = transform;
    }

    private void ResetMeshesContainer() {
        if (meshesContainer != null)
            Destroy(meshesContainer);

        meshesContainer = new GameObject();
        meshesContainer.transform.parent = transform;
        meshesContainer.name = "Chunk meshes container";
    }
    
    #endregion Private Methods
}
