using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private static readonly int MoveXAnimation = Animator.StringToHash("MoveX");
    private static readonly int MoveZAnimation = Animator.StringToHash("MoveZ");
    private static readonly int IsJumpingAnimation = Animator.StringToHash("IsJumping");
    private static readonly int VerticalVelocityAnimation = Animator.StringToHash("VerticalVelocity");

    [SerializeField] private Animator _animator;
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] public int maxArrowInventory = 10;

    public PlayerMovementConfig movementConfig;
    public GroundCheck groundCheck;
    public Transform cameraHolder;

    private CharacterController _controller;
    private Vector3 _velocity;
    private float _cameraPitch;
    private BowController _bowController;
    private float _jumpTimer;

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
        bool isGrounded = groundCheck.IsGrounded;
        _animator.SetBool(IsJumpingAnimation, !isGrounded);
        _animator.SetFloat(VerticalVelocityAnimation, groundCheck.VerticalVelocity);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            groundCheck.ApplyJumpForce();
        }

        HandleLook();
        HandleMovement(isGrounded);

        _velocity.y = groundCheck.VerticalVelocity;
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
        _cameraPitch = Mathf.Clamp(_cameraPitch, -75f, 75f);

        cameraHolder.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
    }

    private void HandleMovement(bool isGrounded)
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        
        float targetSpeed = movementConfig.targetMoveSpeed;
        if (_bowController != null && _bowController.IsCharging)
            targetSpeed = movementConfig.chargingMoveSpeed;
        else if (Input.GetKey(KeyCode.LeftShift))
            targetSpeed = movementConfig.runSpeed;

        Vector3 moveDirection = (transform.right * inputX + transform.forward * inputZ).normalized;

        float rate = isGrounded ? movementConfig.accelerationRate : movementConfig.accelerationRate * movementConfig.airControlFactor;
        Vector3 horizontalVelocity = moveDirection * targetSpeed;

        _velocity.x = Mathf.MoveTowards(_velocity.x, horizontalVelocity.x, rate * Time.deltaTime);
        _velocity.z = Mathf.MoveTowards(_velocity.z, horizontalVelocity.z, rate * Time.deltaTime);

        Vector3 flatVelocity = new Vector3(_velocity.x, 0f, _velocity.z);
        Vector3 localDir = transform.InverseTransformDirection(flatVelocity.normalized);

        float scale = Input.GetKey(KeyCode.LeftShift) ? 1f : 0.5f;

        _animator.SetFloat(MoveXAnimation, localDir.x * scale);
        _animator.SetFloat(MoveZAnimation, localDir.z * scale);
        
        Debug.Log("MoveZ: " + localDir.x * scale);
        Debug.Log("MoveX: " + localDir.z * scale);
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
