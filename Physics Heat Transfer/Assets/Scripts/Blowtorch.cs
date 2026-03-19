using UnityEngine;

public class Blowtorch : MonoBehaviour
{
    [SerializeField]
    private Transform _flameOrigin;

    [SerializeField]
    private float _degreesPerSecond = 2.0f;

    private bool _blowtorchEnabled = false;

    private float _flameRange = 5f;

    [SerializeField]
    private LayerMask _heatLayerMask;

    private void Start()
    {
        ToggleBlowtorch();//KEEP IT ENABLED FOR NOW
    }

    private void Update()
    {
        if (_blowtorchEnabled)
        {
            HeatUp();
        }

        Debug.DrawRay(_flameOrigin.position, _flameOrigin.forward, Color.red);
    }

    private void HeatUp()
    {
        if (Physics.Raycast(_flameOrigin.position, _flameOrigin.forward, out RaycastHit hit, _flameRange, _heatLayerMask))
        {
            Heat heatComponent = hit.collider.gameObject.GetComponent<Heat>();

            if (heatComponent == null)
            {
                Debug.LogWarning("No heat component found inside heat block!");
                return;
            }

            heatComponent.heat += _degreesPerSecond * Time.deltaTime;
        }

        //Debug.DrawRay(_flameOrigin.position, _flameOrigin.forward, Color.red);
    }

    private void ToggleBlowtorch()
    {
        _blowtorchEnabled = !_blowtorchEnabled;
    }
}
