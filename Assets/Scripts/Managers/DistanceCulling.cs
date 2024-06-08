using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activates the object when a certain target is close
/// </summary>
public class DistanceCulling : MonoBehaviour
{
    //======================================== fields
    /// <summary>
    /// How often the coroutine checks if player is near (seconds)
    /// </summary>
    private static float checkRateo = 1f;
    /// <summary>
    /// Object enabled when at this distance zDistance from target
    /// </summary>
    private static float zEnableDistance =30f;
    /// <summary>
    /// Object enabled when at this distance from target
    /// </summary>
    private static float generalEnableDistance = 18;

    /// <summary>
    /// Culling target
    /// </summary>
    public static GameObject target { get; set; }
    /// <summary>
    /// Object used as owner of all culling coroutines
    /// </summary>
    private static MyMonobehaviour coroutinesOwner;

    //======================================== methods
    public void Init() {
        if (coroutinesOwner == null) {
            coroutinesOwner = new GameObject().AddComponent<MyMonobehaviour>();
            coroutinesOwner.gameObject.name = "Culling Coroutines Owner";
        }
        coroutinesOwner.StartCoroutine(CullingCor());
    }
    
    /// <summary>
    /// Handles culling
    /// </summary>
    /// <returns></returns>
    private IEnumerator CullingCor() {
        while (coroutinesOwner != null) {
            if((transform.position.z < target.transform.position.z && Vector3.Distance(transform.position, target.transform.position)<zEnableDistance)||
                Vector3.Distance(transform.position, target.transform.position) < generalEnableDistance)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
            yield return new WaitForSeconds(checkRateo);
        }
    }

    private void OnDestroy() {
        if (coroutinesOwner != null) {
            coroutinesOwner.StopAllCoroutines();
            Destroy(coroutinesOwner);
        }
    }
}
