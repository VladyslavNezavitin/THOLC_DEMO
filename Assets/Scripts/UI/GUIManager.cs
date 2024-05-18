using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class GUIManager : MonoBehaviour
{
    [SerializeField] private OptionsMenuGUI _optionsMenuPrefab; 
    [SerializeField] private PauseMenuGUI _pauseMenuPrefab; 
    [SerializeField] private AchievementsMenuGUI _achievementsMenuPrefab; 

    private Stack<GUIWindow> _activeGUIs;


    private void Awake() => _activeGUIs = new Stack<GUIWindow>();
    public void LoadAndProcessOptionsMenu() => LoadAndProcessGUIAsync(_optionsMenuPrefab);
    public void LoadAndProcessPauseMenu() => LoadAndProcessGUIAsync(_pauseMenuPrefab);
    public void LoadAndProcessAchievementsMenu() => LoadAndProcessGUIAsync(_achievementsMenuPrefab);

    public void ExitAll()
    {
        while (_activeGUIs.Count > 0)
        {
            CloseTopOne();
            DestroyTopOne();
        }
    }

    public void CloseTopOne()
    {
        if (_activeGUIs.Count > 0)
        {
            var gui = _activeGUIs.Peek();
            gui.Deactivate();
        }
    }

    private void DestroyTopOne()
    {
        if (_activeGUIs.Count > 0)
            Destroy(_activeGUIs.Pop().gameObject);
    }

    private async void LoadAndProcessGUIAsync(GUIWindow prefab)
    {
        GUIWindow gui = Instantiate(prefab, transform);

        _activeGUIs.Push(gui);
        ShowTopOne();
        
        while (gui.IsActive)
            await Task.Delay(100);

        CloseTopOne();
        DestroyTopOne();

        ShowTopOne();
    }

    private void ShowTopOne()
    {
        if (_activeGUIs.Count == 0)
            return;

        foreach (var g in _activeGUIs)
            g.Hide();

        GUIWindow gui = _activeGUIs.Peek();
        gui.Show();
    }
}

