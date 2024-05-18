using UnityEngine;

public class EPElementView : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private int _materialIndex;
    [SerializeField] private Material _poweredMaterial;
    [SerializeField] private Material _unpoweredMaterial;

    private bool _isPowered;
    public bool IsPowered
    {
        get => _isPowered;
        set
        {
            _isPowered = value;
            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        Material[] materials = _renderer.materials;
        materials[_materialIndex] = IsPowered ? _poweredMaterial : _unpoweredMaterial;

        _renderer.materials = materials;
    }
}