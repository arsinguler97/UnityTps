using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public PlayerMovementConfig movementConfig;

    private bool _isGrounded;
    private float _verticalVelocity;

    public bool IsGrounded => _isGrounded;
    public float VerticalVelocity => _verticalVelocity;

    private void Update()
    {
        Vector3 boxCenter = transform.position;
        Vector3 boxHalfExtents = new Vector3(movementConfig.groundCheckDistance, movementConfig.groundCheckHeight, movementConfig.groundCheckDistance);

        _isGrounded = Physics.CheckBox(boxCenter, boxHalfExtents, Quaternion.identity, LayerMask.GetMask("Ground"));

        if (_isGrounded && _verticalVelocity < 0)
            _verticalVelocity = 0f;

        _verticalVelocity += Physics.gravity.y * movementConfig.gravityMultiplier * Time.deltaTime;
        
        Debug.Log(_verticalVelocity);
    }

    public void ApplyJumpForce()
    {
        _verticalVelocity = movementConfig.baseJumpForce;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 boxCenter = transform.position;
        Vector3 boxSize = new Vector3(movementConfig.groundCheckDistance * 2, movementConfig.groundCheckHeight * 2, movementConfig.groundCheckDistance * 2);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}