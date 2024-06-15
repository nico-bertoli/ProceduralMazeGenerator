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
    //======================================== fields
    [SerializeField] float animationFramesDelay = 0.3f;
    TextMeshProUGUI text;
    List<string> dotsAnimationFrames;

    //======================================== methods
    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }
    
    /// <summary>
    /// Evaluates text frames
    /// </summary>
    /// <param name="message"></param>
    public void SetText(string message) {
        dotsAnimationFrames = new List<string>();
        dotsAnimationFrames.Add(" "+message+".");
        for (int i = 0;i<2;i++)
            dotsAnimationFrames.Add(" "+dotsAnimationFrames[dotsAnimationFrames.Count - 1]+".");
    }
    private void OnEnable() {
        GameController.Instance.StartCoroutine(DotsAnimationCor());
    }
    /// <summary>
    /// Handles animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator DotsAnimationCor() {
        int i = 0;
        while (gameObject.activeSelf) {
            text.text = dotsAnimationFrames[i++];
            if (i == dotsAnimationFrames.Count)
                i = 0;
            yield return new WaitForSeconds(animationFramesDelay);
        }
    }
}
