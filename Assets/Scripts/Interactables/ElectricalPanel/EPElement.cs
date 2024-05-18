using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EPElement : MonoBehaviour
{
    [SerializeField] private EPDirection _inputSides;
    [SerializeField] private EPDirection _outputSides;
    [SerializeField] private int _constantIOOffset;
    [SerializeField] private bool _isEmitter; 
     
    private EPElement _north, _south, _east, _west;
    private ElectricalPanel _panel;
    private Dictionary<EPElement, EPDirection> _connections;

    private EPDirection _currentInput;
    private int _currentIOOffset;

    public bool IsEmitter => _isEmitter;
    public IReadOnlyDictionary<EPElement, EPDirection> Connections => _connections;
    public EPDirection InputSides => _inputSides.Offset(CurrentIOOffset);
    public EPDirection OutputSides => _outputSides.Offset(CurrentIOOffset);

    public EPDirection CurrentOutput { get; set; }
    public EPDirection CurrentInput
    {
        get => _currentInput;
        set
        {
            _currentInput = value;
            OnInputChanged();
        }
    }
    protected int CurrentIOOffset
    {
        get => _currentIOOffset;
        set => _currentIOOffset = value + _constantIOOffset;
    }

    public static void MakeNorthSouthNeighbours(EPElement north, EPElement south)
    {
        if (north != null)
            north._south = south;

        if (south != null)
            south._north = north;
    }

    public static void MakeEastWestNeighbours(EPElement east, EPElement west)
    {
        if (east != null)
            east._west = west;

        if (west != null)
            west._east = east;
    }

    public void Initialize(ElectricalPanel panel)
    {
        _panel = panel;
        _connections = new Dictionary<EPElement, EPDirection>();

        RecalculateIOOffset();
    }

    protected virtual void OnInputChanged() { }
    protected virtual void RecalculateIOOffset() { CurrentIOOffset = 0; }

    public void RecalculateConnections()
    {
        ClearConnections();

        bool canConnectNorth =
            (CanConductFrom(EPDirection.North) && _north.CanConductTo(EPDirection.South)) ||
            (CanConductTo(EPDirection.North) && _north.CanConductFrom(EPDirection.South));

        bool canConnectSouth =
            (CanConductFrom(EPDirection.South) && _south.CanConductTo(EPDirection.North)) ||
            (CanConductTo(EPDirection.South) && _south.CanConductFrom(EPDirection.North));

        bool canConnectEast =
            (CanConductFrom(EPDirection.East) && _east.CanConductTo(EPDirection.West)) ||
            (CanConductTo(EPDirection.East) && _east.CanConductFrom(EPDirection.West));

        bool canConnectWest =
            (CanConductFrom(EPDirection.West) && _west.CanConductTo(EPDirection.East)) ||
            (CanConductTo(EPDirection.West) && _west.CanConductFrom(EPDirection.East));

        if (canConnectNorth) Connect(_north, EPDirection.North);
        if (canConnectSouth) Connect(_south, EPDirection.South);
        if (canConnectEast) Connect(_east, EPDirection.East);
        if (canConnectWest) Connect(_west, EPDirection.West);

        _panel.RecalculatePowered();
    }

    private bool CanConductFrom(EPDirection from)
    {
        switch (from)
        {
            case EPDirection.North: return (InputSides & EPDirection.North) != 0 && _north != null;
            case EPDirection.South: return (InputSides & EPDirection.South) != 0 && _south != null;
            case EPDirection.East: return (InputSides & EPDirection.East) != 0 && _east != null;
            case EPDirection.West: return (InputSides & EPDirection.West) != 0 && _west != null;
        }

        return false;
    }

    private bool CanConductTo(EPDirection to)
    {
        switch (to)
        {
            case EPDirection.North: return (OutputSides & EPDirection.North) != 0 && _north != null;
            case EPDirection.South: return (OutputSides & EPDirection.South) != 0 && _south != null;
            case EPDirection.East: return (OutputSides & EPDirection.East) != 0 && _east != null;
            case EPDirection.West: return (OutputSides & EPDirection.West) != 0 && _west != null;
        }

        return false;
    }

    private void Connect(EPElement target, EPDirection direction)
    {
        if (!_connections.ContainsKey(target))
            _connections.Add(target, direction);

        if (!target._connections.ContainsKey(this))
            target._connections.Add(this, direction.Offset(2));
    }

    private void ClearConnections()
    {
        foreach (var c in _connections.Keys)
            c._connections.Remove(this);

        _connections.Clear();
    }

    [ContextMenu("Debug Info")]
    private void ShowDebugInfo()
    {
        Debug.Log($"Current input: " + CurrentInput);
        Debug.Log($"Current output: " + CurrentOutput);

        Debug.Log("===== Connections =====");
        foreach (var c in Connections)
            Debug.Log(c);
        Debug.Log("===== Neighbours =====");
        Debug.Log("North: " + _north);
        Debug.Log("South: " + _south);
        Debug.Log("East: " + _east);
        Debug.Log("West: " + _west);
    }
}

[Flags]
public enum EPDirection
{
    North = 1,
    East = 2,
    South = 4,
    West = 8
}