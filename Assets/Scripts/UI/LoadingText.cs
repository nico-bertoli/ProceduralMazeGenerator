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
    [SerializeField] float animationSpeed = 0.3f;
    TextMeshProUGUI text;
    List<string> textFrames;

    //======================================== methods
    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }
    /// <summary>
    /// Evaluates text frames
    /// </summary>
    /// <param name="_message"></param>
    public void SetText(string _message) {
        textFrames = new List<string>();
        textFrames.Add(" "+_message+".");
        for (int i = 0;i<2;i++)
            textFrames.Add(" "+textFrames[textFrames.Count - 1]+".");
    }
    private void OnEnable() {
        GameController.Instance.StartCoroutine(Animation());
    }
    /// <summary>
    /// Handles animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator Animation() {
        int i = 0;
        while (gameObject.activeSelf) {
            text.text = textFrames[i++];
            if (i == textFrames.Count) i = 0;
            yield return new WaitForSeconds(animationSpeed);
        }
    }
}
