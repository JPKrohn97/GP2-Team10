using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("Scene Names")]
    public string gameSceneName = "Game";

    [Header("Audio")]
    public AudioMixer mainMixer;

    [Header("Sliders")]
    public Slider sldMaster;
    public Slider sldSFX;
    public Slider sldMusic;

    const string KEY_MASTER = "MasterVol";
    const string KEY_SFX = "SFXVol";
    const string KEY_MUSIC = "MusicVol";

    void Start()
    {
        LoadAudioSettings();
        

        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        SoundManager.Instance?.PlayMusic(SoundManager.Instance.MainMenuMusic);
    }

    public void PlayNewGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void CloseOptions()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit called (won't close in editor)");
    }

    // CALLED BY ACCEPT BUTTON
    public void ApplyAudioSettings()
    {
        ApplyMixerFromSliders();
        SaveSliderValues();
        CloseOptions();
    }

    void ApplyMixerFromSliders()
    {
        SetVolume("MasterVolume", sldMaster.value);
        SetVolume("SFXVolume", sldSFX.value);
        SetVolume("MusicVolume", sldMusic.value);
    }

    public void LoadAudioSettings()
    {
        float master = PlayerPrefs.GetFloat(KEY_MASTER, 1f);
        float sfx = PlayerPrefs.GetFloat(KEY_SFX, 1f);
        float music = PlayerPrefs.GetFloat(KEY_MUSIC, 1f);

        sldMaster.value = master;
        sldSFX.value = sfx;
        sldMusic.value = music;

        ApplyMixerFromSliders();
    }

    void SaveSliderValues()
    {
        PlayerPrefs.SetFloat(KEY_MASTER, sldMaster.value);
        PlayerPrefs.SetFloat(KEY_SFX, sldSFX.value);
        PlayerPrefs.SetFloat(KEY_MUSIC, sldMusic.value);
        PlayerPrefs.Save();
    }

    void SetVolume(string exposedParam, float sliderValue)
    {
        sliderValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);

        float db = Mathf.Log10(sliderValue) * 20f; // 
        mainMixer.SetFloat(exposedParam, db);
    }
}
