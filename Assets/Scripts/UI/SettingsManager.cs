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
    public GameObject closeThing; // A gomb vagy panel, ami látszik, ha a settings zárva van

    private Resolution[] resolutions;

    void Start()
    {
        // 1. FELBONTÁSOK BEOLVASÁSA ÉS DROPDOWN FELTÖLTÉSE
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        // Ellenőrizzük, van-e már mentett beállításunk
        bool hasSavedResolution = PlayerPrefs.HasKey("resolutionIndex");
        int savedIndex = PlayerPrefs.GetInt("resolutionIndex");

        for (int i = 0; i < resolutions.Length; i++)
        {
            // Formátum: "1920 x 1080"
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (hasSavedResolution)
            {
                // Ha van mentés, azt keressük meg a listában
                if (i == savedIndex)
                {
                    currentResolutionIndex = i;
                }
            }
            else
            {
                // HA NINCS MENTÉS (Első indítás):
                // Azt keressük, ami megegyezik a monitor jelenlegi felbontásával (Screen.currentResolution)
                // Ez általában a natív (legmagasabb) felbontás.
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

        // 2. EGYÉB BEÁLLÍTÁSOK BETÖLTÉSE (Hangerő, Fullscreen)
        LoadSettings(currentResolutionIndex);
    }

    public void LoadSettings(int currentResIndex)
    {
        // --- Hangerő betöltése ---
        // Ha nincs mentés, alapértelmezett (1f)
        float masterVol = PlayerPrefs.GetFloat("masterVolume", 1f);
        float musicVol = PlayerPrefs.GetFloat("musicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("sfxVolume", 1f);

        masterVolumeSlider.value = masterVol;
        musicVolumeSlider.value = musicVol;
        sfxVolumeSlider.value = sfxVol;

        // Fontos: A csúszkák beállítása nem hívja meg automatikusan a SetMasterVolume-ot induláskor,
        // ezért kézzel is be kell állítani a Mixert.
        SetMasterVolume(masterVol);
        SetMusicVolume(musicVol);
        SetSfxVolume(sfxVol);

        // --- Fullscreen betöltése ---
        // Alapértelmezett: 1 (True)
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        fullscreenToggle.isOn = isFullscreen;
        Screen.fullScreen = isFullscreen;

        // --- Felbontás érvényesítése ---
        // Ez biztosítja, hogy induláskor a játék tényleg átváltson a helyes felbontásra
        Resolution res = resolutions[currentResIndex];
        Screen.SetResolution(res.width, res.height, isFullscreen);
    }

    // --- AUDIO BEÁLLÍTÁSOK ---

    public void SetMasterVolume(float volume)
    {
        // Logaritmikus skála a decibelhez
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

    // --- GRAFIKAI BEÁLLÍTÁSOK ---

    public void SetResolution(int resolutionIndex)
    {
        // Biztonsági ellenőrzés
        if (resolutions == null || resolutionIndex < 0 || resolutionIndex >= resolutions.Length) return;

        Resolution resolution = resolutions[resolutionIndex];

        // Beállítjuk a felbontást
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // ELMENTJÜK a kiválasztott indexet
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        // ELMENTJÜK az állapotot (1 = true, 0 = false)
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    // --- MENÜ NYITÁS/ZÁRÁS ---

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