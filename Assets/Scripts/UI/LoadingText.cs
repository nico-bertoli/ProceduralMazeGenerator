using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
/// <summary>
/// Handles loading text animation
/// </summary>
public class LoadingText : MonoBehaviour
{
    public string Text { get; set; }
    
    #region ============================================================================================= Private Fields
    
    [SerializeField] private float animationFramesDelay = 0.3f;
    [SerializeField] private TextMeshProUGUI shownTf;
    
    private List<string> beginningAnimationCharacters = new List<string>() { "", " ", "  ", "   " };
    private List<string> endAnimationCharacters = new List<string>() { "", ".", "..", "..." };
    
    #endregion Private Fields
    #region ============================================================================================= Methods

    private void OnEnable()
    {
      StartCoroutine(DotsAnimationCor());
    }

    /// <summary>
    /// Handles animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator DotsAnimationCor() {
        int i = 0;
        while (true)
        {
            shownTf.text = $"{beginningAnimationCharacters[i]}{Text}{endAnimationCharacters[i++]}";
            
            if (i == endAnimationCharacters.Count)
                i = 0;
            yield return new WaitForSeconds(animationFramesDelay);
        }
    }
    
    #endregion Methods

    
    
}
