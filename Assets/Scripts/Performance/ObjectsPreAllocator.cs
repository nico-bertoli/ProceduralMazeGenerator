using System.Collections.Generic;
using UnityEngine;
public class ObjectsPreAllocator : Singleton<ObjectsPreAllocator> 
{
    #region ============================================================================================= Private Fields

    [Header("References")]
    [SerializeField] private GameObject preAllocatedObjPrefab;
    
    // int size = 65025; //250x250
    // int size = 90000; //300 x 300
    int size = 160000; //400 x 400
    List<GameObject> objects = new List<GameObject>();

    #endregion Private Fields
    #region ============================================================================================= Public Methods
    
    public IReadOnlyList<GameObject> GetPreallocatedObjects() => objects.AsReadOnly();
    
    public void ResetPreallocatedObjects() {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
            obj.transform.parent = transform;
        }
    }

    #endregion Public Methods
    #region ========================================================================================== Protected Methods

    protected override void Awake() {
        base.Awake();
        for (int i = 0; i < size; i++) {
            GameObject obj = Instantiate(preAllocatedObjPrefab, transform);//true
            obj.SetActive(false);
            objects.Add(obj);
        }
    }

    #endregion Protected Methods
}
