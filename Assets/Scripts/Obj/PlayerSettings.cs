using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "GameSettings/Player")]
public class PlayerSettings : ScriptableObject
{
    [SerializeField] private int abilitiesCountInEditor = 0;
    public string Name = "Player";
    public int Money = 0;
    public int UnlockedLevels;

    private const string UNLOCKED_LEVELS_KEY = "UNLOCKED_LEVELS";
    private const string MONEY_KEY = "MONEY";
    private const int DEFAULT_ABILITY_COUNT_KEY = 3;

    private string Parametr(string key) => $"{Name}_{key}";
    public void LoadPlayerSettings()
    {
        UnlockedLevels = PlayerPrefs.GetInt(Parametr(UNLOCKED_LEVELS_KEY), 1);
        Money = PlayerPrefs.GetInt(Parametr(MONEY_KEY), 0);
    }
    public void SaveAllPlayerSettings()
    {
        PlayerPrefs.SetInt(Parametr(UNLOCKED_LEVELS_KEY), UnlockedLevels);
        PlayerPrefs.SetInt(Parametr(MONEY_KEY), Money);
        var abilities = AbilityHendler.GetAbilities();
        PlayerPrefs.Save();
    }
    public int GetAbilityCount(string abilityName)
    {
#if UNITY_EDITOR
        return abilitiesCountInEditor;
#else
        string key = Parametr(abilityName);
        return PlayerPrefs.GetInt(key, DEFAULT_ABILITY_COUNT_KEY);
#endif
    }
    public void UnlockLevel(int level)
    {
        if (level > UnlockedLevels)
        {
            UnlockedLevels = level;
            PlayerPrefs.SetInt(Parametr(UNLOCKED_LEVELS_KEY), UnlockedLevels);
            PlayerPrefs.Save();
        }
    }
    public void UpdateAbilityCount(string name, int value)
    {
        string key = Parametr(name);
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }
}
