using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public int slotIndex;
    public Image itemImage;
    public TextMeshProUGUI quantityText;

    // Reference to the inventory manager
    private InventoryManager inventoryManager;

    void Start()
    {
        // Find references if not set
        if (itemImage == null)
        {
            // First try to find a child named "ItemImage" (best practice)
            Transform itemImageTransform = transform.Find("ItemImage");
            if (itemImageTransform != null)
            {
                itemImage = itemImageTransform.GetComponent<Image>();
            }

            // If that fails, get first Image that's not this object's Image
            if (itemImage == null)
            {
                Image[] images = GetComponentsInChildren<Image>();
                foreach (Image img in images)
                {
                    // Skip the slot's background image (usually on the slot itself)
                    if (img.transform != transform)
                    {
                        itemImage = img;
                        break;
                    }
                }
            }

            // Last resort, just get any Image
            if (itemImage == null)
            {
                itemImage = GetComponentInChildren<Image>();
            }
        }

        if (quantityText == null)
        {
            // Try to find a child named "QuantityText" (best practice)
            Transform quantityTransform = transform.Find("QuantityText");
            if (quantityTransform != null)
            {
                quantityText = quantityTransform.GetComponent<TextMeshProUGUI>();
            }

            // If that fails, get any TextMeshProUGUI
            if (quantityText == null)
            {
                quantityText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        // Find the inventory manager
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("No InventoryManager found in scene!");
        }
        else
        {
            // Register this slot with the inventory manager
            if (slotIndex >= 0 && slotIndex < inventoryManager.inventorySlots.Length)
            {
                // Update the InventoryManager's reference to this slot's UI elements
                inventoryManager.inventorySlots[slotIndex].slotUI = gameObject;
                inventoryManager.inventorySlots[slotIndex].itemImage = itemImage;
                inventoryManager.inventorySlots[slotIndex].quantityText = quantityText;

                // Refresh the UI for this slot
                inventoryManager.inventorySlots[slotIndex].isEmpty = true; // Ensure it's properly initialized

                Debug.Log("InventorySlotUI initialized for slot " + slotIndex);
            }
            else
            {
                Debug.LogWarning("Slot index " + slotIndex + " is out of range for inventory!");
            }
        }
    }

    // Method to use an item when clicked
    public void OnSlotClicked()
    {
        if (inventoryManager == null)
            return;

        // Get the slot data
        if (slotIndex >= 0 && slotIndex < inventoryManager.inventorySlots.Length)
        {
            var slot = inventoryManager.inventorySlots[slotIndex];
            if (!slot.isEmpty)
            {
                Debug.Log("Clicked on item: " + slot.itemID + " (Quantity: " + slot.quantity + ")");
                // Here you can implement item usage logic

                // Example: Use the item (decrease quantity by 1)
                // inventoryManager.RemoveItem(slot.itemID, 1);
            }
        }
    }
}