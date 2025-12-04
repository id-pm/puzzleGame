using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Button buttonCount;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI textCount;
    [SerializeField] private bool clicked = false;
    private IAbilityAction _currentAbility;
    private CanvasGroup canvasGroup;
    private readonly ObservableProperty<int> count = new();
    public static System.Action<string> OnHoverEnter;
    public static System.Action OnHoverExit;
    public static System.Action<IAbilityAction, int> ChangeValue;

    private void Awake()
    {
        button.onClick.AddListener(ButtonClick);
        buttonCount.onClick.AddListener(CounterClick);
        button.onClick.AddListener(UpdateCountDisplay);
        buttonCount.onClick.AddListener(UpdateCountDisplay);
        UpdateCountDisplay();
        canvasGroup = gameObject.GetComponentInParent<CanvasGroup>();
    }

    public void Bind(IAbilityAction ability, string text, Sprite sprite, int count)
    {
        _currentAbility = ability;
        _currentAbility.OnActionStarted += OnAbilityStart;
        _currentAbility.OnActionCompleted += OnAbilityComplete;
        this.text.text = text;
#if UNITY_EDITOR
        this.count.Value = 3; // For testing in the editor, set a default value
#else
        this.count.Value = count;
#endif
        icon.sprite = sprite;
        this.count.OnValueChanged += (value) => 
        {
            ChangeValue?.Invoke(_currentAbility, value);
        };
        UpdateCountDisplay();
    }

    private void OnAbilityStart()
    {
        LevelHendler.State = GameState.MoveCar;
        canvasGroup.interactable = false;
        OnHoverExit?.Invoke();
    }
    private void OnAbilityComplete()
    {
        canvasGroup.interactable = true;
        count.Value--;
        clicked = false;
        UpdateCountDisplay();
        LevelHendler.State = GameState.Idle;
    }

    private void UpdateCountDisplay()
    {
        if (clicked)
        {
            textCount.text = "X";
        }
        else if (count == 0)
        {
            textCount.text = "+";
        }
        else
        {
            textCount.text = count.ToString();
        }
    }
    
    private void ButtonClick()
    {
        if (clicked)
        {
            clicked = false;
            _currentAbility.OnAbilityCancel();
            OnHoverExit?.Invoke();
        }
        else
        {
            if(count == 0)
            {
                OpenStore();
                return;
            }
            clicked = true;
            _currentAbility.OnAbilityStart();
            OnHoverEnter?.Invoke(_currentAbility.Description);
        }
    }
    private void CounterClick()
    {
        if (clicked)
        {
            clicked = false;
            _currentAbility.OnAbilityCancel();
        }
        else
        {
            OpenStore();
        }
    }
    private void OpenStore()
    {
        Debug.Log("Открываем магазин...");
        // TODO: вызвать окно магазина
    }
}
