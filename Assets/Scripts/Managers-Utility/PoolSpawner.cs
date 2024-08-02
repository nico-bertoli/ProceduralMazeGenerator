using System.Collections.Generic;
using UnityEngine;

public class PoolSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject objectToSpawn;

    private List<GameObject> pool;
    private void Start()
    {
        Debug.Assert(objectToSpawn != null, $"{nameof(objectToSpawn)} is null");

        int startingPoolSize = (int)Mathf.Pow(Settings.Instance.MazeSettings.LiveGenMaxSideCells,2);

        pool = new List<GameObject>();
        for (int i = 0; i < startingPoolSize; i++)
            pool.Add(CreateNewPoolObject(setActive: false));
    }

    public GameObject GetItem()
    {
        //return pool object if can find one not active
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].gameObject.activeSelf == false)
            {
                pool[i].gameObject.SetActive(true);
                return pool[i];
            }
        }

        //instantiate a new object otherwise
        Debug.LogWarning("pool spawner:all pooled objects were active, creating a new one, you may want to increase starting pool size");
        GameObject res = CreateNewPoolObject(setActive: true);
        return res;
    }

    public void ReturnItem(GameObject item) => item.SetActive(false);

    private GameObject CreateNewPoolObject(bool setActive)
    {
        GameObject newObj = Instantiate(objectToSpawn, transform, true);
        newObj.SetActive(setActive);
        pool.Add(newObj);
        return newObj;
    }
}
