using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlayerSettings",fileName = "PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    [field:Header("Player Settings")]
    [field:SerializeField] public float moveSpeed { get; private set; } = 5f;
    [field: SerializeField] public float rotationSpeed { get; private set; } = 2f;
    
    
}
