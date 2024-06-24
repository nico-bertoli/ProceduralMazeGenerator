using System;
using System.Collections;
using Cinemachine.Utility;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region ============================================================================================= Private Fields
    
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 2f;

    [Header("References")]
    [SerializeField] private Rigidbody rigidBody;

    [SerializeField] private PlayerInputReader inputReader;
    
    private Collision currentWallCollision;
    private Coroutine rotationCor;

    #endregion Private Fields
    #region ============================================================================================ Private Methods

    private void OnEnable()
    {
        rotationCor = null;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleRotation()
    {
        if (inputReader.IsRotating && rotationCor == null)
            rotationCor = StartCoroutine(RotateCor());
    }

    private IEnumerator RotateCor()
    {
        float totalRotation = 0;

        while (totalRotation < 90)
        {
            float frameRotation = rotationSpeed * Time.deltaTime;

            if (totalRotation + frameRotation > 90)
                frameRotation = 90 - totalRotation;
            
            transform.RotateAround(transform.position, Vector3.up, frameRotation);
            Debug.LogError($"total rotation: {totalRotation}");
            
            totalRotation += frameRotation;
            yield return null;
        }

        rotationCor = null;
    }
    
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
            return moveDirectionForcedByWall;
        }
        return moveDir;
    }
    
    private void OnCollisionStay(Collision collision) => currentWallCollision = collision;

    private void OnCollisionExit(Collision other) => currentWallCollision = null;

    #endregion Private Methods
}
