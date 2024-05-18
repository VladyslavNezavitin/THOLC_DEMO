using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Startup : MonoBehaviour
{
    private void Start()
    {
        ProjectContext.Instance.SceneLoader.SwitchSceneAsync(Constants.SCENE_MAIN_MENU);
    }
}
