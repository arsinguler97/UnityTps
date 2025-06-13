using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public PlayerMovementConfig movementConfig;
    public GroundCheck groundCheck;

    private CharacterController _controller;
    private Vector3 _velocity;
    
    private float _cameraPitch;

    public Transform cameraHolder;
    
    [SerializeField] Animator playerAnimator;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        bool isGrounded = groundCheck.IsGrounded;

        HandleLook();
        HandleMovement(isGrounded);

        _velocity.y = groundCheck.VerticalVelocity;

        _controller.Move(_velocity * Time.deltaTime);
        
        playerAnimator.SetFloat("MovementSpeed", _velocity.magnitude);
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * movementConfig.lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * movementConfig.lookSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -80f, 80f);

        cameraHolder.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
    }

    private void HandleMovement(bool isGrounded)
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = transform.right * inputX + transform.forward * inputZ;
        moveDirection.Normalize();

        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? movementConfig.runSpeed : movementConfig.targetMoveSpeed;
        float rate = isGrounded ? movementConfig.accelerationRate : movementConfig.accelerationRate * movementConfig.airControlFactor;

        Vector3 horizontalVelocity = moveDirection * targetSpeed;
        _velocity.x = Mathf.MoveTowards(_velocity.x, horizontalVelocity.x, rate * Time.deltaTime);
        _velocity.z = Mathf.MoveTowards(_velocity.z, horizontalVelocity.z, rate * Time.deltaTime);
    }
}