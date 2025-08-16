using UnityEngine;
using System.Collections;

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
    [SerializeField] public int maxArrowInventory = 20;
    [SerializeField] private GameObject bowObject;
    [SerializeField] private Transform bowBackSlot;
    [SerializeField] private Transform bowHandSlot;
    [SerializeField] private Transform bowInHandPose;
    [SerializeField] private Transform bowOnBackPose;
    [SerializeField] private Transform spineBone;

    public PlayerMovementConfig movementConfig;
    public GroundCheck groundCheck;
    public Transform cameraHolder;

    private CharacterController _controller;
    private Vector3 _velocity;
    private float _cameraPitch;
    private BowController _bowController;
    private float _jumpTimer;
    private bool _isBowEquipped = false;
    public bool IsBowEquipped => _isBowEquipped;
    public int currentArrowCount = 10;
    private Collider[] _pickupHits = new Collider[10];

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        _bowController = FindFirstObjectByType<BowController>();

        _isBowEquipped = false;
        bowObject.SetActive(true);
        bowObject.transform.SetParent(bowBackSlot);
        bowObject.transform.localPosition = Vector3.zero;
        bowObject.transform.localRotation = Quaternion.identity;
    }

    private void Update()
    {
        if (PauseMenu.GameIsPaused) return;

        bool isGrounded = groundCheck.IsGrounded;
        _animator.SetBool(IsJumpingAnimation, !isGrounded);
        _animator.SetFloat(VerticalVelocityAnimation, groundCheck.VerticalVelocity);

        if (Input.GetButtonDown("Jump") && isGrounded && (_bowController == null || !_bowController.IsCharging))
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleBow();
        }
    }

    private void LateUpdate()
    {
        if (_bowController != null && _bowController.IsCharging && spineBone != null)
        {
            float pitch = cameraHolder.localEulerAngles.x;
            if (pitch > 180f) pitch -= 360f;
            pitch = Mathf.Clamp(pitch, -45f, 45f);

            spineBone.localRotation = Quaternion.Euler(0f, 0f, pitch);
        }
    }

    private void HandleLook()
    {
        if (PauseMenu.GameIsPaused) return;

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

        float targetSpeed;

        if (_isBowEquipped)
        {
            targetSpeed = movementConfig.bowMoveSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            targetSpeed = movementConfig.runSpeed;
        }
        else
        {
            targetSpeed = movementConfig.targetMoveSpeed;
        }

        Vector3 moveDirection = (transform.right * inputX + transform.forward * inputZ).normalized;

        float rate = isGrounded ? movementConfig.accelerationRate : movementConfig.accelerationRate * movementConfig.airControlFactor;
        Vector3 horizontalVelocity = moveDirection * targetSpeed;

        _velocity.x = Mathf.MoveTowards(_velocity.x, horizontalVelocity.x, rate * Time.deltaTime);
        _velocity.z = Mathf.MoveTowards(_velocity.z, horizontalVelocity.z, rate * Time.deltaTime);

        Vector3 flatVelocity = new Vector3(_velocity.x, 0f, _velocity.z);
        Vector3 localDir = transform.InverseTransformDirection(flatVelocity.normalized);

        float scale = (_isBowEquipped || !Input.GetKey(KeyCode.LeftShift)) ? 0.5f : 1f;

        _animator.SetFloat(MoveXAnimation, localDir.x * scale);
        _animator.SetFloat(MoveZAnimation, localDir.z * scale);
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

    private void ToggleBow()
    {
        if (_bowController != null && _bowController.IsCharging)
            return;

        if (_isBowEquipped)
        {
            _animator.SetTrigger("TriggerUnequip");
        }
        else
        {
            _animator.SetTrigger("TriggerEquip");
        }

        StartCoroutine(SwitchBowAfterDelay());
    }

    private IEnumerator SwitchBowAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        _isBowEquipped = !_isBowEquipped;

        _animator.SetBool("IsBowEquipped", _isBowEquipped);

        if (_isBowEquipped)
            EquipBow();
        else
            UnequipBow();

        _animator.ResetTrigger("TriggerEquip");
        _animator.ResetTrigger("TriggerUnequip");
    }

    private void EquipBow()
    {
        bowObject.SetActive(true);
        bowObject.transform.SetParent(bowHandSlot);
        bowObject.transform.localPosition = bowInHandPose.localPosition;
        bowObject.transform.localRotation = bowInHandPose.localRotation;

        _animator.Play("EquipBow");
    }

    private void UnequipBow()
    {
        bowObject.transform.SetParent(bowBackSlot);
        bowObject.transform.localPosition = bowOnBackPose.localPosition;
        bowObject.transform.localRotation = bowOnBackPose.localRotation;

        _animator.Play("UnequipBow");
    }

    public Animator GetAnimator()
    {
        return _animator;
    }
}
