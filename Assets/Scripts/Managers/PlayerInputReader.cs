using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    #region ============================================================================================== Public Fields
    
    public Vector2 MoveDirection { get; private set; }
    public bool IsMoving { get; set; }
    
    #endregion Public Fields
    #region ============================================================================================= Public Methods
    
    public void Move(InputAction.CallbackContext context) {
        if (context.performed) {
            IsMoving = true;
            MoveDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled) {
            IsMoving = false;
        }
    }
    
    #endregion PublicMethods
}