using UnityEngine;

public class EPTarget : EPElement
{
	public bool IsFulfilled { get; private set; } 

	[SerializeField] private EPElementView _view;

    protected override void OnInputChanged() => _view.IsPowered = CurrentInput > 0;

}