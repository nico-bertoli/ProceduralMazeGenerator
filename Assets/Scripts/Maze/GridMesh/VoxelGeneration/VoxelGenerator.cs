using System;
using System.Collections.Generic;
using UnityEngine;

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
    private float wallsHeight => Settings.Instance.mazeGenerationSettings.InGameWallsHeight;
    private int chunkSize => Settings.Instance.mazeGenerationSettings.VoxelChunkSize;

    #endregion Private Properties
    #region ============================================================================================ Pucblic Methods
    
    public void CreateChunks(DataGrid dataGrid)
    {
        int mChunksCount = Mathf.CeilToInt(dataGrid.RowsCount / chunkSize) +1;
        int nChunksCount = Mathf.CeilToInt(dataGrid.ColumnsCount / chunkSize) +1;

        for (int m = 0; m < mChunksCount; m++)
            for (int n = 0; n < nChunksCount; n++)
                chunks.Add(CreateChunk(dataGrid,m*chunkSize,n*chunkSize,chunkSize));
        
        marginWallsGenerator.InitMargins(dataGrid,wallsWidth);
        OnMeshGenerated?.Invoke();
    }
    
    #endregion Public Methods
    #region ============================================================================================ Private Methods

    private VoxelChunk CreateChunk(DataGrid dataGrid, int mBegin, int nBegin, int chunkSize)
    {
        // create voxel
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3 topWallScale = new Vector3(1.1f, wallsHeight, wallsWidth);
        Vector3 rightWallScale = new Vector3(wallsWidth, wallsHeight, 1.1f);

        float wallsOffsetFromCenter = 0.5f;
        
        for (int m = mBegin; m < mBegin + chunkSize && m < dataGrid.RowsCount; m++)
        {
            for (int n = nBegin; n < nBegin + chunkSize && n < dataGrid.ColumnsCount; n++)
            {
                DataCell cell = dataGrid.GetCell(m, n);
                
                if (cell.IsTopWallActive)
                {
                    Vector3 topWallPos = new Vector3(cell.PosN - wallsOffsetFromCenter, wallsHeight/2f, -cell.PosM);
                    CubeMeshDataGenerator.GetMeshData(vertices,triangles,topWallScale * 0.5f,topWallPos,false);
                }
                if (cell.IsRightWallActive)
                {
                    Vector3 rightWallPos = new Vector3(cell.PosN, wallsHeight/2f, -cell.PosM - wallsOffsetFromCenter);
                    CubeMeshDataGenerator.GetMeshData(vertices,triangles,rightWallScale * 0.5f,rightWallPos,false);
                }
            }
        }
        
        VoxelChunk chunk = Instantiate(voxelChunkPrototype,transform).GetComponent<VoxelChunk>();
        chunk.Init(vertices,triangles,material);
        chunk.gameObject.isStatic = true;
        return chunk;
    }
    
    public void Reset()
    {
        foreach (VoxelChunk chunk in chunks)
            Destroy(chunk.gameObject);
        
        chunks.Clear();
        
        marginWallsGenerator.Reset();
    }
    #endregion Private Methods
}


