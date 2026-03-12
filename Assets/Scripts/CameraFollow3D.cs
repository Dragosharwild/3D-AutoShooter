using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow3D : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -5f);
    public float followSpeed = 12f;

    [Header("Look Input")]
    [SerializeField] private InputActionReference lookInputAction;
    [SerializeField] private float mouseSensitivity = 0.12f;
    [SerializeField] private float stickSensitivity = 180f;

    [Header("Orbit")]
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 70f;
    [SerializeField] private bool invertY;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionMask = ~0;
    [SerializeField] private float collisionRadius = 0.2f;
    [SerializeField] private float collisionBuffer = 0.05f;
    [SerializeField] private float minCollisionDistance = 0.75f;

    [Header("Cursor")]
    [SerializeField] private bool lockCursorOnEnable = true;
    [SerializeField] private bool hideCursorWhenLocked = true;

    private InputAction _lookAction;
    private float _yaw;
    private float _pitch;

    private void Awake()
    {
        _lookAction = lookInputAction != null ? lookInputAction.action : null;

        Vector3 initialAngles = transform.eulerAngles;
        _yaw = initialAngles.y;
        _pitch = initialAngles.x > 180f ? initialAngles.x - 360f : initialAngles.x;
    }

    private void OnEnable()
    {
        _lookAction?.Enable();

        if (lockCursorOnEnable)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = !hideCursorWhenLocked;
        }
    }

    private void OnDisable()
    {
        _lookAction?.Disable();
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        UpdateOrbitRotation();

        Quaternion orbitRotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 targetPosition = CalculateCollisionAwarePosition(orbitRotation);

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target.position);
    }

    private Vector3 CalculateCollisionAwarePosition(Quaternion orbitRotation)
    {
        Vector3 pivotPosition = target.position;
        Vector3 desiredOffset = orbitRotation * offset;
        float desiredDistance = desiredOffset.magnitude;

        if (desiredDistance <= 0.001f)
            return pivotPosition;

        Vector3 castDirection = desiredOffset / desiredDistance;
        Vector3 desiredPosition = pivotPosition + desiredOffset;

        if (!Physics.SphereCast(pivotPosition, collisionRadius, castDirection, out RaycastHit hit, desiredDistance, collisionMask, QueryTriggerInteraction.Ignore))
            return desiredPosition;

        float safeDistance = Mathf.Max(minCollisionDistance, hit.distance - collisionBuffer);
        return pivotPosition + castDirection * safeDistance;
    }

    private void UpdateOrbitRotation()
    {
        if (_lookAction == null)
            return;

        Vector2 look = _lookAction.ReadValue<Vector2>();
        if (look == Vector2.zero)
            return;

        bool usingPointerDelta = _lookAction.activeControl != null && _lookAction.activeControl.device is Pointer;
        float sensitivity = usingPointerDelta ? mouseSensitivity : stickSensitivity * Time.deltaTime;

        _yaw += look.x * sensitivity;

        float pitchInput = look.y * sensitivity;
        float pitchDelta = invertY ? pitchInput : -pitchInput;
        _pitch = Mathf.Clamp(_pitch + pitchDelta, minPitch, maxPitch);
    }
}
