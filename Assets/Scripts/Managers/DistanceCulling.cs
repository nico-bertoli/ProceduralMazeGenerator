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

    #endregion Private Fields
    #region ============================================================================================= Public Methods
    
    public void Init() {
        Coroutiner.Instance.StartCoroutine(RefreshCullingCor());
    }
    
    #endregion Public Methods
    #region ============================================================================================ Private Methods
    
    private IEnumerator RefreshCullingCor() {
        while (this != null && gameObject != null)
        {
            if((transform.position.z < target.transform.position.z && Vector3.Distance(transform.position, target.transform.position)<zEnableDistance)||
               Vector3.Distance(transform.position, target.transform.position) < generalEnableDistance)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
            yield return new WaitForSeconds(refreshCullingEverySeconds);
            Debug.LogError("refreshing");
        }
    }
    #endregion Private Methods
}
