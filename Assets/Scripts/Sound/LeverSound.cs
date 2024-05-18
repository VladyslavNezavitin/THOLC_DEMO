using UnityEngine;

[RequireComponent(typeof(Lever))]
public class LeverSound : MonoBehaviour
{
    [SerializeField] private SoundPlaybackData _pullSound;
    private SoundManager SoundManager => ProjectContext.Instance.SoundManager;
    private Lever _lever;

    private void Awake() => _lever = GetComponent<Lever>();
    private void OnEnable() => _lever.Pulled += Lever_OnPulled;
    private void OnDisable() => _lever.Pulled -= Lever_OnPulled;

    private void Lever_OnPulled() =>
        SoundManager.PlaySound(_pullSound, transform.position);
}
