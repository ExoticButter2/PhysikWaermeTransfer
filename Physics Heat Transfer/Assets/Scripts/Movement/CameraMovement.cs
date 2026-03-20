using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public InputActionReference moveInput;
    public InputActionReference lookInput;
    public InputActionReference startLookKey;

    private bool _lookEnabled = false;

    private Vector2 _moveVector = Vector2.zero;

    private Transform _cameraTransform;

    [SerializeField]
    private float _moveSensitivity = 3.0f;

    [SerializeField]
    private float _mouseLookSensitivity = 1.0f;

    [SerializeField]
    private float _minX;
    [SerializeField]
    private float _maxX;

    private float _xRotation = 0f;
    private float _yRotation = 0f;

    private Vector2 _lockedMousePosition = Vector2.zero;
    private bool _mouseLocked = false;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (_moveVector != Vector2.zero)
        {
            MoveCamera();
        }
    }

    private void MoveMouseToLockPosition()
    {
        if (!_mouseLocked)
        {
            return;
        }    

        Mouse.current.WarpCursorPosition(_lockedMousePosition);
    }

    private void EnableLook(InputAction.CallbackContext ctx)
    {
        _lookEnabled = true;
        _lockedMousePosition = Mouse.current.position.ReadValue();

        _mouseLocked = true;
    }

    private void DisableLook(InputAction.CallbackContext ctx)
    {
        _lookEnabled = false;
        _mouseLocked = false;
    }

    private void Look(InputAction.CallbackContext ctx)
    {
        if (!_lookEnabled)
        {
            return;
        }

        Vector2 lookInput = ctx.ReadValue<Vector2>() * _mouseLookSensitivity;

        _yRotation += lookInput.x;
        _xRotation = Mathf.Clamp(_xRotation - lookInput.y, _minX, _maxX);

        Vector3 camAngles = _cameraTransform.eulerAngles;

        float clampXRotation = Mathf.Clamp(_xRotation, _minX, _maxX);

        _cameraTransform.rotation = Quaternion.Euler(clampXRotation, _yRotation, 0f);
    }

    private void StartMove(InputAction.CallbackContext ctx)
    {
        _moveVector = ctx.ReadValue<Vector2>() * _moveSensitivity;
    }

    private void StopMove(InputAction.CallbackContext ctx)
    {
        _moveVector = Vector2.zero;
    }

    private void MoveCamera()
    {
        Vector3 positionAddVector = _cameraTransform.forward * _moveVector.y + _cameraTransform.right * _moveVector.x;
        _cameraTransform.position += positionAddVector * Time.deltaTime;
    }

    private void OnEnable()
    {
        startLookKey.action.started += EnableLook;
        startLookKey.action.canceled += DisableLook;
        moveInput.action.performed += StartMove;
        moveInput.action.canceled += StopMove;
        lookInput.action.performed += Look;
        InputSystem.onAfterUpdate += MoveMouseToLockPosition;
    }

    private void OnDisable()
    {
        startLookKey.action.started -= EnableLook;
        startLookKey.action.canceled -= DisableLook;
        moveInput.action.performed -= StartMove;
        moveInput.action.canceled -= StopMove;
        lookInput.action.performed -= Look;
        InputSystem.onAfterUpdate -= MoveMouseToLockPosition;
    }
}
