using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ShopManagerScript : MonoBehaviour
{
    public int[,] shopItems = new int[5, 5]; // Number of items in script 
    public float coins = 100; // Starting with some coins for testing
    public TextMeshProUGUI CoinsTXT;

    // Reference to the inventory manager
    private InventoryManager inventoryManager;

    // Reference to currently selected item
    private int selectedItemID = -1;
    // Add a flag to prevent double-processing
    private bool isProcessingPurchase = false;

    void Start()
    {
        // Find the inventory manager
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogWarning("No InventoryManager found in scene. Items won't be added to inventory when purchased.");
        }

        // Make sure CoinsTXT is assigned
        if (CoinsTXT == null)
        {
            Debug.LogError("CoinsTXT is not assigned in ShopManagerScript!");
            CoinsTXT = GameObject.Find("CoinTxt")?.GetComponent<TextMeshProUGUI>();
        }

        if (CoinsTXT != null)
        {
            CoinsTXT.text = "Coins: " + coins.ToString();
        }

        // ID'S
        shopItems[1, 1] = 1;
        shopItems[1, 2] = 2;
        shopItems[1, 3] = 3;
        shopItems[1, 4] = 4;

        // Price 
        shopItems[2, 1] = 10;
        shopItems[2, 2] = 20;
        shopItems[2, 3] = 30;
        shopItems[2, 4] = 40;

        // Quantity 
        shopItems[3, 1] = 0;
        shopItems[3, 2] = 0;
        shopItems[3, 3] = 0;
        shopItems[3, 4] = 0;
    }

    // Method to select an item
    public void SelectItem(int itemID)
    {
        selectedItemID = itemID;
        Debug.Log("Shop Manager: Selected item ID set to " + itemID);
    }

    // Modified Buy method that uses the selected item
    public void Buy()
    {
        // Use a lock to prevent the function from being called multiple times in the same frame
        if (isProcessingPurchase)
        {
            Debug.Log("Already processing a purchase, ignoring additional calls");
            return;
        }

        isProcessingPurchase = true;

        // Check if an item is selected
        if (selectedItemID <= 0)
        {
            Debug.Log("No item selected!");
            isProcessingPurchase = false;
            return;
        }

        Debug.Log("Trying to buy item: " + selectedItemID + ", Price: " + shopItems[2, selectedItemID] + ", Coins: " + coins);

        // Check for enough coins to buy the selected item
        if (coins >= shopItems[2, selectedItemID])
        {
            // Subtract price
            coins -= shopItems[2, selectedItemID];

            // Increase quantity by exactly 1
            shopItems[3, selectedItemID] += 1;

            // Update coins display
            if (CoinsTXT != null)
            {
                CoinsTXT.text = "Coins: " + coins.ToString();
            }

            Debug.Log("Purchase successful! New coin amount: " + coins + ", New quantity: " + shopItems[3, selectedItemID]);

            // Update the quantity display of the item
            UpdateItemQuantityDisplay(selectedItemID);

            // Add the item to inventory if inventory manager exists
            if (inventoryManager != null)
            {
                bool added = inventoryManager.AddItem(selectedItemID);
                if (added)
                {
                    Debug.Log("Item added to inventory successfully");
                }
                else
                {
                    Debug.LogWarning("Failed to add item to inventory - inventory might be full");
                }
            }
        }
        else
        {
            Debug.Log("Not enough coins to purchase item: " + selectedItemID);
        }

        // Release the lock after a short delay to prevent double-clicks
        StartCoroutine(ResetProcessingFlag());
    }

    private IEnumerator ResetProcessingFlag()
    {
        // Wait for a short time to prevent double-clicks
        yield return new WaitForSeconds(0.2f);
        isProcessingPurchase = false;
    }

    // Helper method to update quantity display for a specific item
    private void UpdateItemQuantityDisplay(int itemID)
    {
        // Find the item's ButtonInfo component to update its quantity text
        ButtonInfo[] allItems = FindObjectsOfType<ButtonInfo>();
        foreach (ButtonInfo item in allItems)
        {
            if (item.ItemID == itemID)
            {
                item.UpdateQuantityText();
                break;
            }
        }
    }
}