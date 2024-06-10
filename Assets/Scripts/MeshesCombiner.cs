using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.RuleTile.TilingRuleOutput;

//code without chunks: https://www.youtube.com/watch?v=wYAlky1aZn4

/// <summary>
/// Allows mesh combining
/// </summary>
public class MeshesCombiner : MonoBehaviour
{
    [SerializeField] GameObject meshChunkPrefab;
    //int chunkSize = 2000;
    
    /// <summary>
    /// Combines all meshes in object child. For a lot of meshes, this could throw an exception, so you may want to use CombineMeshesNoSizeLimit instead
    /// </summary>
    /// <param name="_obj">Object whose child you want to combine the meshes</param>
    /// <returns></returns>
    public GameObject CombineMeshes(GameObject _obj) {

        MeshFilter[] meshFilters = _obj.GetComponentsInChildren<MeshFilter>();

        CombineInstance[] allCombiners = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++) {
            //0 is the default value for this field
            allCombiners[i].subMeshIndex = 0;
            allCombiners[i].mesh = meshFilters[i].sharedMesh;
            allCombiners[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }

        GameObject chunksContainer = new GameObject();
        chunksContainer.transform.position = _obj.transform.position;

        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(allCombiners);
        GameObject obj = Instantiate(meshChunkPrefab,transform.position,Quaternion.identity);
        obj.GetComponent<MeshFilter>().sharedMesh = finalMesh;
        obj.transform.parent = chunksContainer.transform;
        
        return chunksContainer;
    }

    //----------------------------------------------------------------------------------------------- not used

    /// <summary>
    /// Allows mesh comining for a really large number of meshes.
    /// Meshes combining has a limit, so for really large objects you may want to use this method
    /// </summary>
    /// <param name="_obj"></param>
    /// <returns></returns>
    //public GameObject CombineMeshesNoSizeLimit(GameObject _obj) {

    //    MeshFilter[] meshFilters = _obj.GetComponentsInChildren<MeshFilter>();

    //    List<CombineInstance[]> combinersChunks = new List<CombineInstance[]>();
    //    CombineInstance[] allCombiners = new CombineInstance[meshFilters.Length];

    //    for (int i = 0; i < meshFilters.Length; i++) {
    //        allCombiners[i].subMeshIndex = 0;
    //        allCombiners[i].mesh = meshFilters[i].sharedMesh;
    //        allCombiners[i].transform = meshFilters[i].transform.localToWorldMatrix;
    //    }

    //    int lastiBack = 0;


    //    for (int iBack = 0, iForw = chunkSize; iForw < allCombiners.Length; iBack += chunkSize, iForw += chunkSize) {
    //        CombineInstance[] combinersChunk = new CombineInstance[chunkSize];
    //        Array.Copy(allCombiners, iBack, combinersChunk, 0, chunkSize);
    //        combinersChunks.Add(combinersChunk);
    //        lastiBack = iBack;
    //    }

    //    // copy of the last elements that didn't fit in a chunk
    //    if (lastiBack != 0) lastiBack += chunkSize;
    //    List<CombineInstance> lastCombChunk = new List<CombineInstance>();
    //    for (int i = lastiBack; i < allCombiners.Length; i++) {
    //        lastCombChunk.Add(allCombiners[i]);
    //    }
    //    combinersChunks.Add(lastCombChunk.ToArray());

    //    GameObject chunksContainer = new GameObject();
    //    chunksContainer.transform.position = _obj.transform.position;
    //    foreach (CombineInstance[] chunk in combinersChunks) {
    //        Mesh finalMesh = new Mesh();
    //        finalMesh.CombineMeshes(chunk);
    //        GameObject obj = Instantiate(meshChunkPrefab, transform.position, Quaternion.identity);
    //        obj.GetComponent<MeshFilter>().sharedMesh = finalMesh;
    //        obj.transform.parent = chunksContainer.transform;
    //    }

    //    return chunksContainer;
    //}
}