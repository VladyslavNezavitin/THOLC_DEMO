using UnityEngine;

public abstract class GUIWindow : MonoBehaviour
{
    public bool IsActive { get; private set; }
    public bool IsShown { get; private set; }

    private void Awake()
    {
        IsActive = true;
        Hide();
    }

    public void Hide()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        IsShown = false;
    }

    public void Show()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(true);

        IsShown = true;
    }

    public void Deactivate()
    {
        Hide();
        IsActive = false;
    }
}