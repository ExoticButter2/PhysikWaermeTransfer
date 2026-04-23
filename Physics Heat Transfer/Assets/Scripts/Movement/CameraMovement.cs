using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public InputActionReference horizontalMovementInput;
    public InputActionReference verticalMovementInput;
    public InputActionReference lookInput;
    public InputActionReference startLookKey;

    private bool _lookEnabled = false;

    private Vector2 _moveHorizontalVector = Vector2.zero;
    private float _moveVerticalValue = 0f;

    private Transform _cameraTransform;

    [SerializeField]
    private float _moveSensitivity = 3.0f;

    [SerializeField]
    private float _mouseLookSensitivity = 1.0f;

    private float _xRotation = 0f;
    private float _yRotation = 0f;

    private Vector2 _lockedMousePosition = Vector2.zero;
    private bool _mouseLocked = false;

    [SerializeField]
    private float _rotationSmoothing = 2f;
    [SerializeField]
    private float _positionSmoothing = 2f;

    private Quaternion _targetRotation = Quaternion.identity;
    private Vector3 _targetPosition = Vector3.zero;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (_moveHorizontalVector != Vector2.zero || _moveVerticalValue != 0f)
        {
            MoveCamera();
        }

        ApplySmoothPosition();
        ApplySmoothRotation();
    }

    private void ApplySmoothPosition()
    {
        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _targetPosition, _positionSmoothing * Time.deltaTime);
    }

    private void ApplySmoothRotation()
    {
        _cameraTransform.rotation = Quaternion.Lerp(_cameraTransform.rotation, _targetRotation, _rotationSmoothing * Time.deltaTime);
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
        _xRotation -= lookInput.y;

        Vector3 camAngles = _cameraTransform.eulerAngles;

        Quaternion rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);

        _targetRotation = rotation;
    }

    private void StartMove(InputAction.CallbackContext ctx)
    {
        _moveHorizontalVector = ctx.ReadValue<Vector2>() * _moveSensitivity;
    }

    private void StartVerticalMove(InputAction.CallbackContext ctx)
    {
        _moveVerticalValue = ctx.ReadValue<float>() * _moveSensitivity;
    }

    private void StopMove(InputAction.CallbackContext ctx)
    {
        _moveHorizontalVector = Vector2.zero;
    }

    private void StopVerticalMove(InputAction.CallbackContext ctx)
    {
        _moveVerticalValue = 0f;
    }

    private void MoveCamera()
    {
        Vector3 positionAddVector = (_cameraTransform.forward * _moveHorizontalVector.y) + (_cameraTransform.right * _moveHorizontalVector.x) + (_cameraTransform.up * _moveVerticalValue);
        _targetPosition = positionAddVector + _cameraTransform.position;
    }

    private void OnEnable()
    {
        startLookKey.action.started += EnableLook;
        startLookKey.action.canceled += DisableLook;
        horizontalMovementInput.action.performed += StartMove;
        horizontalMovementInput.action.canceled += StopMove;
        verticalMovementInput.action.performed += StartVerticalMove;
        verticalMovementInput.action.canceled += StopVerticalMove;
        lookInput.action.performed += Look;
        InputSystem.onAfterUpdate += MoveMouseToLockPosition;
    }

    private void OnDisable()
    {
        startLookKey.action.started -= EnableLook;
        startLookKey.action.canceled -= DisableLook;
        horizontalMovementInput.action.performed -= StartMove;
        horizontalMovementInput.action.canceled -= StopMove;
        verticalMovementInput.action.performed -= StartVerticalMove;
        verticalMovementInput.action.canceled -= StopVerticalMove;
        lookInput.action.performed -= Look;
        InputSystem.onAfterUpdate -= MoveMouseToLockPosition;
    }
}
