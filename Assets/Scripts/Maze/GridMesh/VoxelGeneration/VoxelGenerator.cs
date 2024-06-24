using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CubeMeshDataGenerator;

// : https://www.youtube.com/watch?v=b_1ZlHrJZc4&list=PL5KbKbJ6Gf9-d303Lk8TGKCW-t5JsBdtB&index=10

public class VoxelGenerator : MonoBehaviour
{
    public Action OnMeshGenerated;
    
    #region ============================================================================================= Private Fields

    [Header("References")] 
    [SerializeField] private VoxelChunk voxelChunkPrototype;
    [SerializeField] private MarginWallsGenerator marginWallsGenerator;
    
    [Header("Settings")]
    [SerializeField] private Material material;

    private List<VoxelChunk> chunks = new ();
    
    #endregion Private Fields
    #region ========================================================================================= Private Properties
    private float wallsWidth => Settings.Instance.mazeGenerationSettings.InGameWallsWidth;
    private float wallsHeight => Settings.Instance.mazeGenerationSettings.WallsHeight;
    private int chunkSize => Settings.Instance.mazeGenerationSettings.VoxelChunkSize;

    #endregion Private Properties
    #region ============================================================================================ Pucblic Methods

    public void CreateGrid(DataGrid dataGrid) => StartCoroutine(CreateGridCor(dataGrid));
    
    public void Reset()
    {
        foreach (VoxelChunk chunk in chunks)
            Destroy(chunk.gameObject);
        
        chunks.Clear();
        
        marginWallsGenerator.Reset();
    }

    #endregion Public Methods
    #region ============================================================================================ Private Methods

    private IEnumerator CreateGridCor(DataGrid dataGrid)
    {
        int mChunksCount = Mathf.CeilToInt(dataGrid.RowsCount / chunkSize) +1;
        int nChunksCount = Mathf.CeilToInt(dataGrid.ColumnsCount / chunkSize) +1;

        for (int m = 0; m < mChunksCount; m++)
        for (int n = 0; n < nChunksCount; n++)
        {
            chunks.Add(CreateChunk(dataGrid,m*chunkSize,n*chunkSize,chunkSize));
            yield return null;
        }
        marginWallsGenerator.InitMargins(dataGrid,wallsWidth);
        OnMeshGenerated?.Invoke();
    }
    
    private VoxelChunk CreateChunk(DataGrid dataGrid, int mBegin, int nBegin, int chunkSize)
    {
        // create voxel
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

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
                    List<CubeFaceDirection> dontCreateFaces = GetNotVisibleFaces(dataGrid,m,n,true);
                    Vector3 topWallPos = new Vector3(cell.PosN - wallsOffsetFromCenter, 0, -cell.PosM);
                    CubeMeshDataGenerator.GetMeshData(vertices,triangles,topWallScale * 0.5f,topWallPos,dontCreateFaces);
                }
                if (cell.IsRightWallActive)
                {
                    List<CubeFaceDirection> preventCreationFaces = GetNotVisibleFaces(dataGrid,m,n,false);
                    Vector3 rightWallPos = new Vector3(cell.PosN, 0, -cell.PosM - wallsOffsetFromCenter);
                    CubeMeshDataGenerator.GetMeshData(vertices,triangles,rightWallScale * 0.5f,rightWallPos,preventCreationFaces);
                }
            }
        }
        
        VoxelChunk chunk = Instantiate(voxelChunkPrototype,transform).GetComponent<VoxelChunk>();
        chunk.Init(vertices,triangles,material);
        chunk.gameObject.isStatic = true;
        return chunk;
    }
    
    private List<CubeFaceDirection> GetNotVisibleFaces(DataGrid dataGrid, int m, int n, bool isTopWall)
    {
        List<CubeFaceDirection> dontCreateFaces = new List<CubeFaceDirection>() { CubeFaceDirection.Bottom };
        DataCell cell = dataGrid.GetCell(m, n);

        // top wall
        if (isTopWall)
        {
            DataCell rightCell = dataGrid.GetNeighbourAtDirection(cell, DataGrid.Direction.Right);
            if(rightCell !=null && rightCell.IsTopWallActive)
                dontCreateFaces.Add(CubeFaceDirection.Right);
            
            DataCell leftCell = dataGrid.GetNeighbourAtDirection(cell, DataGrid.Direction.Back);
            if(leftCell !=null && leftCell.IsTopWallActive)
                dontCreateFaces.Add(CubeFaceDirection.Left);
        }
        // right wall
        else
        {
            DataCell bottomCell = dataGrid.GetNeighbourAtDirection(cell, DataGrid.Direction.Bottom);
            if(bottomCell !=null && bottomCell.IsRightWallActive)
                dontCreateFaces.Add(CubeFaceDirection.Back);
            
            DataCell forwardCell = dataGrid.GetNeighbourAtDirection(cell, DataGrid.Direction.Forward);
            if(forwardCell !=null && forwardCell.IsRightWallActive)
                dontCreateFaces.Add(CubeFaceDirection.Forward);  
        }
        
        return dontCreateFaces;
    }
    #endregion Private Methods
}


