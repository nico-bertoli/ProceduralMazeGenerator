using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class VoxelChunk : MonoBehaviour
{
    public void Init(MeshData meshData, Material material)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MeshCollider collider = GetComponent<MeshCollider>();
        
        mesh.Clear();
        mesh.vertices = meshData.Vertices.ToArray();
        mesh.triangles = meshData.Triangles.ToArray();
        mesh.RecalculateNormals();
        
        meshRenderer.material = material;
        collider.sharedMesh = mesh;
    }
}
