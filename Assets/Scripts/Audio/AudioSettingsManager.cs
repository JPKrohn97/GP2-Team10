using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject optionsPanel;
    public Slider sldMaster;
    public Slider sldSFX;
    public Slider sldMusic;

    [Header("FMOD Bus Paths")]
    public string masterBusPath = "bus:/";
    public string sfxBusPath = "bus:/SFX";
    public string musicBusPath = "bus:/Music";

    Bus masterBus;
    Bus sfxBus;
    Bus musicBus;

    const string KEY_MASTER = "MasterVol";
    const string KEY_SFX = "SFXVol";
    const string KEY_MUSIC = "MusicVol";

    void Awake()
    {
        masterBus = RuntimeManager.GetBus(masterBusPath);
        sfxBus = RuntimeManager.GetBus(sfxBusPath);
        musicBus = RuntimeManager.GetBus(musicBusPath);
    }

    void Start()
    {
        LoadAudioSettings();
    }

    // Accept button calls this
    public void ApplyAndSave()
    {
        ApplyFromSliders();
        SaveSliderValues();
        CloseOptions();
    }

    public void CloseOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    public void ApplyFromSliders()
    {
        masterBus.setVolume(Mathf.Clamp01(sldMaster.value));
        sfxBus.setVolume(Mathf.Clamp01(sldSFX.value));
        musicBus.setVolume(Mathf.Clamp01(sldMusic.value));
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

        ApplyFromSliders(); 
    }

    
    public void OnMasterChanged(float v) { masterBus.setVolume(Mathf.Clamp01(v)); }
    public void OnSFXChanged(float v) { sfxBus.setVolume(Mathf.Clamp01(v)); }
    public void OnMusicChanged(float v) { musicBus.setVolume(Mathf.Clamp01(v)); }
}
