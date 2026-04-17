using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    //[HideInInspector]
    //public HeatGridManager HeatGridManager;

    //private Heat _currentUsedHeatComponent = null;

    //private void OnCollisionEnter(Collision collision)
    //{
    //    ContactPoint contactPoint = collision.GetContact(0);

    //    Collider childCollider = contactPoint.thisCollider;
    //    Debug.Log($"Collision started with {collision.gameObject.name} at {childCollider.name}!");

    //    _currentUsedHeatComponent = childCollider.gameObject.GetComponent<Heat>();

    //    Heat heatComponent = contactPoint.otherCollider.gameObject.GetComponent<Heat>();

    //    if (heatComponent == null)
    //    {
    //        Debug.LogWarning("The collided object does not have a Heat component.");
    //    }

    //    if (heatComponent != null)
    //    {
    //        _currentUsedHeatComponent.heatNeighbors.Add(heatComponent);
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    Collider childCollider = collision.collider;
    //    Heat heatComponent = childCollider.gameObject.GetComponent<Heat>();

    //    if (heatComponent == null)
    //    {
    //        Debug.LogWarning("The collided object does not have a Heat component.");
    //    }

    //    if (heatComponent != null && _currentUsedHeatComponent.heatNeighbors.Contains(heatComponent))
    //    {
    //        _currentUsedHeatComponent.heatNeighbors.Remove(heatComponent);
    //    }
    //}
}
