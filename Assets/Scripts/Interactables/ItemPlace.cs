using UnityEngine;

public class ItemPlace : Interactable
{
    [SerializeField] private Item.ID _itemID;
    [SerializeField] private GameObject _placedGO;
    [SerializeField] private bool _isPlaced;

    public bool RequiredItemPlaced => _isPlaced;
    protected GameObject PlacedGO => _placedGO;

    private void Start()
    {
        _placedGO.SetActive(_isPlaced);
    }

    protected override bool ValidateInteractionInternal(Item item) => !_isPlaced && item != null && item.Id == _itemID;

    protected override void InteractInternal(InteractionData data)
    {
        PlaceItem(data);
    }

    protected void PlaceItem(InteractionData data)
    {
        if (ValidateInteraction(data.item))
        {
            _placedGO.SetActive(true);
            data.handler.Handle(this);
        }
    }
}
