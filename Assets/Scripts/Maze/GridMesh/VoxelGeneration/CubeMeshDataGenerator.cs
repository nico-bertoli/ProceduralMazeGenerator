using System;
using System.Collections.Generic;
using UnityEngine;

public static class CubeMeshDataGenerator
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

    
    #region ================================================================================================= Public Methods

    public static void GetCubeMeshData(
        Vector3 scale,
        Vector3 position,
        List<CubeFaceDirection> dontCreateFaces,
        ref MeshData meshData
    )
    {
        Debug.Assert(meshData.Vertices != null, "Received null vertices!");
        Debug.Assert(meshData.Triangles != null, "Received null triangles!");

        foreach (CubeFaceDirection direction in Enum.GetValues(typeof(CubeFaceDirection)))
        {
            // prevents creating bottom face
            if (dontCreateFaces.Contains(direction))
                continue;

            GetFaceData(meshData, direction, scale, position);
        }
    }

    #endregion Public Methods

    #region ================================================================================================= Private Methods

    private static readonly Vector3[] CubeVertices = new Vector3[]
   {
        new ( 1, 1, 1),
        new (-1, 1, 1),
        new (-1,-1, 1),
        new ( 1,-1, 1),
        new (-1, 1,-1),
        new ( 1, 1,-1),
        new ( 1,-1,-1),
        new (-1,-1,-1),
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

    private static void GetFaceData(MeshData meshData , CubeFaceDirection cubeFaceDirection, Vector3 scale, Vector3 position)
    {
        meshData.Vertices.AddRange(GetFaceVertices(cubeFaceDirection, scale, position));
        int vertCount = meshData.Vertices.Count;

        meshData.Triangles.Add(vertCount - 4);
        meshData.Triangles.Add(vertCount - 4 + 1);
        meshData.Triangles.Add(vertCount - 4 + 2);
        meshData.Triangles.Add(vertCount - 4);
        meshData.Triangles.Add(vertCount - 4 + 2);
        meshData.Triangles.Add(vertCount - 4 + 3);
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

    #endregion Private Methods
}
