using System;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public event Action OnExitReached;
    private void OnTriggerEnter(Collider other) {
        OnExitReached();
    }
    
}
