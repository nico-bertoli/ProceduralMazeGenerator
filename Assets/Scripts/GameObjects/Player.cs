using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float moveSpeed = 5f;

    [Header("References")]
    [SerializeField] Rigidbody rigidBody;

    //======================================== methods
    
    private void Update() => HandleMovement();

    private void HandleMovement() {
        if (InputManager.Instance.IsMoving) {
            Vector3 moveDir = (-Vector3.forward * InputManager.Instance.MoveDirection.y - Vector3.right * InputManager.Instance.MoveDirection.x).normalized;
            rigidBody.velocity = moveDir * moveSpeed;
        }
        else {
            rigidBody.velocity = Vector3.zero;
        }
    }
}
