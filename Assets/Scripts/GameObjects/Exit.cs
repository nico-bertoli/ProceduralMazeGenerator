using System;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public Action OnExitReached;
    private void OnTriggerEnter(Collider other) {
        OnExitReached();
    }
    
}
