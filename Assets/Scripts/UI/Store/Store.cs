using UnityEngine;

public class Store : MonoBehaviour
{
    [SerializeField] private Transform storePanel;
    [SerializeField] private ItemStore itemPrefab;
    [SerializeField] private int itemPrice = 200; // Example price, can be adjusted or set per item
    void Start()
    {
        AbilityHendler.GetAbilities().ForEach(ability =>
        {
            ItemStore item = Instantiate(itemPrefab, storePanel);
            item.Initialize(ability.Name, ability.Description, ability.Icon, itemPrice);
        });
    }
}
