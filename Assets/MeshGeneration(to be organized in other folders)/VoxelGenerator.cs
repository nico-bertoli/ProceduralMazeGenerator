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

    [SerializeField] private float scale = 1f;

    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshCollider collider;
    
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> trinangles = new List<int>();
    
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
        vertices.Clear();
        trinangles.Clear();

        for (int m = 0; m < dataGrid.RowsCount; m++)
        {
            for (int n = 0; n < dataGrid.ColumnsCount; n++)
            {
                DataCell cell = dataGrid.GetCell(m, n);
                
                if (cell.IsTopWallActive)
                {
                    Vector3 topWallPos = new Vector3(cell.PosM + 0.5f, 0.5f, -cell.PosN);
                    MakeCube(vertices,trinangles,scale * 0.5f,topWallPos  * scale);
                }
                if (cell.IsRightWallActive)
                {
                    Vector3 rightWallPos = new Vector3(cell.PosM, 0.5f, -cell.PosN - 0.5f);
                    MakeCube(vertices,trinangles,scale * 0.5f,rightWallPos  * scale);
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

    private void MakeVoxel(List<Vector3> vertices, List<int> triangles)
    {
        
    }
    
    private void MakeCube(List<Vector3> vertices, List<int> triangles, float scale, Vector3 position)
    {
        for(int i = 0; i < 6; i++)
            MakeFace(vertices,trinangles,i, scale, position);
    }

    private void MakeFace(List<Vector3> vertices, List<int> triangles, int direction, float scale, Vector3 position)
    {
        vertices.AddRange(GetFaceVertices(direction,scale, position));
        int vertCount = vertices.Count;
        
        triangles.Add(vertCount -4);
        triangles.Add(vertCount -4 + 1);
        triangles.Add(vertCount -4 + 2);
        triangles.Add(vertCount -4);
        triangles.Add(vertCount -4 + 2);
        triangles.Add(vertCount -4 + 3);
    }

    private static Vector3[] GetFaceVertices(int direction, float scale, Vector3 position)
    {
        Vector3[] faceVertices = new Vector3[4];
        for (int i = 0; i < faceVertices.Length; i++)
            faceVertices[i] = CubeVertices[CubeTriangles[direction][i]] * scale + position;
        
        return faceVertices;
    }
    
    //-------------------

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
