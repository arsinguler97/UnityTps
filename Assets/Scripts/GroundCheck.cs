using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private static readonly int Jump = Animator.StringToHash("Jump");
    public PlayerMovementConfig movementConfig;
    
    private bool _isGrounded;
    private float _verticalVelocity;
    private float _jumpTimer;

    public bool IsGrounded => _isGrounded;
    public float VerticalVelocity => _verticalVelocity;
    
    private void Update()
    {
        CheckGround();
        HandleJump();
        ApplyGravity();
    }

    private void CheckGround()
    {
        Vector3 boxCenter = transform.position;
        Vector3 boxHalfExtents = new Vector3(movementConfig.groundCheckDistance, movementConfig.groundCheckHeight, movementConfig.groundCheckDistance);

        _isGrounded = Physics.CheckBox(boxCenter, boxHalfExtents, Quaternion.identity, LayerMask.GetMask("Ground"));

        if (_isGrounded && _verticalVelocity < 0)
            _verticalVelocity = -2f;
    }

    private void HandleJump()
    {
        if (_isGrounded && Input.GetButtonDown("Jump"))
        {
            _verticalVelocity = movementConfig.baseJumpForce;
            _jumpTimer = 0f;
        }

        if (Input.GetButton("Jump") && _jumpTimer < movementConfig.maxJumpHoldTime)
        {
            _verticalVelocity += movementConfig.baseJumpForce * Time.deltaTime;
            _jumpTimer += Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump"))
        {
            _jumpTimer = movementConfig.maxJumpHoldTime;
        }
    }

    private void ApplyGravity()
    {
        _verticalVelocity += Physics.gravity.y * movementConfig.gravityMultiplier * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 boxCenter = transform.position;
        Vector3 boxSize = new Vector3(movementConfig.groundCheckDistance * 2, movementConfig.groundCheckHeight * 2, movementConfig.groundCheckDistance * 2);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}