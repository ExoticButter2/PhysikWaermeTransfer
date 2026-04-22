using System;
using TMPro;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class TemperatureLabelSystem : SystemBase
{
    public TemperatureUIHandler _uiHandler;

    public static Action<float> OnUpdateTemperatureUI;
    public static Action OnTemperatureRaycastExit;

    protected override void OnCreate()
    {
        _uiHandler = TemperatureUIHandler.Instance;
    }

    protected override void OnUpdate()
    {
        if (!SystemAPI.TryGetSingleton<PhysicsWorldSingleton>(out var physicsWorldSingleton))
            return;

        var physicsWorld = physicsWorldSingleton.PhysicsWorld;

        UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Mouse.current.position.value);

        RaycastInput raycastInput = new RaycastInput
        {
            Start = cameraRay.origin,
            End = cameraRay.origin + (cameraRay.direction * 10000f),
            Filter = CollisionFilter.Default
        };

        if (physicsWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit))
        {
            Entity hitEntity = hit.Entity;

            if (SystemAPI.HasComponent<HeatData>(hitEntity))
            {
                HeatData data = SystemAPI.GetComponent<HeatData>(hitEntity);
                OnUpdateTemperatureUI?.Invoke(data.temperature);
            }
        }
        else
        {
            OnTemperatureRaycastExit?.Invoke();
        }
    }
}
