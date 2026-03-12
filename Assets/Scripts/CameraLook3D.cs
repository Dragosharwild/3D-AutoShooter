using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook3D : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference lookInputAction;

    [Header("Rotation")]
    [SerializeField] private Transform yawTransform;
    [SerializeField] private float mouseSensitivity = 0.12f;
    [SerializeField] private float stickSensitivity = 180f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private bool invertY;

    [Header("Cursor")]
    [SerializeField] private bool lockCursorOnEnable = true;
    [SerializeField] private bool hideCursorWhenLocked = true;

    private InputAction _lookAction;
    private float _pitch;
    private float _yaw;

    private void Awake()
    {
        _lookAction = lookInputAction != null ? lookInputAction.action : null;

        if (yawTransform == null)
            yawTransform = transform.parent;

        Vector3 cameraAngles = transform.localEulerAngles;
        _pitch = cameraAngles.x > 180f ? cameraAngles.x - 360f : cameraAngles.x;

        if (yawTransform == transform)
        {
            float yaw = cameraAngles.y;
            _yaw = yaw > 180f ? yaw - 360f : yaw;
        }
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

    private void Update()
    {
        if (_lookAction == null)
            return;

        Vector2 look = _lookAction.ReadValue<Vector2>();
        if (look == Vector2.zero)
            return;

        bool usingPointerDelta = _lookAction.activeControl != null && _lookAction.activeControl.device is Pointer;
        float sensitivity = usingPointerDelta ? mouseSensitivity : stickSensitivity * Time.deltaTime;

        float yawDelta = look.x * sensitivity;
        float pitchInput = look.y * sensitivity;
        float pitchDelta = invertY ? pitchInput : -pitchInput;

        ApplyYaw(yawDelta);
        ApplyPitch(pitchDelta);
    }

    private void ApplyYaw(float yawDelta)
    {
        if (yawTransform == null)
            return;

        if (yawTransform == transform)
        {
            _yaw += yawDelta;
            transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
            return;
        }

        yawTransform.Rotate(Vector3.up, yawDelta, Space.Self);
    }

    private void ApplyPitch(float pitchDelta)
    {
        _pitch = Mathf.Clamp(_pitch + pitchDelta, minPitch, maxPitch);

        if (yawTransform == transform)
        {
            transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
            return;
        }

        transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }
}
