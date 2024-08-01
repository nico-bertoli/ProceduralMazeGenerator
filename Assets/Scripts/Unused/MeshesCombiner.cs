//using UnityEngine;

////https://www.youtube.com/watch?v=wYAlky1aZn4

//public class MeshesCombiner : MonoBehaviour
//{
//    [SerializeField] GameObject meshChunkPrefab;

//    /// <summary>
//    /// Combines all meshes in object child. For a lot of meshes, this could throw an exception, so you may want to use CombineMeshesNoSizeLimit instead
//    /// </summary>
//    /// <param name="objToCombineMeshes">Object whose child you want to combine the meshes</param>
//    /// <returns></returns>
//    public GameObject CombineMeshes(GameObject objToCombineMeshes)
//    {
//        MeshFilter[] meshFilters = objToCombineMeshes.GetComponentsInChildren<MeshFilter>();
//        CombineInstance[] allCombiners = new CombineInstance[meshFilters.Length];

//        for (int i = 0; i < meshFilters.Length; i++)
//        {
//            allCombiners[i].subMeshIndex = 0;
//            allCombiners[i].mesh = meshFilters[i].sharedMesh;
//            allCombiners[i].transform = meshFilters[i].transform.localToWorldMatrix;
//        }

//        Mesh combinedMesh = new Mesh();
//        combinedMesh.CombineMeshes(allCombiners);

//        GameObject combinedMeshObject = Instantiate(meshChunkPrefab,transform.position,Quaternion.identity);
//        combinedMeshObject.GetComponent<MeshFilter>().sharedMesh = combinedMesh;
//        combinedMeshObject.isStatic = true;
        
//        return combinedMeshObject;
//    }
//}