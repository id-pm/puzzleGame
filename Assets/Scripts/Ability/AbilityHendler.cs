using System.Collections.Generic;
using UnityEngine;

public class AbilityHendler : MonoBehaviour
{
    private static readonly Dictionary<IAbilityAction, int> abilitiesCountDictionary = new();

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            if (!child.TryGetComponent<IAbilityAction>(out IAbilityAction action))
            {
                Debug.LogError($"No IAbilityAction found on {child.name}");
                continue;
            }
            int count = GameManager.playerSettings.GetAbilityCount(action.Name);
            abilitiesCountDictionary.Add(action, count);
        }
        AbilityButton.ChangeValue += UpdateAbilityCount;

    }
    public static List<IAbilityAction> GetAbilities()
    {
        List<IAbilityAction> abilities = new();
        foreach(var dictionary in abilitiesCountDictionary)
        {
            abilities.Add(dictionary.Key);
        }
        return abilities;
    }

    private void OnDestroy()
    {
        AbilityButton.ChangeValue -= UpdateAbilityCount;
    }

    private void UpdateAbilityCount(IAbilityAction ability, int value)
    {
        abilitiesCountDictionary[ability] = value;
        GameManager.playerSettings.UpdateAbilityCount(ability.Name, value);
    }
}
