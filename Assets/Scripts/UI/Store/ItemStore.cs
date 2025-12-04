using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStore : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Button buyButton;
    [SerializeField] private int itemPrice;
    public void Initialize(string name, string description, Sprite image, int price)
    {
        itemImage.sprite = image;
        itemName.text = name;
        itemDescription.text = description;
        itemPrice = price;
        buyButton.GetComponentInChildren<TextMeshProUGUI>().text = price.ToString();
    }
}
