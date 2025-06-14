using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public int inventorySize = 35;
    public int coins = 1000;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text coinText2;
    // Optional: Reference to coin image if you want to animate it
    [SerializeField] private Image coinImage;

    void Awake()
    {
        UpdateCoinDisplay();
    }

    public void UpdateCoinDisplay()
    {
        coinText.text = coins.ToString(); // Just the number, no "Coins:" prefix
        coinText2.text = coins.ToString();
    }

    public bool AddItem(Item item)
    {
        Item existingItem = items.Find(i => i.itemName == item.itemName);

        if (existingItem != null)
        {
            existingItem.itemQuantity += 1;
            coins = coins - item.itemPrice;
            UpdateCoinDisplay();
            return true;
        }
        else if (items.Count < inventorySize)
        {
            item.itemQuantity = 1;
            items.Add(item);
            coins = coins - item.itemPrice;
            UpdateCoinDisplay();
            return true;
        }
        else
        {
            Debug.Log("full");
            return false;
        }
    }
}
