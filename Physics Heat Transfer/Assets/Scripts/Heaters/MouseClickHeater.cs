//using UnityEngine;
//using UnityEngine.InputSystem;

//public class MouseClickHeater : MonoBehaviour
//{
//    [SerializeField]
//    private InputActionReference _leftMouseClickAction;

//    private bool _heatingEnabled = false;

//    [SerializeField]
//    private LayerMask _heatComponentLayerMask;

//    [SerializeField]
//    private float _kelvinIncreasePerSecond = 5f;

//    private void Update()
//    {
//        if (_heatingEnabled)
//        {
//            HeatUpObject();
//        }
//    }

//    private void HeatUpObject()
//    {
//        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, Mathf.Infinity, _heatComponentLayerMask))
//        {
//            Heat heatComponent = hit.collider.gameObject.GetComponent<Heat>();

//            if (heatComponent == null)
//            {
//                Debug.LogWarning("No heat component found inside heat object!");
//                return;
//            }

//            heatComponent.HeatP += _kelvinIncreasePerSecond * Time.deltaTime;
//        }
//    }

//    public void OnTemperatureIncreasePerSecondChange(string newValueString)
//    {
//        Debug.Log($"New value string: {newValueString}");

//        if (float.TryParse(newValueString, out float newValue))
//        {
//            _kelvinIncreasePerSecond = newValue;
//        }
//        else
//        {
//            Debug.LogWarning("Could not parse new value for temperature increase per second!");
//        }
//    }

//    private void StartHeatingObject(InputAction.CallbackContext ctx)
//    {
//        _heatingEnabled = true;
//    }

//    private void StopHeatingObject(InputAction.CallbackContext ctx)
//    {
//        _heatingEnabled = false;
//    }

//    private void OnEnable()
//    {
//        _leftMouseClickAction.action.started += StartHeatingObject;
//        _leftMouseClickAction.action.canceled += StopHeatingObject;
//    }

//    private void OnDisable()
//    {
//        _leftMouseClickAction.action.started -= StartHeatingObject;
//        _leftMouseClickAction.action.canceled -= StopHeatingObject;
//    }
//}
