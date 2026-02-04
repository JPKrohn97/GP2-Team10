using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Mixer")]
    public AudioMixer mixer;

    private const string MASTER = "MasterVolume";
    private const string SFX = "SFXVolume";
    private const string MUSIC = "MusicVolume";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMaster(float value) => SetVolume(MASTER, value);
    public void SetSFX(float value) => SetVolume(SFX, value);
    public void SetMusic(float value) => SetVolume(MUSIC, value);

    void SetVolume(string param, float value)
    {
        float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        mixer.SetFloat(param, db);
    }
}
