//using TMPro;
//using Unity.Entities;
//using UnityEngine;

//public class TemperatureLabelAuthoring : MonoBehaviour
//{
//    public TextMeshProUGUI temperatureLabel;

//    public float xOffset;
//    public float yOffset;
//}

//public enum TemperatureUnit
//{
//    Celsius,
//    Kelvin,
//    Fahrenheit
//}

//public struct TemperatureLabelData : IComponentData
//{
//    public Entity entity;

//    public TemperatureUnit selectedUnit;

//    public Vector2 offsetVector;
//}

//public class TemperatureLabelBaker : Baker<TemperatureLabelAuthoring>
//{
//    public override void Bake(TemperatureLabelAuthoring authoring)
//    {
//        DependsOn(authoring.temperatureLabel);

//        Entity entity = GetEntity(TransformUsageFlags.None);
//        Entity tmpEntity = GetEntity(authoring.temperatureLabel.gameObject, TransformUsageFlags.Dynamic);

//        AddComponent(entity, new TemperatureLabelData { 
//            entity = tmpEntity, 
//            selectedUnit = TemperatureUnit.Celsius,
//            offsetVector = new Vector2(authoring.xOffset, authoring.yOffset)
//        });

//        AddComponentObject(tmpEntity, authoring.temperatureLabel);
//    }
//}