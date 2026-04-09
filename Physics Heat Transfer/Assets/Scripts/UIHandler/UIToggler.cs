using UnityEngine;
using System.Collections.Generic;

public class UIToggler : MonoBehaviour
{
    [SerializeField]
    private List<Canvas> _uiCanvasesToToggle;

    public void OnUIToggle(bool mode)
    {
        if (mode)
        {
            foreach (Canvas canvas in _uiCanvasesToToggle)
            {
                canvas.enabled = true;
            }

        }
        else
        {
            foreach (Canvas canvas in _uiCanvasesToToggle)
            {
                canvas.enabled = false;
            }
        }
    }
}
