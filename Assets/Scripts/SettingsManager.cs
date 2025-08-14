using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    // Audió
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public AudioMixer mainMixer;
    // Kijelző
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    private Resolution[] resolutions;

    void Start()
    {
        // Felbontások listájának feltöltése a Dropdown-ba
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Beállítások betöltése a mentett értékekből
        LoadSettings();
    }

    public void LoadSettings()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfxVolume", 1f);

        SetMasterVolume(masterVolumeSlider.value);
        SetMusicVolume(musicVolumeSlider.value);
        SetSfxVolume(sfxVolumeSlider.value);

        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;
    }

    public void SetMasterVolume(float volume)
    {
        if (volume == 0)
        {
            mainMixer.SetFloat("MasterVolume", -80f);
        }
        else
        {
            mainMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 12);
        }
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        if (volume == 0)
        {
            mainMixer.SetFloat("MusicVolume", -80f);
        }
        else
        {
            mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 12);
        }
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSfxVolume(float volume)
    {
        if (volume == 0)
        {
            mainMixer.SetFloat("SfxVolume", -80f);
        }
        else
        {
            mainMixer.SetFloat("SfxVolume", Mathf.Log10(volume) * 12);
        }
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }


}