using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{

    private PlayerSettings playerSettings => Settings.Instance.PlayerSettings;

    #region ============================================================================================= Private Fields

    [Header("References")]
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private PlayerInputReader inputReader;
    
    private Collision currentWallCollision;
    private Coroutine rotationCor;

    #endregion Private Fields
    #region ============================================================================================ Unity Methods

    private void OnCollisionStay(Collision collision) => currentWallCollision = collision;
    private void OnCollisionExit(Collision other) => currentWallCollision = null;

    private void OnEnable()
    {
        rotationCor = null;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    #endregion Unity Methods
    #region ========================================================================================== Private Methods

    private void HandleRotation()
    {
        if (inputReader.IsRotating && rotationCor == null)
            rotationCor = StartCoroutine(RotateCor(inputReader.RotateDirection > 0));
    }

    private IEnumerator RotateCor(bool rotateRight)
    {
        float rotateDirection = rotateRight ? 1 : -1;
        
        float totalRotation = 0;

        while (totalRotation < 90)
        {
            float frameRotation = playerSettings.rotationSpeed * Time.deltaTime;

            if (totalRotation + frameRotation > 90)
                frameRotation = 90 - totalRotation;
            
            transform.RotateAround(transform.position, Vector3.up, frameRotation * rotateDirection);

            totalRotation += frameRotation;
            yield return null;
        }

        rotationCor = null;
    }
    
    private void HandleMovement()
    {
        if (inputReader.IsMoving)
        {
            Vector3 moveDir = (-transform.forward * inputReader.MoveDirection.y - transform.right * inputReader.MoveDirection.x).normalized;
            moveDir = RotateMoveDirectionAlongCollidingWall(moveDir);
            rigidBody.velocity = moveDir * playerSettings.moveSpeed;
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
            Vector3 moveDirectionForcedByWall = Vector3.ProjectOnPlane(moveDir, collisionNormal).normalized;
            return moveDirectionForcedByWall;
        }
        return moveDir;
    }

    #endregion Private Methods
}
