using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement3D : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float JumpForce = 5f;

    [Header("Camera Relative Movement")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool rotateTowardsMoveDirection = true;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveInputAction;
    [SerializeField] private InputActionReference jumpInputAction;

    private Rigidbody _rigidbody;
    private Collider _collider;
    private Vector2 _moveInput;
    private bool _jumpQueued;
    private InputAction _moveAction;
    private InputAction _jumpAction;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _moveAction = moveInputAction != null ? moveInputAction.action : null;
        _jumpAction = jumpInputAction != null ? jumpInputAction.action : null;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        if (_moveAction != null)
            _moveAction.Enable();

        if (_jumpAction != null)
        {
            _jumpAction.performed += OnJumpPerformed;
            _jumpAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (_jumpAction != null)
        {
            _jumpAction.performed -= OnJumpPerformed;
            _jumpAction.Disable();
        }

        if (_moveAction != null)
            _moveAction.Disable();
    }

    private void Update()
    {
        if (_moveAction != null)
            _moveInput = _moveAction.ReadValue<Vector2>();
        else
            _moveInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = GetCameraRelativeMoveDirection(_moveInput);
        Vector3 worldMove = moveDirection * (MoveSpeed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(_rigidbody.position + worldMove);

        if (rotateTowardsMoveDirection && moveDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            Quaternion smoothedRotation = Quaternion.Slerp(_rigidbody.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(smoothedRotation);
        }

        if (_jumpQueued && IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }

        _jumpQueued = false;
    }

    private Vector3 GetCameraRelativeMoveDirection(Vector2 moveInput)
    {
        if (moveInput.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        Transform referenceTransform = cameraTransform != null ? cameraTransform : transform;

        Vector3 cameraForward = Vector3.ProjectOnPlane(referenceTransform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(referenceTransform.right, Vector3.up).normalized;

        Vector3 moveDirection = (cameraForward * moveInput.y) + (cameraRight * moveInput.x);
        return moveDirection.sqrMagnitude > 1f ? moveDirection.normalized : moveDirection;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        _jumpQueued = true;
    }

    private bool IsGrounded()
    {
        if (_collider == null) return false;

        Vector3 origin = _collider.bounds.center;
        float rayDistance = _collider.bounds.extents.y + 0.15f;

        return Physics.Raycast(origin, Vector3.down, rayDistance);
    }
}
