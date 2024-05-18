using UnityEngine;

public class EPInverterView : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private int _materialIndexIn;
    [SerializeField] private int _materialIndexOut;
    [SerializeField] private int _materialIndexInversion;
    [SerializeField] private Material _poweredIOMaterial;
    [SerializeField] private Material _unpoweredIOMaterial;
    [SerializeField] private Material _poweredInversionMaterial;
    [SerializeField] private Material _unpoweredInversionMaterial;

    public void UpdateVisual(bool isInPowered)
    {
        Material[] materials = _renderer.materials;

        materials[_materialIndexIn] = isInPowered ? _poweredIOMaterial : _unpoweredIOMaterial;
        materials[_materialIndexInversion] = !isInPowered ? _poweredInversionMaterial : _unpoweredInversionMaterial;
        materials[_materialIndexOut] = !isInPowered ? _poweredIOMaterial : _unpoweredIOMaterial;

        _renderer.materials = materials;
    }
}