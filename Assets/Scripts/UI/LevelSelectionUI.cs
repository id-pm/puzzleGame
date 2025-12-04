using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private GameObject lockedLevelButtonPrefab;
    [SerializeField] private Transform levelsContainer;
    [SerializeField] private ScrollRect scrollRect;

    

    void Start()
    {
        for (int i = 0; i < GameManager.playerSettings.UnlockedLevels; i++)
        {
            int levelIndex = i;
            GameObject levelButton = Instantiate(levelButtonPrefab, levelsContainer);
            levelButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = (levelIndex + 1).ToString();
            levelButton.name = "Level " + (levelIndex + 1);
            levelButton.GetComponent<Button>().onClick.AddListener(() => GameManager.LoadGame(levelIndex));
        }
        int toAdd = (3 - (GameManager.playerSettings.UnlockedLevels % 3)) % 3;
        for(int i = 0; i < toAdd; i++)
        {
            Instantiate(lockedLevelButtonPrefab, levelsContainer);
        }
        for(int i = 0; i < 15; i++)
        {
            Instantiate(lockedLevelButtonPrefab, levelsContainer);
        }
        StartCoroutine(DisableLayoutNextFrame(GameManager.playerSettings.UnlockedLevels + toAdd + 3));
    }
    IEnumerator DisableLayoutNextFrame(int firstIndex)
    {
        yield return null;
        for (int i = firstIndex; i < levelsContainer.childCount; i++)
        {
            levelsContainer.GetChild(i).GetComponent<LayoutElement>().ignoreLayout = true;
        }
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;

    }
}
