using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public struct GenerateGridRequest : IComponentData
{
    public float3 gridParentPosition;
}

public class GenerateButtonHandler : MonoBehaviour
{
    private Transform _cameraTransform;

    [SerializeField]
    private float distanceFromCameraToHeatGrid = 5f;

    public void OnGenerateButtonClicked()
    {
        _cameraTransform = Camera.main.transform;

        Vector3 heatGridParentPosition = _cameraTransform.position + _cameraTransform.forward * distanceFromCameraToHeatGrid;

        World entityManagerWorld = World.DefaultGameObjectInjectionWorld;

        if (entityManagerWorld == null)
            return;

        EntityManager entityManager = entityManagerWorld.EntityManager;

        Entity requestEntity = entityManager.CreateEntity();
        entityManager.AddComponentData(requestEntity, new GenerateGridRequest { gridParentPosition = (float3)heatGridParentPosition });
    }
}
