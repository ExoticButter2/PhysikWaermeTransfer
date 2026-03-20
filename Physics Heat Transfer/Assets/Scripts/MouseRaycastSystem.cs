using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycastSystem : MonoBehaviour
{
    #region HEAT_UI_EVENTS
    public static event Action OnEnableHeatUI;
    public static event Action OnDisableHeatUI;
    public static event Action<float> OnUpdateHeatUI;
    #endregion

    private Camera _mainCamera;

    [SerializeField]
    private LayerMask _heatComponentMask;

    private bool _cursorFacingHeatBlock = false;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        CheckHeatFromMouse();
    }

    private void CheckHeatFromMouse()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray mouseRay = _mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, _heatComponentMask))
        {
            Heat heatComponent = hit.collider.gameObject.GetComponent<Heat>();

            if (heatComponent == null)
            {
                Debug.LogWarning("Heat block is missing heat component!");
                return;
            }

            if (!_cursorFacingHeatBlock)
            {
                MouseRaycastSystem.OnEnableHeatUI?.Invoke();
            }

            _cursorFacingHeatBlock = true;

            MouseRaycastSystem.OnUpdateHeatUI?.Invoke(heatComponent.heat);
        }
        else if (_cursorFacingHeatBlock)
        {
            _cursorFacingHeatBlock = false;
            MouseRaycastSystem.OnDisableHeatUI?.Invoke();
        }
    }
}
