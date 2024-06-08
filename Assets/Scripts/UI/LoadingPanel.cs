using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles loading text 
/// </summary>
public class LoadingPanel : MonoBehaviour
{
    [SerializeField] LoadingText text;
    public string Text {
        set { text.SetText(value); }
    }

    private void OnEnable() {
        text.gameObject.SetActive(true);
    }
    private void OnDisable() {
        text.gameObject.SetActive(false);
    }
}
