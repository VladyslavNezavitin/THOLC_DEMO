using UnityEngine;

[RequireComponent(typeof(Item))]
public class ItemSound : MonoBehaviour
{
    [SerializeField] private SoundPlaybackData _pickSound;
    [SerializeField] private SoundPlaybackData _hitSound;

    private SoundManager SoundManager => ProjectContext.Instance.SoundManager;
    private Item _item;

    private void Awake() => _item = GetComponent<Item>();

    private void OnEnable()
    {
        _item.Picked += Item_OnPicked;
        _item.ObjectHit += Item_OnObjectHit;
    }

    private void OnDisable()
    {
        _item.Picked -= Item_OnPicked;
        _item.ObjectHit -= Item_OnObjectHit;
    }

    private void Item_OnPicked() =>
        SoundManager.PlaySound(_pickSound, transform.position);

    private void Item_OnObjectHit(float fallDistance)
    {
        float volumeFactor = Mathf.InverseLerp(0f, 2f, fallDistance);

        var hitSound = _hitSound.GetClone();
        hitSound.volume = _hitSound.volume * volumeFactor;

        SoundManager.PlaySound(hitSound, transform.position);
    }
}
