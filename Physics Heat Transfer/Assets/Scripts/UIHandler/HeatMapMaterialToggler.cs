using Unity.Entities;
using UnityEngine;

public class HeatMapMaterialToggler : MonoBehaviour
{
    public void OnToggle(bool state)
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity heatGridEntity = entityManager.CreateEntityQuery(typeof(GridData)).GetSingletonEntity();

        GridData gridDataComponent = entityManager.GetComponentData<GridData>(heatGridEntity);

        gridDataComponent.heatMapEnabled = state;
    }
}
