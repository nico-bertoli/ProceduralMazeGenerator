using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/LiveGenSettings", fileName = "LiveGenSettings")]
public class LiveGenSettings : ScriptableObject
{
    [field: SerializeField] public float MaxStepDelay { get; private set; } = 0.4f;
    [field: SerializeField] public float DefaultSpeedSliderValue { get; private set; } = 0.4f;
}
