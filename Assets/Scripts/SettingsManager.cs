using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private Toggler soundToggle;
    [SerializeField] private Toggler vibrationToggle;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource buttonsAudioSource;
    [SerializeField] private AudioClip testSFX;
    [SerializeField] public AudioMixerGroup DefaultOutput;

    public static bool IsVibrationEnabled { get; private set; } = true;
    public static bool IsSoundEnabled { get; private set; } = true;
    
    public static SettingsManager I;
    private float musicVolume = -18.0f;
    private float SFXVolume = -18.0f;

    private const string SOUND_ENABLED_KEY = "SoundEnabled";
    private const string VIBRATION_ENABLED_KEY = "MusicEnabled";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    
    private void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSlider.value = 10f;

        IsSoundEnabled = PlayerPrefs.GetInt(SOUND_ENABLED_KEY, 1) == 1;
        IsVibrationEnabled = PlayerPrefs.GetInt(VIBRATION_ENABLED_KEY, 1) == 1;
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, musicVolume);
        SFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, SFXVolume);
        
        HideWindow();
    }
    private void Start()
    {
        soundToggle.OnToggleChanged.AddListener(OnSoundToggleChanged);
        vibrationToggle.OnToggleChanged.AddListener(OnVibrationToggleChanged);
        musicSlider.transform.GetComponent<SliderHandler>().OnDragEnd.AddListener(OnMusicVolumeChanged);
        SFXSlider.transform.GetComponent<SliderHandler>().OnDragEnd.AddListener(OnSFXVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicValueChanged);
        SFXSlider.onValueChanged.AddListener(OnSFXValueChanged);
    }
    private void OnMusicValueChanged(float value)
    {
        audioMixer.SetFloat("MusicVolume", value);
    }
    private void OnSFXValueChanged(float value)
    {
        audioMixer.SetFloat("EffectsVolume", value);
    }
    private void OnSoundToggleChanged(bool value)
    {
        IsSoundEnabled = value;
        PlayerPrefs.SetInt(SOUND_ENABLED_KEY, IsSoundEnabled ? 1 : 0);
        PlayerPrefs.Save();
        audioMixer.SetFloat("MasterVolume", IsSoundEnabled ? 0f : -80f);
    }
    private void OnVibrationToggleChanged(bool value)
    {
        IsVibrationEnabled = value;
        PlayerPrefs.SetInt(VIBRATION_ENABLED_KEY, IsVibrationEnabled ? 1 : 0);
        PlayerPrefs.Save();
        GameManager.Vibrate();
    }
    
    private void OnMusicVolumeChanged()
    {
        musicVolume = musicSlider.value;
        Debug.Log("Music volume changed to: " + musicVolume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.Save();
    }
    private void OnSFXVolumeChanged()
    {
        SFXVolume = SFXSlider.value;
        Debug.Log("SFX volume changed to: " + SFXVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, SFXVolume);
        PlayerPrefs.Save();
        buttonsAudioSource.PlayOneShot(testSFX);
    }
    public void ShowWindow()
    {
        settingsCanvas.SetActive(true);
    }
    public void HideWindow()
    {
        settingsCanvas.SetActive(false);
    }
    public static void ChangeSound(AudioClip audio)
    {
        if (I == null || I.musicAudioSource == null)
        {
#if !UNITY_EDITOR
            Debug.LogError("SettingsManager or musicAudioSource is not initialized.");
            return;
#endif
        }
        else
        {
            I.musicAudioSource.clip = audio;
            I.musicAudioSource.Play();
        }
    }
    public static void UpdateVolumeValue()
    {
        I.soundToggle.Set(IsSoundEnabled);
        I.vibrationToggle.Set(IsVibrationEnabled);
        I.musicSlider.value = I.musicVolume;
        I.SFXSlider.value = I.SFXVolume;

        I.OnMusicValueChanged(I.musicVolume);
        I.OnSFXValueChanged(I.SFXVolume);
        I.audioMixer.SetFloat("MasterVolume", IsSoundEnabled ? 0f : -80f);
    }
}
