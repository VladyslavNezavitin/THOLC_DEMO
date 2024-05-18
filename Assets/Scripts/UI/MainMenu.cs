using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void OnPlayButtonPressed()
    {
        ProjectContext.Instance.SceneLoader.SwitchSceneAsync(Constants.SCENE_DEMO);
    }

    public void OnAchievementsButtonPressed()
    {
        ProjectContext.Instance.GUIManager.LoadAndProcessAchievementsMenu();
    }

    public void OnOptionsButtonPressed()
    {
        ProjectContext.Instance.GUIManager.LoadAndProcessOptionsMenu();
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
}
