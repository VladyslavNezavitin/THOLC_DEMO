using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ElectricalPanel : MonoBehaviour, IPowerController
{
	public event Action PowerOn, PowerOff;
	
    [SerializeField] private int _elementMatrixSize = 7;
    [SerializeField] private EPElement[] _elements;
    [SerializeField] private EPElement[] _exits;

    private bool _isPowered;
    public bool IsPowered
    {
        get => _isPowered;
        private set
        {
            if (_isPowered == value)
                return;

            if (_isPowered = value)
                PowerOn?.Invoke();
            else
                PowerOff?.Invoke();
        }
    }

    private void Start()
    {
        InitializeElements();
    }

    private void InitializeElements()
    {
        for (int i = 0, y = 0; y < _elementMatrixSize; y++)
        {
            for (int x = 0; x < _elementMatrixSize; x++, i++)
            {
                if (x > 0)
                {
                    EPElement.MakeEastWestNeighbours(
                        _elements[i], _elements[i - 1]);
                }

                if (y > 0)
                {
                    EPElement.MakeNorthSouthNeighbours(
                        _elements[i - _elementMatrixSize], _elements[i]);
                }

                if (_elements[i] != null)
                    _elements[i].Initialize(this);
            }
        }

        foreach (var element in _elements)
        {
            if (element != null)
                element.RecalculateConnections();
        }

        RecalculatePowered();
    }
    
    public void RecalculatePowered()
    {
        List<EPElement> passed = new List<EPElement>();

        foreach (var element in _elements)
        {
            if (element != null)
                element.CurrentInput = 0;
        }

        foreach (var emitter in _elements.Where(e => e != null && e.IsEmitter))
        {
            emitter.CurrentInput = emitter.CurrentInput;
            UpdateAllConductors(emitter, passed);
        }

        IsPowered = _exits.Any(t => t.CurrentInput > 0);
    }

    private void UpdateAllConductors(EPElement origin, List<EPElement> passed)
    {
        var queue = new Queue<EPElement>();
        queue.Enqueue(origin);
        passed.Add(origin);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var c in current.Connections)
            {
                bool canConduct = (c.Key.InputSides & c.Value.Offset(2) & current.CurrentOutput.Offset(2)) > 0;
                if (!canConduct)
                    continue;

                if (!passed.Contains(c.Key))
                {
                    queue.Enqueue(c.Key);
                    passed.Add(c.Key);

                    c.Key.CurrentInput = c.Key.Connections[current];
                }
                else
                {
                    if (c.Key is EPGate)
                        queue.Enqueue(c.Key);

                    c.Key.CurrentInput = c.Key.CurrentInput | c.Key.Connections[current];
                } 
            }
        }
    }
}