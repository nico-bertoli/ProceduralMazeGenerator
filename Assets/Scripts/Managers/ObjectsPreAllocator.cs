using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Preallocates a certain number of objects, and allows other objects to access them in block.
/// </summary>
public class ObjectsPreAllocator : Singleton<ObjectsPreAllocator> 
{
    //======================================== fields

    [Header("References")]
    [SerializeField] GameObject preAllocatedObjPrefab;

    //preallocates 250 x 250 cells, making maze generation faster
    int size = 65025;
    List<GameObject> objects = new List<GameObject>();


    

    //======================================== methods

    /// <summary>
    /// Returns list of preallocated objects
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<GameObject> GetObjects() {
        return objects.AsReadOnly();
    }

    /// <summary>
    /// Gives controll of preallocated objects to ObjectPreallocator
    /// </summary>
    public void RelaeaseAll() {
        for (int i = 0; i < objects.Count; i++) {
            if (objects[i] != null) {
                objects[i].gameObject.SetActive(false);
                objects[i].transform.parent = transform;
            }
        }
    }
    protected override void Awake() {
        base.Awake();
        for (int i = 0; i < size; i++) {
            GameObject obj = Instantiate(preAllocatedObjPrefab);
            obj.transform.parent = transform;
            obj.SetActive(false);
            objects.Add(obj);
        }
    }

}
