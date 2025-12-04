using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Transform abilityPanel;
    [SerializeField] private Transform backgroundPanel;
    [SerializeField] private Button settingsButton;
    [SerializeField] private AbilityButton abilityButtonPrefab;
    [SerializeField] private GameObject abilityTextInfoTransform;
    [SerializeField] private ResultUIHandler resultUIHandler;
    [SerializeField] private Store store;
    private TextMeshProUGUI abilityTextInfo;
    private SettingsManager settingsManager;

    void Start()
    {
        abilityTextInfoTransform.SetActive(false);
        backgroundPanel.gameObject.SetActive(false);
        resultUIHandler.gameObject.SetActive(false);
        store.gameObject.SetActive(false);

        abilityTextInfo = abilityTextInfoTransform.GetComponentInChildren<TextMeshProUGUI>();

        var abilities = AbilityHendler.GetAbilities();
        foreach (IAbilityAction action in abilities)
        {
            CreateAbilityButton(action, action.Name, action.Icon);
        }

        settingsManager = SettingsManager.I;
        settingsManager.HideWindow();
        settingsButton.onClick.AddListener(() =>
        {
            settingsManager.ShowWindow();
        });

        AbilityButton.OnHoverEnter += ShowAbilityDescription;
        AbilityButton.OnHoverExit += HideAbilityDescription;
    }
    private void CreateAbilityButton(IAbilityAction ability, string text, Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogError("Sprite not found");
            return;
        }
        AbilityButton button = Instantiate(abilityButtonPrefab, abilityPanel);
        button.Bind(ability, text, sprite, GameManager.playerSettings.GetAbilityCount(ability.Name));
    }

    private void ShowAbilityDescription(string desc)
    {
        abilityTextInfo.text = desc;
        abilityTextInfoTransform.SetActive(true);
    }

    private void HideAbilityDescription()
    {
        abilityTextInfoTransform.SetActive(false);
    }
    public void ShowResult(bool isWin)
    {
        resultUIHandler.gameObject.SetActive(true);
        resultUIHandler.ShowResult(isWin);
    }
    public void HideResult()
    {
        resultUIHandler.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        AbilityButton.OnHoverEnter -= ShowAbilityDescription;
        AbilityButton.OnHoverExit -= HideAbilityDescription;
    }


}
