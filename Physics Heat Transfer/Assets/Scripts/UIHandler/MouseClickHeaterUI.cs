using Unity.Entities;
using UnityEngine;

public struct MouseClickHeaterValueRequest : IComponentData
{
    public float value;
}

public class MouseClickHeaterUI : MonoBehaviour
{
    public void OnValueEnter(string stringValue)
    {
        float value = float.Parse(stringValue);

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity requestEntity = entityManager.CreateEntity();

        entityManager.AddComponentData(requestEntity, new MouseClickHeaterValueRequest { value = value } );
    }
}
