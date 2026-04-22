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
        if (!float.TryParse(stringValue, out float value))
            return;

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity requestEntity = entityManager.CreateEntity();

        entityManager.AddComponentData(requestEntity, new MouseClickHeaterValueRequest { value = value } );
    }
}
