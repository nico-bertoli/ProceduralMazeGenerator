using System;
using System.Collections.Generic;
using UnityEngine;

// : https://www.youtube.com/watch?v=b_1ZlHrJZc4&list=PL5KbKbJ6Gf9-d303Lk8TGKCW-t5JsBdtB&index=10

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class VoxelGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Material material;

    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshCollider collider;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<MeshCollider>();
        meshRenderer.material = material;
    }

    public void CreateVoxel(DataGrid dataGrid)
    {
        // create voxel
        List<Vector3> vertices = new List<Vector3>();
        List<int> trinangles = new List<int>();

        Vector3 topWallScale = new Vector3(0.5f, 0.5f, 0.1f);
        Vector3 rightWallScale = new Vector3(0.1f, 0.5f, 0.5f);

        for (int m = 0; m < dataGrid.RowsCount; m++)
        {
            for (int n = 0; n < dataGrid.ColumnsCount; n++)
            {
                DataCell cell = dataGrid.GetCell(m, n);

                float wallOffsetFromCenter = 0.5f;
                
                if (cell.IsTopWallActive)
                {
                    Vector3 topWallPos = new Vector3(cell.PosN - wallOffsetFromCenter, 0.5f, -cell.PosM);
                    MakeCube(vertices,trinangles,topWallScale * 0.5f,topWallPos);
                }
                if (cell.IsRightWallActive)
                {
                    Vector3 rightWallPos = new Vector3(cell.PosN, 0.5f, -cell.PosM - wallOffsetFromCenter);
                    MakeCube(vertices,trinangles,rightWallScale * 0.5f,rightWallPos);
                }
            }
        }
        
        // create mesh
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = trinangles.ToArray();
        mesh.RecalculateNormals();
        
        // setup collider
        collider.sharedMesh = mesh;
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
