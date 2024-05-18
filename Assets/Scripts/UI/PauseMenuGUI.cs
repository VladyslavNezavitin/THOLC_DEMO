public class PauseMenuGUI : GUIWindow
{
    public void OnResumeButtonPressed() => Game.Instance.TogglePause();     // temp
    public void OnMainMenuButtonPressed() => Game.Instance.ExitGame();      // temp

    public void OnAchievementsButtonPressed() => 
        ProjectContext.Instance.GUIManager.LoadAndProcessAchievementsMenu();
    public void OnOptionsButtonPressed() =>
        ProjectContext.Instance.GUIManager.LoadAndProcessOptionsMenu();

}
