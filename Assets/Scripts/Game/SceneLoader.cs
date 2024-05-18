using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public event Action SceneLoadingStarted;

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _progressText;

    public async void SwitchSceneAsync(string sceneName)
    {
        SceneLoadingStarted?.Invoke();

        _progressText.text = "0%";
        _canvasGroup.gameObject.SetActive(true);
        await PlayFadeAnimationAsync(0f, 1f);

        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        while (!op.isDone)
        {
            _progressText.text = $"{(int)(op.progress * 100)}%";
            await Task.Delay(100);
        }
        
        _progressText.text = "100%";
        await PlayFadeAnimationAsync(1f, 0f);
        _canvasGroup.gameObject.SetActive(false);
    }

    private async Task PlayFadeAnimationAsync(float initialAlpha, float targetAlpha)
    {
        var tcs = new TaskCompletionSource<bool>();
        _canvasGroup.alpha = initialAlpha;

        LeanTween.alphaCanvas(_canvasGroup, targetAlpha, 1f)
        .setOnComplete(() => tcs.SetResult(true));

        await tcs.Task;
    }
}
