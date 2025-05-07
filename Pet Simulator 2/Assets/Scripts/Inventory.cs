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

    void Awake()
    {
        coinText.text = "Coins: " + coins;
    }
    
    public bool AddItem(Item item)
    {
        // First check if the item already exists in inventory
        Item existingItem = items.Find(i => i.itemName == item.itemName);
        
        if (existingItem != null)
        {
            // Item already exists, increase its quantity
            existingItem.itemQuantity += 1;
            coins = coins - item.itemPrice;
            coinText.text = "Coins: " + coins;
            return true;
        }
        else if (items.Count < inventorySize)
        {
            // Item doesn't exist yet and we have space
            // Set initial quantity to 1
            item.itemQuantity = 1;
            items.Add(item);
            coins = coins - item.itemPrice;
            coinText.text = "Coins: " + coins;
            return true;
        }
        else
        {
            // Inventory is full
            Debug.Log("full");
            return false;
        }
    }
}
