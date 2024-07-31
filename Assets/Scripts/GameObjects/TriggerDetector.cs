using System;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    public event Action OnTriggerEnterCalled;
    private void OnTriggerEnter(Collider other) => OnTriggerEnterCalled?.Invoke();

}
