using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUIHandler : MonoBehaviour
{
    private Sprite _startSprite;

    [Header("UI Elements")]
    [SerializeField] private Sprite _receivedStarSprite;
    [SerializeField] private Sprite starSprite;
    [SerializeField] private Transform _starsPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button nextButton;
    private void Awake()
    {

        _startSprite = _starsPanel.GetComponentInChildren<Image>().sprite;
    }

    public void ShowResult(bool isWin)
    {
        nextButton.gameObject.SetActive(isWin);
        resultText.text = isWin ? "Victory!" : "Defeat!";
    }
}
