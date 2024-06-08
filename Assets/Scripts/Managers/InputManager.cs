using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    //======================================== fields
    public Vector2 MoveDirection { get; private set; }
    public bool IsMoving { get; set; }

    //======================================== methods
    public void Move(InputAction.CallbackContext context) {
        if (context.performed) {
            IsMoving = true;
            MoveDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled) {
            IsMoving = false;
        }
    }
}