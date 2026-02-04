using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPauseGame : MonoBehaviour
{
    [Header("Panels")]
    public GameObject gamePanel;
    public GameObject optionsPanel;

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

        gamePanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        gamePanel.SetActive(true);
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        gamePanel.SetActive(false);
    }

    public void OpenOptions()
    {
        Time.timeScale = 0f;
        //gamePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        Time.timeScale = 0f;
        //gamePanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
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

    void LoadAudioSettings()
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
