using System;
using System.Collections.Generic;
using UnityEngine;

// : https://www.youtube.com/watch?v=b_1ZlHrJZc4&list=PL5KbKbJ6Gf9-d303Lk8TGKCW-t5JsBdtB&index=10

public class VoxelGenerator : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private VoxelChunk voxelChunkPrototype;
    
    [Header("Settings")]
    [SerializeField] private Material material;
    [SerializeField] private float wallsHeight = 0.5f;

    private float wallsWidth => Settings.Instance.mazeGenerationSettings.IngameWallsWidth;
    private int chunkSize => Settings.Instance.mazeGenerationSettings.VoxelChunkSize;
    
    public Action OnMeshGenerated;

    public void CreateChunks(DataGrid dataGrid)
    {
        int mChunksCount = Mathf.CeilToInt(dataGrid.RowsCount / chunkSize);
        int nChunksCount = Mathf.CeilToInt(dataGrid.ColumnsCount / chunkSize);

        for (int m = 0; m < mChunksCount; m++)
            for (int n = 0; n < nChunksCount; n++)
                CreateChunk(dataGrid,m*chunkSize,n*chunkSize,chunkSize);
            
        
        OnMeshGenerated?.Invoke();
    }

    private void CreateChunk(DataGrid dataGrid, int mBegin, int nBegin, int chunkSize)
    {
        // create voxel
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3 topWallScale = new Vector3(1, wallsHeight, wallsWidth);
        Vector3 rightWallScale = new Vector3(wallsWidth, wallsHeight, 1);

        float wallsOffsetFromCenter = 0.5f;
        
        for (int m = mBegin; m < mBegin + chunkSize && m < dataGrid.RowsCount; m++)
        {
            for (int n = nBegin; n < nBegin + chunkSize && n < dataGrid.ColumnsCount; n++)
            {
                DataCell cell = dataGrid.GetCell(m, n);
                
                if (cell.IsTopWallActive)
                {
                    Vector3 topWallPos = new Vector3(cell.PosN - wallsOffsetFromCenter, 0, -cell.PosM);
                    CreateCube(vertices,triangles,topWallScale * 0.5f,topWallPos);
                }
                if (cell.IsRightWallActive)
                {
                    Vector3 rightWallPos = new Vector3(cell.PosN, 0, -cell.PosM - wallsOffsetFromCenter);
                    CreateCube(vertices,triangles,rightWallScale * 0.5f,rightWallPos);
                }
            }
        }
        
        VoxelChunk chunk = Instantiate(voxelChunkPrototype,transform).GetComponent<VoxelChunk>();
        chunk.Init(vertices,triangles,material);
        chunk.gameObject.isStatic = true;
    }
    
    

    //-------------------------------------------------------------------------------

    private void CreateCube(List<Vector3> vertices, List<int> triangles, Vector3 scale, Vector3 position)
    {
        foreach (CubeFaceDirection direction in Enum.GetValues(typeof(CubeFaceDirection)))
        {
            // prevents creating bottom face
            if(direction == CubeFaceDirection.Bottom)
                continue;
            
            CreateFace(vertices,triangles,direction, scale, position);
        }
    }

    private void CreateFace(List<Vector3> vertices, List<int> triangles, CubeFaceDirection cubeFaceDirection, Vector3 scale, Vector3 position)
    {
        vertices.AddRange(GetFaceVertices(cubeFaceDirection,scale, position));
        int vertCount = vertices.Count;
        
        triangles.Add(vertCount -4);
        triangles.Add(vertCount -4 + 1);
        triangles.Add(vertCount -4 + 2);
        triangles.Add(vertCount -4);
        triangles.Add(vertCount -4 + 2);
        triangles.Add(vertCount -4 + 3);
    }

    private static Vector3[] GetFaceVertices(CubeFaceDirection faceDirection, Vector3 scale, Vector3 position)
    {
        Vector3[] faceVertices = new Vector3[4];
        for (int i = 0; i < faceVertices.Length; i++)
        {
            var vertex = CubeVertices[CubeTriangles[(int)faceDirection][i]];
            vertex.Scale(scale);
            vertex += position;
            faceVertices[i] = vertex;
        }
        return faceVertices;
    }

    //-------------------
    
    private enum CubeFaceDirection
    {
        Forward = 0,
        Right = 1,
        Back = 2,
        Left = 3,
        Up = 4,
        Bottom = 5
    }

    private static readonly Vector3[] CubeVertices = new Vector3[]
    {
        new (1, 1,  1),
        new (-1, 1, 1),
        new (-1,-1, 1),
        new (1, -1, 1),
        new (-1, 1,-1),
        new (1, 1, -1),
        new (1, -1,-1),
        new (-1, -1,-1),
    };

    private static readonly int[][] CubeTriangles = new int[][]
    {
        new [] { 0, 1, 2, 3 },
        new [] { 5, 0, 3, 6 },
        new [] { 4, 5, 6, 7 },
        new [] { 1, 4, 7, 2 },
        new [] { 5, 4, 1, 0 },
        new [] { 3, 2, 7, 6 }
    };
}
