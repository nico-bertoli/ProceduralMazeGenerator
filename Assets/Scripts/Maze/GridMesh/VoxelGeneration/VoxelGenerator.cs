using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CubeMeshDataGenerator;
using static GridDirections;

// : https://www.youtube.com/watch?v=b_1ZlHrJZc4&list=PL5KbKbJ6Gf9-d303Lk8TGKCW-t5JsBdtB&index=10

public class VoxelGenerator : MonoBehaviour
{
    public event Action OnMeshGenerated;
    
    #region ============================================================================================= Private Fields

    [Header("References")] 
    [SerializeField] private VoxelChunk voxelChunkPrototype;
    [SerializeField] private MarginWallsHandler marginWallsGenerator;
    
    [Header("Settings")]
    [SerializeField] private Material material;

    private List<VoxelChunk> chunks = new ();
    private Coroutine generationCor;

    #endregion Private Fields
    #region ========================================================================================= Private Properties

    private float wallsWidth => Settings.Instance.MazeSettings.VoxelWallsWidth;
    private float wallsHeight => Settings.Instance.MazeSettings.WallsHeight;
    private int chunkSize => Settings.Instance.MazeGenerationSettings.NumberOfCellsComposingChunk;

    #endregion Private Properties
    #region ============================================================================================ Pucblic Methods

    public void GenerateGridMesh(DataGrid dataGrid) => generationCor = StartCoroutine(GenerateGridMeshCor(dataGrid));
    
    public void Reset()
    {
        if(generationCor != null)
        {
            StopCoroutine(generationCor);
            generationCor = null;
        }

        foreach (VoxelChunk chunk in chunks)
            Destroy(chunk.gameObject);
        
        chunks.Clear();
        
        marginWallsGenerator.EnableMargins(false);
    }

    #endregion Public Methods
    #region ============================================================================================ Private Methods

    private IEnumerator GenerateGridMeshCor(DataGrid dataGrid)
    {
        int mChunksCount = Mathf.CeilToInt(dataGrid.RowsCount / (float)chunkSize);
        int nChunksCount = Mathf.CeilToInt(dataGrid.ColumnsCount / (float)chunkSize);

        for (int m = 0; m < mChunksCount; m++)
        {
            for (int n = 0; n < nChunksCount; n++)
            {
                chunks.Add(CreateChunk(dataGrid, m * chunkSize, n * chunkSize, chunkSize));
                yield return null;
            }
        }
        
        marginWallsGenerator.InitMargins(dataGrid,wallsWidth);
        OnMeshGenerated?.Invoke();
    }
    
    private VoxelChunk CreateChunk(DataGrid dataGrid, int mBegin, int nBegin, int chunkSize)
    {
        MeshData meshData = MeshData.CreateEmpty();

        Vector3 topWallScale = new Vector3(1f + wallsWidth, wallsHeight, wallsWidth);
        Vector3 rightWallScale = new Vector3(wallsWidth, wallsHeight, 1f + wallsWidth);

        float wallsOffsetFromCenter = 0.5f;
        
        for (int m = mBegin; m < mBegin + chunkSize && m < dataGrid.RowsCount; m++)
        {
            for (int n = nBegin; n < nBegin + chunkSize && n < dataGrid.ColumnsCount; n++)
            {
                DataCell cell = dataGrid.GetCell(m, n);

                if (cell.IsTopWallActive)
                {
                    List<CubeFaceDirection> notVisibleFaces = GetNotVisibleFaces(dataGrid, m, n, true);
                    Vector3 topWallPos = new Vector3(cell.PosN - wallsOffsetFromCenter, 0, -cell.PosM);
                    GetCubeMeshData(topWallScale * 0.5f, topWallPos, notVisibleFaces, ref meshData);
                }
                if (cell.IsRightWallActive)
                {
                    List<CubeFaceDirection> notVisibleFaces = GetNotVisibleFaces(dataGrid, m, n, false);
                    Vector3 rightWallPos = new Vector3(cell.PosN, 0, -cell.PosM - wallsOffsetFromCenter);
                    GetCubeMeshData(rightWallScale * 0.5f, rightWallPos, notVisibleFaces, ref meshData);
                }
            }
        }

        Debug.Assert(meshData.Vertices != null, "Vertices list is null!");
        Debug.Assert(meshData.Triangles != null, "Triangles list is null!");
        Debug.Assert(meshData.Vertices.Count > 0, "Vertices list is empty!");
        Debug.Assert(meshData.Triangles.Count > 0, "Triangles list is empty!");

        VoxelChunk chunk = Instantiate(voxelChunkPrototype,transform).GetComponent<VoxelChunk>();
        chunk.Init(meshData,material);
        chunk.gameObject.isStatic = true;
        return chunk;
    }
    
    private List<CubeFaceDirection> GetNotVisibleFaces(DataGrid dataGrid, int m, int n, bool siHorizzontalWall)
    {
        //bottom face is never visible
        List<CubeFaceDirection> notVisibleFaces = new List<CubeFaceDirection>() { CubeFaceDirection.Bottom };
        DataCell cell = dataGrid.GetCell(m, n);

        if (siHorizzontalWall)
        {
            DataCell rightCell = dataGrid.GetNeighbourAtDirection(cell, Directions.Right);
            if(rightCell !=null && rightCell.IsTopWallActive)
                notVisibleFaces.Add(CubeFaceDirection.Right);
            
            DataCell leftCell = dataGrid.GetNeighbourAtDirection(cell, Directions.Left);
            if(leftCell !=null && leftCell.IsTopWallActive)
                notVisibleFaces.Add(CubeFaceDirection.Left);
        }
        else //vertical wall
        {
            DataCell bottomCell = dataGrid.GetNeighbourAtDirection(cell, Directions.Down);
            if(bottomCell !=null && bottomCell.IsRightWallActive)
                notVisibleFaces.Add(CubeFaceDirection.Back);
            
            DataCell forwardCell = dataGrid.GetNeighbourAtDirection(cell, Directions.Up);
            if(forwardCell !=null && forwardCell.IsRightWallActive)
                notVisibleFaces.Add(CubeFaceDirection.Forward);  
        }
        
        return notVisibleFaces;
    }
    #endregion Private Methods
}


