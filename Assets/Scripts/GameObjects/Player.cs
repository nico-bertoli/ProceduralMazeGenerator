using System;
using Cinemachine.Utility;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region ============================================================================================= Private Fields
    
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("References")]
    [SerializeField] private Rigidbody rigidBody;

    [SerializeField] private PlayerInputReader inputReader;
    
    private Collision currentWallCollision;

    #endregion Private Fields
    #region ============================================================================================ Private Methods

    private void Update() => HandleMovement();

    private void HandleMovement() {
        if (inputReader.IsMoving)
        {
            Vector3 moveDir = (-Vector3.forward * inputReader.MoveDirection.y - Vector3.right * inputReader.MoveDirection.x).normalized;
            moveDir = RotateMoveDirectionAlongCollidingWall(moveDir);
            rigidBody.velocity = moveDir * moveSpeed;
        }
        else
        {
            rigidBody.velocity = Vector3.zero;
        }
    }

    private Vector3 RotateMoveDirectionAlongCollidingWall(Vector3 moveDir)
    {
        if (currentWallCollision == null)
            return moveDir;
        
        Vector3 collisionNormal = currentWallCollision.contacts[0].normal;

        if (Vector3.Angle(moveDir, collisionNormal) > 90)
        {
            Vector3 moveDirectionForcedByWall = moveDir.ProjectOntoPlane(collisionNormal).normalized;
            // Debug.DrawLine(currentWallCollision.contacts[0].point,
            //     currentWallCollision.contacts[0].point + moveDirectionForcedByWall * 100 - currentWallCollision.contacts[0].point,Color.red,
            //     10);
            return moveDirectionForcedByWall;
        }
        return moveDir;
    }
    
    private void OnCollisionStay(Collision collision) => currentWallCollision = collision;

    private void OnCollisionExit(Collision other) => currentWallCollision = null;

    #endregion Private Methods
}
