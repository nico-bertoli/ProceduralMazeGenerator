using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    #region ============================================================================================== Public Fields

    //--------- movement
    public bool IsMoving { get; private set; }
    public Vector2 MoveDirection { get; private set; }

    //--------- rotation
    public bool IsRotating { get; private set; }
    public float RotateDirection { get; private set; }
   
    #endregion Public Fields
    #region ============================================================================================= Public Methods
    
    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsMoving = true;
            MoveDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            IsMoving = false;
        }
    }

    public void Rotate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsRotating = true;
            RotateDirection = context.ReadValue<float>();
        }
        else if (context.canceled)
        {
            IsRotating = false;
        }
    }
    
    #endregion PublicMethods
}