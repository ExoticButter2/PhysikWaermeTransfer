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
        EntityManager entityManager = state.EntityManager;

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
                float deltaTime = SystemAPI.Time.DeltaTime;

                HeatData heatData = SystemAPI.GetComponent<HeatData>(hitEntity);

                if (heatData.temperature + degreesPerSecond * deltaTime <= 0f)
                {
                    heatData.temperature = 0f;
                }
                else
                {
                    heatData.temperature += degreesPerSecond * deltaTime;
                }

                SystemAPI.SetComponent(hitEntity, heatData);
            }
        }
    }
}