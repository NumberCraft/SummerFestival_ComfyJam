using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer mixer;

    [SerializeField] private List<RenderPipelineAsset> shadowResolutions = new();
    [SerializeField] private Volume volume;

    //[SerializeField] private Camera cam;
    Resolution[] resolutions;

    private SettingsData settingsData = new();

    [Header("Audio")]
    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundVolumeSlider;

    [Header("Video")]
    [Header("Video")]
    [SerializeField] private TMP_Dropdown shadowDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [SerializeField] private Slider brightnessSlider;
    //[SerializeField] private Slider fovSlider;

    private void Awake()
    {
        if (volume == null)
        {
            volume = FindAnyObjectByType<Volume>();
        }
    }

    private void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();

        DataPersistenceManager.i.NewSettings();

        Load();
    }

    #region Audio
    public void SetMasterVolume(float v)
    {
        mixer.SetFloat("MasterVolume", v);
    }
    
    public void SetMusicVolume(float v)
    {
        mixer.SetFloat("MusicVolume", v);
    }
    
    public void SetSoundVolume(float v)
    {
        mixer.SetFloat("SoundVolume", v);
    }
    #endregion

    #region Video
    public void SetShadowQuality(int index)
    {
        QualitySettings.renderPipeline = shadowResolutions[index];
    }

    public void SetScreenMode(int index)
    {
        switch (index)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed; break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break;
        }
    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
    }

    public void SetBrightness(float v)
    {
        if (volume.profile.TryGet<ColorAdjustments>(out var adjustments))
            adjustments.postExposure.value = v;
    }

    /*public void SetFOV(float v)
    {
        if (Camera.main == null)
            return;

        if (Camera.main.orthographic)
        {
            Camera.main.orthographicSize = v;
        }
        else
        {
            Camera.main.fieldOfView = v;
        }
    }*/
    #endregion

    #region Game

    #endregion

    #region Save System
    public void Save()
    {
        DataPersistenceManager.i.ChangeSelectedProfileId("Settings");

        settingsData = new SettingsData(masterVolumeSlider.value, musicVolumeSlider.value, soundVolumeSlider.value, shadowDropdown.value, 
            screenModeDropdown.value, resolutionDropdown.value, brightnessSlider.value);

        DataPersistenceManager.i.SaveSettings(settingsData);
    }

    public void Load()
    {
        DataPersistenceManager.i.ChangeSelectedProfileId("Settings");
        DataPersistenceManager.i.LoadSettings(ref settingsData);

        masterVolumeSlider.value = settingsData.masterVolume;
        musicVolumeSlider.value = settingsData.musicVolume;
        soundVolumeSlider.value = settingsData.soundVolume;

        shadowDropdown.value = settingsData.shadowQuality;
        screenModeDropdown.value = settingsData.screenMode;
        resolutionDropdown.value = settingsData.resolution;

        brightnessSlider.value = settingsData.brightness;
        //fovSlider.value = settingsData.fov;

        SetMasterVolume(settingsData.masterVolume);
        SetMusicVolume(settingsData.musicVolume);
        SetSoundVolume(settingsData.soundVolume);

        SetShadowQuality(settingsData.shadowQuality);
        SetScreenMode(settingsData.screenMode);
        SetResolution(settingsData.resolution);

        SetBrightness(settingsData.brightness);
        //SetFOV(settingsData.fov);
    }
    #endregion
}
