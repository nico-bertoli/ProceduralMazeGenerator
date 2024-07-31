using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPAnimation : MonoBehaviour
{
    [SerializeField] private List<string> animationStrings = new List<string>();
    
    #region ============================================================================================= Private Fields
    
    [SerializeField] private float animationFramesDelay = 0.3f;

    TextMeshProUGUI tf;

    #endregion Private Fields
    #region ============================================================================================= Methods

    private void Awake() => tf = GetComponent<TextMeshProUGUI>();

    private void OnEnable() => StartCoroutine(AnimationCor());

    private IEnumerator AnimationCor()
    {
        int i = 0;
        while (true)
        {
            tf.text = $"{animationStrings[i++]}";
            
            if (i == animationStrings.Count)
                i = 0;

            yield return new WaitForSeconds(animationFramesDelay);
        }
    }
    
    #endregion Methods

    
    
}
