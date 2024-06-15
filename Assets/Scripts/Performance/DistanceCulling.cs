using System;
using System.Collections;
using UnityEngine;

public class DistanceCulling : MonoBehaviour
{
    #region ============================================================================================== Public Fields
    
    public static GameObject target { get; set; }
    
    #endregion Public Fields
    #region ============================================================================================= Private Fields
    
    private static float cullingRefreshFactor = 0.1f;
    private static float enableDistance = 30;

    #endregion Private Fields
    #region ============================================================================================= Public Methods

    public void StartCulling() => Coroutiner.Instance.StartCoroutine(RefreshCullingCor());

        #endregion Public Methods
    #region ============================================================================================ Private Methods
    
    private IEnumerator RefreshCullingCor() {
        while (this != null && gameObject != null)
        {
            float targetDistance = Vector3.Distance(transform.position, target.transform.position);
            if(targetDistance < enableDistance)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);

            //more distant chunks refresh later
            float waitTime = cullingRefreshFactor * targetDistance;
            if (waitTime < 1)
                waitTime = 1;

            yield return new WaitForSeconds(waitTime);
        }
    }
    #endregion Private Methods
}
