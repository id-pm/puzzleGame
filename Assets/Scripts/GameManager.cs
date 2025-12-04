using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] dontDestroyObjects;
    [SerializeField] private AudioSource buttonAudioSource;
    public static int currentLevel = -1;
    public static PlayerSettings playerSettings;
    public static GameManager I { get; private set; }
    private GameManager() { }
    void Awake(){
        I = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerSettings = Resources.Load<PlayerSettings>("PlayerSettings");
        playerSettings.LoadPlayerSettings();
    }
    private void Start()
    {
        foreach (GameObject obj in dontDestroyObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
        SettingsManager.UpdateVolumeValue();
    }

    public static void LoadGame(int level)
    {
        currentLevel = level;
        SceneManager.LoadScene("Game");
    }
    public static void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public static void Vibrate()
    {
        if (SettingsManager.IsVibrationEnabled)
            Handheld.Vibrate();
    }
    public static void Exit()
    {
        Application.Quit();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        Debug.Log($"Found {allButtons.Length} buttons in the scene.");
        foreach (Button button in allButtons)
        {
            button.onClick.AddListener(() => 
            {
                buttonAudioSource.PlayOneShot(buttonAudioSource.clip);
            });
        }
    }
    public static void UnlockLevel()
    {
        int level = currentLevel + 1;
        playerSettings.UnlockLevel(level);
    }
}
