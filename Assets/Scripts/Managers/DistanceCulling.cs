using System.Collections;
using UnityEngine;


public class DistanceCulling : MonoBehaviour
{
    #region ============================================================================================== Public Fields
    
    public static GameObject target { get; set; }
    
    #endregion Public Fields
    #region ============================================================================================= Private Fields
    
    private static float refreshCullingEverySeconds = 1f;
    private static float zEnableDistance =30f;
    private static float generalEnableDistance = 18;
    private static MyMonobehaviour coroutinesOwner;
    
    #endregion Private Fields
    #region ============================================================================================= Public Methods
    
    public void Init() {
        if (coroutinesOwner == null) {
            coroutinesOwner = new GameObject().AddComponent<MyMonobehaviour>();
            coroutinesOwner.gameObject.name = "Culling Coroutines Owner";
        }
        coroutinesOwner.StartCoroutine(RefreshCullingCor());
    }
    
    #endregion Public Methods
    #region ============================================================================================ Private Methods
    
    private IEnumerator RefreshCullingCor() {
        while (coroutinesOwner != null) {
            if((transform.position.z < target.transform.position.z && Vector3.Distance(transform.position, target.transform.position)<zEnableDistance)||
               Vector3.Distance(transform.position, target.transform.position) < generalEnableDistance)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
            yield return new WaitForSeconds(refreshCullingEverySeconds);
        }
    }
    
    private void OnDestroy() {
        if (coroutinesOwner != null) {
            coroutinesOwner.StopAllCoroutines();
            Destroy(coroutinesOwner);
        }
    }
    #endregion Private Methods
}
