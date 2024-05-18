using System;
using System.Collections;
using UnityEngine;


public class Ladder : Interactable, IControlInteractable
{
    public event Action<Vector3> Step;

    [SerializeField] private float _distanceBetweenSegments;
    [SerializeField] private float _inOutPointsOffset;          // player retreat distance on exit
    [SerializeField] private float _pivotOffset;                // distance from the ladder Y bounds on setup
    [SerializeField] private float _stepCooldown;               // how fast the player is climbing
    [SerializeField] private float _alternateShiftDistance;     // alternate pivot shift from the center on each step
    [SerializeField] private GameObject _segmentPrefab;
    [SerializeField] private Vector3 _segmentRotation;
    [SerializeField] private Transform _segmentRoot;
    [SerializeField] private Transform _pivot;
    
    private BoxCollider _collider;
    private float _stepTimer;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
        GenerateSegments();
    }

    private void OnValidate() =>
        _distanceBetweenSegments = Mathf.Clamp(_distanceBetweenSegments, 0.1f, Mathf.Infinity);

    public InteractionControlData GetControlData() => new InteractionControlData()
    {
        objectToFollow = _pivot,
        lookConstraints = new Vector2(90, 90),
        preventExitOnRequest = true,
        freezeItem = true
    };

    protected override void InteractInternal(InteractionData data)
    {
        SetupPivot(data.playerPosition.y);
        data.handler.Handle(this);
        StartCoroutine(LadderRoutine(data));
    }

    private void SetupPivot(float playerPositionY)
    {
        float pivotBorderOffset = _inOutPointsOffset + 0.3f;
        Vector3 centerWorldSpace = transform.TransformPoint(_collider.center);

        float minPoint = centerWorldSpace.y - _collider.size.y / 2f + pivotBorderOffset;
        float maxPoint = centerWorldSpace.y + _collider.size.y / 2f - pivotBorderOffset;

        Vector3 pivotPosition = centerWorldSpace;
        pivotPosition.y = Mathf.Clamp(playerPositionY, minPoint, maxPoint);
        pivotPosition -= _pivot.forward * _pivotOffset;

        _pivot.position = pivotPosition;
    }

    private IEnumerator LadderRoutine(InteractionData data)
    {
        _collider.enabled = false;

        Vector3 centerWorldSpace = transform.TransformPoint(_collider.center);
        Vector3 bottom = centerWorldSpace + Vector3.down * _collider.size.y / 2f;
        Vector3 top = centerWorldSpace + Vector3.up * _collider.size.y / 2f;
        Vector2 pivotPositionXZ = new Vector3(_pivot.position.x, _pivot.position.z);
        Vector3 finalPosition;
        bool alternateStep = false;

        while (true)
        {
            if (_stepTimer <= 0f && Mathf.Abs(data.inputRaw.y) > 0.1)
            {
                _stepTimer = _stepCooldown;

                float nextPlayerFeetPos = _pivot.position.y - _inOutPointsOffset +
                    data.inputRaw.y * _distanceBetweenSegments;

                if (nextPlayerFeetPos < bottom.y)
                {
                    finalPosition = bottom - _pivot.forward + _pivot.up * _inOutPointsOffset;
                    break;
                }

                if (nextPlayerFeetPos >= top.y)
                {
                    finalPosition = top + _pivot.forward + _pivot.up * _inOutPointsOffset;
                    break;
                }

                Vector3 pivotPosition = new Vector3(pivotPositionXZ.x, _pivot.position.y, pivotPositionXZ.y);
                Vector3 targetPosition = pivotPosition + _pivot.up * data.inputRaw.y * _distanceBetweenSegments;
                targetPosition += _pivot.right * _alternateShiftDistance * (alternateStep ? -1 : 1);
                alternateStep = !alternateStep;
                
                LeanTween.move(_pivot.gameObject, targetPosition, _stepCooldown / 1.5f)
                .setEaseInOutQuad();

                Step?.Invoke(pivotPosition);
            }

            _stepTimer -= Time.deltaTime;
            yield return null;
        }

        yield return StartCoroutine(MovePivotRoutine(finalPosition));
        data.forceExitCallback.Invoke();

        _collider.enabled = true;
    }

    private IEnumerator MovePivotRoutine(Vector3 pivotTargetPosition)
    {
        float t = 0f;
        float currentVelocity = 0f;
        float smoothTime = 0.33f;

        Vector3 pivotStartPosition = _pivot.transform.position;

        while (t < 0.99f)
        {
            t = Mathf.SmoothDamp(t, 1f, ref currentVelocity, smoothTime);
            Vector3 currentPosition = Vector3.Lerp(pivotStartPosition, pivotTargetPosition, t);

            _pivot.position = currentPosition;
            yield return null;
        }

        _pivot.position = pivotTargetPosition;
    }

    private void GenerateSegments()
    {
        foreach (Transform segment in _segmentRoot)
            Destroy(segment.gameObject);

        int segmentsCount = Mathf.RoundToInt(_collider.size.y / _distanceBetweenSegments);

        for (int i = 0; i < segmentsCount; i++)
        {
            Transform segment = Instantiate(_segmentPrefab, _segmentRoot).transform;
            segment.position = transform.position + Vector3.up * _distanceBetweenSegments * i;
            segment.rotation = transform.rotation * Quaternion.Euler(_segmentRotation);
        }
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawCube(collider.center, collider.size);
    }
}
