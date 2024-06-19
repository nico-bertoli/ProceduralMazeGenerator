using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Material material;
    
    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshCollider collider;
    
    private Vector3[] vertices;
    private int[] trinangles;
    
    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<MeshCollider>();
        meshRenderer.material = material;
    }

    private void Start()
    {
        CreateMesh();
    }

    void CreateMesh()
    {
        // array of vertices
        vertices = new Vector3[]
        {
            new (0, 0, 0),
            new (0, 0, 1),
            new (0, 1, 1),
            new (0, 1, 0),
        };

        // array of indices
        trinangles = new[]
        {
            0,1,2,
            0,2,3
        };
        
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = trinangles;

        mesh.RecalculateNormals();
        collider.sharedMesh = mesh;
    }
}
