using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
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
    }

    // CALLED BY ACCEPT BUTTON
    public void ApplyAudioSettings()
    {
        ApplyMixerFromSliders();
        SaveSliderValues();
        CloseOptions();
    }


    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    void ApplyMixerFromSliders()
    {
        SetVolume("MasterVolume", sldMaster.value);
        SetVolume("SFXVolume", sldSFX.value);
        SetVolume("MusicVolume", sldMusic.value);
    }

    void SaveSliderValues()
    {
        PlayerPrefs.SetFloat(KEY_MASTER, sldMaster.value);
        PlayerPrefs.SetFloat(KEY_SFX, sldSFX.value);
        PlayerPrefs.SetFloat(KEY_MUSIC, sldMusic.value);
        PlayerPrefs.Save();
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

    void SetVolume(string exposedParam, float sliderValue)
    {
       
        sliderValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);

        float db = Mathf.Log10(sliderValue) * 20f; // 
        mainMixer.SetFloat(exposedParam, db);
    }
}
