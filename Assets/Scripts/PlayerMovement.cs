using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private static readonly int IsGroundedAnimation = Animator.StringToHash("IsGroundedAnimation");
    private static readonly int MovementSpeedAnimation = Animator.StringToHash("MovementSpeedAnimation");

    [SerializeField] private Animator _animator;
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] public int maxArrowInventory = 10;

    private BowController _bowController;
    public PlayerMovementConfig movementConfig;
    public GroundCheck groundCheck;

    private CharacterController _controller;
    private Vector3 _velocity;
    private float _cameraPitch;
    private float _jumpTimer;

    public Transform cameraHolder;
    public int currentArrowCount = 10;

    private Collider[] _pickupHits = new Collider[10];

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        _bowController = FindFirstObjectByType<BowController>();
    }

    private void Update()
    {
        float horizontalSpeed = new Vector3(_velocity.x, 0f, _velocity.z).magnitude;
        _animator.SetFloat(MovementSpeedAnimation, horizontalSpeed);

        bool isGrounded = groundCheck.IsGrounded;
        _animator.SetBool(IsGroundedAnimation, isGrounded);

        HandleLook();
        HandleMovement(isGrounded);

        if (groundCheck.IsGrounded && Input.GetButtonDown("Jump"))
        {
            _velocity.y = movementConfig.baseJumpForce;
            _jumpTimer = 0f;
            _animator.SetTrigger("StartJump");
        }

        if (Input.GetButton("Jump") && _jumpTimer < movementConfig.maxJumpHoldTime)
        {
            _velocity.y += movementConfig.baseJumpForce * Time.deltaTime;
            _jumpTimer += Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump"))
        {
            _jumpTimer = movementConfig.maxJumpHoldTime;
        }

        _velocity.y += Physics.gravity.y * movementConfig.gravityMultiplier * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);

        if (Input.GetKeyDown(pickupKey))
        {
            TryPickupArrow();
        }
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

        float targetSpeed;

        if (_bowController != null && _bowController.IsCharging)
            targetSpeed = movementConfig.chargingMoveSpeed;
        else
            targetSpeed = Input.GetKey(KeyCode.LeftShift) ? movementConfig.runSpeed : movementConfig.targetMoveSpeed;

        if (inputX != 0 && (_bowController == null || !_bowController.IsCharging))
        {
            float rotationAmount = inputX * movementConfig.rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up * rotationAmount);
        }

        Vector3 moveDirection = (transform.right * inputX + transform.forward * inputZ).normalized;

        float rate = isGrounded ? movementConfig.accelerationRate : movementConfig.accelerationRate * movementConfig.airControlFactor;
        Vector3 horizontalVelocity = moveDirection * targetSpeed;

        _velocity.x = Mathf.MoveTowards(_velocity.x, horizontalVelocity.x, rate * Time.deltaTime);
        _velocity.z = Mathf.MoveTowards(_velocity.z, horizontalVelocity.z, rate * Time.deltaTime);
    }

    private void TryPickupArrow()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, pickupRange, _pickupHits);

        for (int i = 0; i < hitCount; i++)
        {
            GameObject obj = _pickupHits[i].gameObject;

            if (obj.CompareTag("Arrow") && currentArrowCount < maxArrowInventory)
            {
                Destroy(obj);
                currentArrowCount++;
                break;
            }
        }
    }
}
