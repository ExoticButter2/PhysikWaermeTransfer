using Unity.Entities;
using UnityEngine;

public struct ClearButtonRequest : IComponentData
{

}

public class ClearButtonHandler : MonoBehaviour
{
    public void OnClearButtonClicked()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        Entity requestEntity = entityManager.CreateEntity();
        entityManager.AddComponentData(requestEntity, new ClearButtonRequest());
    }
}
