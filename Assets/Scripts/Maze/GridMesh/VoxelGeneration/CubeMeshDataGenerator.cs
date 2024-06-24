using System;
using System.Collections.Generic;
using UnityEngine;

public class CubeMeshDataGenerator : MonoBehaviour
{
    public enum CubeFaceDirection
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
    
    public static void GetMeshData(List<Vector3> vertices, List<int> triangles, Vector3 scale, Vector3 position, List<CubeFaceDirection> dontCreateFaces)
    {
        foreach (CubeFaceDirection direction in Enum.GetValues(typeof(CubeFaceDirection)))
        {
            // prevents creating bottom face
            if(dontCreateFaces.Contains(direction))
                continue;
            
            GetFaceData(vertices,triangles,direction, scale, position);
        }
    }

    private static void GetFaceData(List<Vector3> vertices, List<int> triangles, CubeFaceDirection cubeFaceDirection, Vector3 scale, Vector3 position)
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
}
