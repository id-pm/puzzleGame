using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Toggler : MonoBehaviour
{
    [SerializeField] Color colorOn = Color.green;
    [SerializeField] Color colorOff = Color.red;
    [SerializeField] Transform toggle;
    [SerializeField] float duration = 0.5f;
    [SerializeField] private bool state = true;

    public UnityEvent<bool> OnToggleChanged;

    private const float positionOn = 145f;
    private const float positionOff = -155f;
    private Image toggleImage;
    private Image background;
    private Sequence toggleSequenceOn;
    private Sequence toggleSequenceOff;

    #region implicit operators
    public static implicit operator bool(Toggler toggle)
    {
        return toggle.state;
    }
    #endregion
    private void Awake()
    {
        background = GetComponent<Image>();
        toggleImage = toggle.GetComponent<Image>();

        
        toggleSequenceOn = GetSequence(positionOn, colorOn);
        toggleSequenceOff = GetSequence(positionOff, colorOff);
    }
    public bool Set(bool value)
    {
        state = value;
        OnToggleChanged?.Invoke(state);
        UpdateToggleVisuals();
        return state;
    }
    private Sequence GetSequence(float position, Color color)
    {
        Sequence bounce = DOTween.Sequence()
            .Join(toggle.DOScale(0.8f, 0.1f).SetEase(Ease.OutQuad))
            .Append(toggle.DOScale(1.0f, 0.3f).SetEase(Ease.OutBack))
            .SetAutoKill(false);

        return DOTween.Sequence()
            .Join(toggleImage.DOColor(color, duration))
            .Join(background.DOColor(color, duration))
            .Join(toggle.DOLocalMoveX(position, duration).SetEase(Ease.OutCubic))
            .Join(bounce)
            .SetAutoKill(false);
    }

    public void Toggle()
    {
        state = !state;
        OnToggleChanged?.Invoke(state);
        UpdateToggleVisuals();
    }

    private void UpdateToggleVisuals()
    {
        if (state)
        {

           toggleSequenceOn.Restart();
        }
        else
        {
            toggleSequenceOff.Restart();
        }
    }

}
