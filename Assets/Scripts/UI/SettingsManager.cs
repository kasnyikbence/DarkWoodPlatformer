using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public AudioMixer mainMixer;

    [Header("Video")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    [Header("Menu Elements")]
    public GameObject settingsMenu;
    public GameObject closeThing;

    private Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        bool hasSavedResolution = PlayerPrefs.HasKey("resolutionIndex");
        int savedIndex = PlayerPrefs.GetInt("resolutionIndex");

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (hasSavedResolution)
            {
                if (i == savedIndex)
                {
                    currentResolutionIndex = i;
                }
            }
            else
            {
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        LoadSettings(currentResolutionIndex);
    }

    public void LoadSettings(int currentResIndex)
    {
        float masterVol = PlayerPrefs.GetFloat("masterVolume", 1f);
        float musicVol = PlayerPrefs.GetFloat("musicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("sfxVolume", 1f);

        masterVolumeSlider.value = masterVol;
        musicVolumeSlider.value = musicVol;
        sfxVolumeSlider.value = sfxVol;

        SetMasterVolume(masterVol);
        SetMusicVolume(musicVol);
        SetSfxVolume(sfxVol);

        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        fullscreenToggle.isOn = isFullscreen;
        Screen.fullScreen = isFullscreen;

        Resolution res = resolutions[currentResIndex];
        Screen.SetResolution(res.width, res.height, isFullscreen);
    }

    public void SetMasterVolume(float volume)
    {
        float db = (volume <= 0.0001f) ? -80f : Mathf.Log10(volume) * 12;
        mainMixer.SetFloat("MasterVolume", db);
        PlayerPrefs.SetFloat("masterVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        float db = (volume <= 0.0001f) ? -80f : Mathf.Log10(volume) * 12;
        mainMixer.SetFloat("MusicVolume", db);
        PlayerPrefs.SetFloat("musicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float volume)
    {
        float db = (volume <= 0.0001f) ? -80f : Mathf.Log10(volume) * 12;
        mainMixer.SetFloat("SfxVolume", db);
        PlayerPrefs.SetFloat("sfxVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutions == null || resolutionIndex < 0 || resolutionIndex >= resolutions.Length) return;

        Resolution resolution = resolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OpenSettingsMenu()
    {
        settingsMenu.SetActive(true);
        if (closeThing != null) closeThing.SetActive(false);
    }

    public void CloseSettingsMenu()
    {
        settingsMenu.SetActive(false);
        if (closeThing != null) closeThing.SetActive(true);
    }
}