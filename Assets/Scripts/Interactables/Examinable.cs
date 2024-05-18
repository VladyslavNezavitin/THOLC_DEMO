using System.Collections;
using UnityEngine;

public sealed class Examinable : Interactable, IControlInteractable
{
    [SerializeField] private float _rotationSpeed = 100f;
    private GameObject _clone;
    private bool _examining;

    public InteractionControlData GetControlData() => new InteractionControlData()
    { freezeMovement = true,  freezeRotation = true };

    protected override void InteractInternal(InteractionData data)
    {
        if (!_examining)
            StartCoroutine(ExaminationRoutine(data));

        data.handler.Handle(this);
    }

    private IEnumerator ExaminationRoutine(InteractionData data)
    {
        BeginExamination();

        while (_examining)
        {
            if (data.isExitRequested)
            {
                EndExamination();
                yield break;
            }

            Vector3 rotation = new Vector3()
            {
                x = data.input.y * _rotationSpeed * Time.deltaTime,
                y = -data.input.x * _rotationSpeed * Time.deltaTime,
                z = 0f
            };

            transform.position = data.examinationPoint.position;
            transform.RotateAround(data.examinationPoint.position, data.examinationPoint.up, rotation.y);
            transform.RotateAround(data.examinationPoint.position, data.examinationPoint.right, rotation.x);

            yield return null;
        }

        EndExamination();
    }

    private void BeginExamination()
    {
        _clone = Instantiate(gameObject);
        _clone.transform.parent = transform.parent;
        _clone.transform.position = transform.position;
        _clone.transform.rotation = transform.rotation;
        _clone.name = gameObject.name;
        _clone.SetActive(false);

        gameObject.SetLayer(Constants.LAYER_RENDER_ON_TOP);
        _examining = true;
    }

    private void EndExamination()
    {
        Destroy(gameObject);
        _clone.SetActive(true);
        _clone = null;

        _examining = false;
    }
}
