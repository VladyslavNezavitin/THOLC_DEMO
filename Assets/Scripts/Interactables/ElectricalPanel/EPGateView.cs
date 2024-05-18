using UnityEngine;

public class EPGateView : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private int _materialIndexIn;
    [SerializeField] private int _materialIndexOut;
    [SerializeField] private int _materialIndexGate;
    [SerializeField] private Material _poweredIOMaterial;
    [SerializeField] private Material _unpoweredIOMaterial;
    [SerializeField] private Material _poweredGateMaterial;
    [SerializeField] private Material _unpoweredGateMaterial;
    
    public void UpdateVisual(bool inPowered, bool gatePowered)
    {
        Material[] materials = _renderer.materials;

        materials[_materialIndexIn] = inPowered ? _poweredIOMaterial : _unpoweredIOMaterial;
        materials[_materialIndexGate] = gatePowered ? _poweredGateMaterial : _unpoweredGateMaterial;
        materials[_materialIndexOut] = inPowered && gatePowered ? _poweredIOMaterial : _unpoweredIOMaterial;

        _renderer.materials = materials;
    }
}



