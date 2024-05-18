using UnityEngine;

[RequireComponent(typeof(PlayerInteractions))]
public class PlayerItem : MonoBehaviour
{
    [SerializeField] private Transform _itemPoint;

    private Transform _primaryItemParent;
    private InputManager _input;
    private PlayerInteractions _interactions;
    private bool _isFrozen;

    private Item _item;
    public Item Item => _item;

    private void Awake()
    {
        _input = ProjectContext.Instance.InputManager;
        _interactions = GetComponent<PlayerInteractions>();
    }

    private void OnEnable()
    {
        _input.DropItemTriggered += Input_OnItemDrop;
        _interactions.ControlObjectInteracted += Interactions_OnControlObjectInteracted;
        _interactions.InteractionExited += Interactions_OnInteractionExited;
    }

    private void OnDisable()
    {
        _input.DropItemTriggered -= Input_OnItemDrop;
        _interactions.ControlObjectInteracted -= Interactions_OnControlObjectInteracted;
        _interactions.InteractionExited -= Interactions_OnInteractionExited;
    }

    private void Interactions_OnInteractionExited() => _isFrozen = false;
    private void Interactions_OnControlObjectInteracted(InteractionControlData data)
    {
        if (data.freezeItem)
            _isFrozen = true;
    }

    private void Input_OnItemDrop() => DropItem();

    public void SetItem(Item item)
    {
        if (_isFrozen)
            return;

        if (_item != null)
            DropItem();

        if (item != null)
        {
            _primaryItemParent = item.transform.parent;

            _item = item;
            _item.Pick();

            _item.transform.SetParent(_itemPoint);
            _item.transform.localPosition = Vector3.zero;
            _item.transform.localEulerAngles = Vector3.zero;
        }
    }

    public void DropItem()
    {
        if (_item == null || _isFrozen)
            return;

        _item.transform.SetParent(_primaryItemParent);
        _item.transform.position = GetItemDropPosition();
        _item.Drop();

        _item = null;
    }

    public void DeleteItem()
    {
        if (_item == null || _isFrozen)
            return;

        Destroy(_item.gameObject);
        _item = null;
        _primaryItemParent = null;
    }

    private Vector3 GetItemDropPosition()
    {
        Vector3 itemDropPosition = _itemPoint.position;

        Vector3 castDirection = (_itemPoint.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, castDirection);
        float sphereRadius = 0.3f;

        float distance = Vector3.Distance(_itemPoint.position, transform.position);

        if (Physics.SphereCast(ray, sphereRadius, out var hitInfo, distance,
            Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            itemDropPosition = hitInfo.point - castDirection * sphereRadius / 2f;
        }

        return itemDropPosition;
    }
}