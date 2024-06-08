using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Creates a game object representation of a DataGrid
/// </summary>
public abstract class AbsGridObj : MonoBehaviour
{
    //======================================== fields
    /// <summary>
    /// called after grid generation ended
    /// </summary>
    public Action OnInitCompleted;

    [Header ("Settings")]
    [SerializeField] private float wallsStartingWidth = 0.4f;

    [Header("References")]
    [SerializeField] protected GameObject cellObjPrefab;
    [SerializeField] protected MeshCombiner meshCombiner;
    [SerializeField] private GameObject marginWallPrefab;
    private WallObj leftMargin;
    private WallObj bottomMargin;

    /// <summary>
    /// Maze map used to generate the maze
    /// </summary>
    protected DataGrid grid;
    protected CellObj[,] cellObjs;
    /// <summary>
    /// GameObj used to store mesh chunks
    /// </summary>
    private GameObject meshContainer;
    /// <summary>
    /// GameObj used to store chunks
    /// </summary>
    private GameObject chunksContainer;
    protected const int CHUNK_SIZE = 20;

    //======================================== methods
    /// <summary>
    /// Creates a gameobject representation the DataGrid received
    /// </summary>
    /// <param name="_grid"></param>
    /// <returns></returns>
    public virtual IEnumerator Init(DataGrid _grid) {
        grid = _grid;
        cellObjs = new CellObj[_grid.Nrows, _grid.Ncol];

        IReadOnlyList<GameObject> objs = ObjectsPreAllocator.Instance.GetObjects();

        for (int m = 0; m < _grid.Nrows; m++) {
            for (int n = 0; n < _grid.Ncol; n++) {
                cellObjs[m, n] = objs[m*_grid.Ncol + n].GetComponent<CellObj>();
                cellObjs[m, n].Init(grid.GetCell(m, n));
                cellObjs[m, n].transform.parent = transform;
            }
            yield return null;
        }
        yield return StartCoroutine(SetWallsWithd(wallsStartingWidth));
        InitMargins();
    }

    /// <summary>
    /// Enables/disables all cell game objects
    /// </summary>
    /// <param name="_active"></param>
    /// <returns></returns>
    protected IEnumerator setCellsActive(bool _active) {
        for (int m = 0; m < grid.Nrows; m++) {
            for (int n = 0; n < grid.Ncol; n++) {
                cellObjs[m, n].gameObject.SetActive(true);
            }
            yield return null;
        }
    }

    /// <summary>
    /// Divides cells in chunks and combines their meshes
    /// </summary>
    /// <returns></returns>
    public IEnumerator CombineMeshes() {
        resetMeshesContainer();
        yield return StartCoroutine(setCellsActive(true));

        if (chunksContainer == null) {
            initChunksContainer();
            yield return generateChunks();
        }

        for (int i = 0; i < chunksContainer.transform.childCount; i++) {
            GameObject chunk = chunksContainer.transform.GetChild(i).gameObject;
            GameObject combinedMeshes = meshCombiner.CombineMeshes(chunk);
            combinedMeshes.name = ("Mesh");
            combinedMeshes.transform.parent = meshContainer.transform;
            yield return null;
        }
        SetWallsMeshActive(false);
    }

    /// <summary>
    /// Enables cells distance culling respect a given target
    /// </summary>
    /// <param name="_target">target of the distance culling</param>
    /// <returns></returns>
    public IEnumerator EnableCulling(GameObject _target) {
        DistanceCulling.target = _target;
        for (int i = 0; i < chunksContainer.transform.childCount; i++) {
            chunksContainer.transform.GetChild(i).gameObject.AddComponent<DistanceCulling>().Init();
            if (i % 10 == 0) yield return null;
        }
        for (int i = 0; i < meshContainer.transform.childCount; i++) {
            meshContainer.transform.GetChild(i).gameObject.AddComponent<DistanceCulling>().Init();
            if (i % 10 == 0) yield return null;
        }

    }

