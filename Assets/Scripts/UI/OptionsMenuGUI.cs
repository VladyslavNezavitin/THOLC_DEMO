using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuGUI : GUIWindow
{
	[SerializeField] private Slider _masterVolumeSlider;
	[SerializeField] private Slider _musicVolumeSlider;
	[SerializeField] private Slider _sfxVolumeSlider;

    private void Start()
    {
        _masterVolumeSlider.value = PlayerPrefs.GetFloat(Constants.MIXER_MASTER_VOLUME);
		_musicVolumeSlider.value = PlayerPrefs.GetFloat(Constants.MIXER_MUSIC_VOLUME);
		_sfxVolumeSlider.value = PlayerPrefs.GetFloat(Constants.MIXER_SFX_VOLUME);
    }

    public void OnBackButtonPressed() => Deactivate();

	public void OnMasterVolumeSliderChanged() =>
		ProjectContext.Instance.SoundManager.SetMasterVolume(_masterVolumeSlider.value);

	public void OnEffectsVolumeSliderChanged() =>
		ProjectContext.Instance.SoundManager.SetSFXVolume(_sfxVolumeSlider.value);

	public void OnMusicVolumeSliderChanged() =>
		ProjectContext.Instance.SoundManager.SetMusicVolume(_musicVolumeSlider.value);
}