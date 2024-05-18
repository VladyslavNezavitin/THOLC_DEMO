using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Lever))]
public class LeverRope : MonoBehaviour
{
    [SerializeField] private Transform _rope;
    [SerializeField] private Transform _ropePivot;
    private Lever _lever;

    private bool _processingMovement;
    Vector3 _primaryOffset;


    private void Awake()
    {
        _lever = GetComponent<Lever>();
        _primaryOffset = _rope.position - _ropePivot.position;
    }

    private void OnEnable()
    {
        _lever.Pulled += Lever_OnPulled;
        _lever.AnimationStopped += Lever_OnAnimationStopped;
    }

    private void OnDisable()
    {
        _lever.Pulled -= Lever_OnPulled;
        _lever.AnimationStopped -= Lever_OnAnimationStopped;
    }

    private void Lever_OnPulled()
    {
        _processingMovement = true;
        StartCoroutine(RopeMovementRoutine());
    }

    private void Lever_OnAnimationStopped()
    {
        _processingMovement = false;
    }

    private IEnumerator RopeMovementRoutine()
    {

        while (_processingMovement)
        {
            _rope.position = _ropePivot.position + _primaryOffset;
            yield return null;
        }

        _rope.position = _ropePivot.position + _primaryOffset;
    }
}