    /// <summary>
    /// Enables/disables all wall meshes
    /// </summary>
    /// <param name="_active"></param>
    public void SetWallsMeshActive(bool _active) {
        for (int m = 0; m < grid.Nrows; m++) {
            for (int n = 0; n < grid.Ncol; n++) {
                cellObjs[m, n].SetWallMeshesActive(_active);
            }
        }
    }

    /// <summary>
    /// Sets width of all walls
    /// </summary>
    /// <param name="_widht"></param>
    /// <returns></returns>
    public IEnumerator SetWallsWithd(float _widht) {

        for (int m = 0; m < grid.Nrows; m++) {
            for (int n = 0; n < grid.Ncol; n++) {
                cellObjs[m, n].SetWallsWidht(_widht);
            }
            yield return null;
        }
        if (leftMargin && bottomMargin) {
            leftMargin.SetWidth(_widht);
            bottomMargin.SetWidth(_widht);
            leftMargin.SetLength(grid.Nrows);
            bottomMargin.SetLength(grid.Ncol);
        }
    }

    /// <summary>
    /// Returns bottom right cell position
    /// </summary>
    public Vector3 GetBottomRightCellPos() {
        return new Vector3(transform.position.x + grid.Ncol - 1, transform.position.y, transform.position.z - grid.Nrows + 1);
    }

    /// <summary>
    /// Returns entrance position in world space
    /// </summary>
    /// <returns></returns>
    public Vector3 GetEntrancePosition() {
        return transform.position;
    }

    /// <summary>
    /// Initializes grid margins
    /// </summary>
    protected void InitMargins() {
        leftMargin = Instantiate(marginWallPrefab).GetComponent<WallObj>();
        bottomMargin = Instantiate(marginWallPrefab).GetOrAddComponent<WallObj>();

        leftMargin.gameObject.name = "Left margin";
        bottomMargin.gameObject.name = "Bottom margin";

        leftMargin.gameObject.transform.position = new Vector3(
            -0.5f,
            transform.position.y,
            -grid.Nrows / 2f + 0.5f);

        bottomMargin.gameObject.transform.position = new Vector3(
            grid.Ncol / 2f - 0.5f,
            transform.position.y,
            -grid.Nrows + 0.5f);

        bottomMargin.SetWidth(wallsStartingWidth);
        leftMargin.SetWidth(wallsStartingWidth);
        bottomMargin.SetLength(grid.Ncol);
        leftMargin.SetLength(grid.Nrows);

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
    private IEnumerator generateChunks() {
        
        GameObject chunk;
        for (int ychunk = 0; ychunk <= grid.Nrows / CHUNK_SIZE; ychunk++) {
            for (int xchunk = 0; xchunk <= grid.Ncol / CHUNK_SIZE; xchunk++) {
                chunk = new GameObject("Chunk[" + ychunk + "," + xchunk + "]");
                chunk.transform.parent = chunksContainer.transform;
                chunk.transform.position = new Vector3(xchunk * CHUNK_SIZE +CHUNK_SIZE/ 2f, 0, -(ychunk* CHUNK_SIZE + CHUNK_SIZE/ 2f));

                for (int mchunk = 0; mchunk < CHUNK_SIZE; mchunk++) {
                    for (int nchunk = 0; nchunk < CHUNK_SIZE; nchunk++) {

                        int m = ychunk * CHUNK_SIZE + mchunk;
                        int n = xchunk * CHUNK_SIZE + nchunk;

                        if (m >= grid.Nrows || n >= grid.Ncol) break;
                        cellObjs[m, n].transform.parent = chunk.transform;
                    }
                }
                chunk.SetActive(false);
                //chunk.transform.parent = chunksContainer.transform;
            }
            yield return null;
        }
    }

    private void OnDestroy() {
        if (ObjectsPreAllocator.Instance) ObjectsPreAllocator.Instance.RelaeaseAll();
    }

    private void initChunksContainer() {
        chunksContainer = new GameObject();
        chunksContainer.name = "Chunks container";
        chunksContainer.transform.parent = transform;
    }

    private void resetMeshesContainer() {
        if (meshContainer != null)
            Destroy(meshContainer);

        meshContainer = new GameObject();
        meshContainer.transform.parent = transform;
        meshContainer.name = "Chunk meshes container";
    }
}
