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
    [SerializeField] private Image coinImage;

    void Awake()
    {
        UpdateCoinDisplay();
    }

    public void UpdateCoinDisplay()
    {
        coinText.text = coins.ToString();
        coinText2.text = coins.ToString();
    }

    public bool AddItem(Item item)
    {
        // Check if item already exists in inventory
        Item existingItem = items.Find(i => i.itemName == item.itemName);

        if (existingItem != null)
        {
            // Item exists - increase quantity (or set to 1 if it was 0)
            if (existingItem.itemQuantity <= 0)
                existingItem.itemQuantity = 1;
            else
                existingItem.itemQuantity += 1;
            return true;
        }
        else if (items.Count < inventorySize)
        {
            // Item doesn't exist - create a copy and add it
            Item itemCopy = Instantiate(item);
            itemCopy.itemQuantity = 1; // Always set to 1 for new items
            items.Add(itemCopy);
            return true;
        }
        else
        {
            Debug.Log("Inventory full");
            return false;
        }
    }

    public bool HasItem(Item item, int quantity = 1)
    {
        Item existingItem = items.Find(i => i.itemName == item.itemName);
        return existingItem != null && existingItem.itemQuantity >= quantity;
    }

    public bool RemoveItem(Item item, int quantity = 1)
    {
        Item existingItem = items.Find(i => i.itemName == item.itemName);
        
        if (existingItem != null && existingItem.itemQuantity >= quantity)
        {
            existingItem.itemQuantity -= quantity;
            
            // Remove item from list if quantity reaches zero
            if (existingItem.itemQuantity <= 0)
            {
                items.Remove(existingItem);
            }
            
            return true;
        }
        
        Debug.Log("Not enough " + item.itemName + " in inventory!");
        return false;
    }
}