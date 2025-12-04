using TMPro;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private MeshRenderer meshArrow;

    private void Awake()
    {
        PassengersController.OnChangePassengersCount += UpdateText;
        PassengersController.OnChangeCurrentColor += UpdateColor;
    }
    private void UpdateText<T>(T value)
    {
        text.text = value.ToString();
    }
    public void UpdateColor(Material colorMaterial)
    {
        meshArrow.material = colorMaterial;
    }
}
