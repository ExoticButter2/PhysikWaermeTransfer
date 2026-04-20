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
using Unity.Entities;
using Unity.Physics;
using UnityEngine.InputSystem;
using UnityEngine;

public partial struct MouseClickHeater : ISystem
{
    public float degreesPerSecond;

    public void OnCreate()
    {
        degreesPerSecond = 0f;
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (requestData, requestEntity) in SystemAPI.Query<MouseClickHeaterValueRequest>().WithEntityAccess())
        {
            degreesPerSecond = requestData.value;
            ecb.DestroyEntity(requestEntity);
        }

        if (!Mouse.current.leftButton.isPressed)
            return;

        if (!SystemAPI.TryGetSingleton<PhysicsWorldSingleton>(out var physicsWorldSingleton))
            return;

        PhysicsWorld physicsWorld = physicsWorldSingleton.PhysicsWorld;

        UnityEngine.Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.value);

        RaycastInput raycastInput = new RaycastInput
        {
            Start = mouseRay.origin,
            End = mouseRay.origin + (mouseRay.direction * 100000f),
            Filter = CollisionFilter.Default
        };

        if (physicsWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit))
        {
            Entity hitEntity = hit.Entity;

            if (SystemAPI.HasComponent<HeatData>(hitEntity))
            {
                HeatData heatData = SystemAPI.GetComponent<HeatData>(hitEntity);

                heatData.temperature += degreesPerSecond * SystemAPI.Time.DeltaTime;

                SystemAPI.SetComponent(hitEntity, heatData);
            }
        }
    }
}