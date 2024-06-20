using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// : https://www.youtube.com/watch?v=b_1ZlHrJZc4&list=PL5KbKbJ6Gf9-d303Lk8TGKCW-t5JsBdtB&index=10

public class VoxelGenerator : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private VoxelChunk voxelChunkPrototype;
    
    [Header("Settings")]
    [SerializeField] private Material material;
    [SerializeField] private float wallsThickness = 0.1f;
    [SerializeField] private float wallsHeight = 0.5f;
    
    public Action OnMeshGenerated;
    private int CHUNK_SIZE = 40;

    public void CreateVoxel(DataGrid dataGrid)
    {
        const int chunkSize = 20;

        int mChunksCount = Mathf.CeilToInt(dataGrid.RowsCount / chunkSize);
        int nChunksCount = Mathf.CeilToInt(dataGrid.ColumnsCount / chunkSize);

        for (int m = 0; m < mChunksCount; m++)
        {
            for (int n = 0; n < nChunksCount; n++)
            {
                CreateChunk(dataGrid,m*chunkSize,n*chunkSize,chunkSize);
            }
        }
        
        OnMeshGenerated?.Invoke();
    }

    private void CreateChunk(DataGrid dataGrid, int mBegin, int nBegin, int chunkSize)
    {
        // create voxel
        List<Vector3> vertices = new List<Vector3>();
        List<int> trinangles = new List<int>();

        Vector3 topWallScale = new Vector3(1, wallsHeight, wallsThickness);
        Vector3 rightWallScale = new Vector3(wallsThickness, wallsHeight, 1);

        for (int m = mBegin; m < mBegin + chunkSize && m < dataGrid.RowsCount; m++)
        {
            for (int n = nBegin; n < nBegin + chunkSize && n < dataGrid.ColumnsCount; n++)
            {
                DataCell cell = dataGrid.GetCell(m, n);
                float wallOffsetFromCenter = 0.5f;
                if (cell.IsTopWallActive)
                {
                    Vector3 topWallPos = new Vector3(cell.PosN - wallOffsetFromCenter, 0, -cell.PosM);
                    MakeCube(vertices,trinangles,topWallScale * 0.5f,topWallPos);
                }
                if (cell.IsRightWallActive)
                {
                    Vector3 rightWallPos = new Vector3(cell.PosN, 0, -cell.PosM - wallOffsetFromCenter);
                    MakeCube(vertices,trinangles,rightWallScale * 0.5f,rightWallPos);
                }
            }
        }
        
        VoxelChunk chunk = Instantiate(voxelChunkPrototype,transform).GetComponent<VoxelChunk>();
        chunk.Init(vertices,trinangles,material);
        // chunk.transform.position = transform.position;
        // chunk.transform.parent = transform;
        chunk.gameObject.isStatic = true;
    }

    //-------------------------------------------------------------------------------

    private void MakeCube(List<Vector3> vertices, List<int> triangles, Vector3 scale, Vector3 position)
    {
        foreach (CubeFaceDirection direction in Enum.GetValues(typeof(CubeFaceDirection)))
        {
            // prevents creating bottom face
            if(direction == CubeFaceDirection.Bottom)
                continue;
            
            MakeFace(vertices,triangles,direction, scale, position);
        }
    }

    private void MakeFace(List<Vector3> vertices, List<int> triangles, CubeFaceDirection cubeFaceDirection, Vector3 scale, Vector3 position)
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
