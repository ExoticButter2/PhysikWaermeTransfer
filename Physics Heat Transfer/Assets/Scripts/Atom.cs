using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Atom : MonoBehaviour
{
    [SerializeField]
    private Nucleus _atomNucleus;
    [SerializeField]
    private List<Electron> _electrons;
}
