using UnityEngine;

public class Keycard : Item
{
    [SerializeField] private string _key;
    public string Key => _key;
}

