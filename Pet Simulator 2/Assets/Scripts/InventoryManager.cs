using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // Singleton pattern for easy access
    public static InventoryManager Instance;

    // Maximum inventory slots
    public int maxInventorySlots = 5;

    // Reference to the inventory UI parent
    public GameObject inventoryUI;

    // Array to store inventory slot data
    [System.Serializable]
    public class InventorySlot
    {
        public int itemID;
        public int quantity;
        public GameObject slotUI;
        public TextMeshProUGUI quantityText;
        public Image itemImage;
        public bool isEmpty;

        public InventorySlot()
        {
            isEmpty = true;
            itemID = -1;
            quantity = 0;
        }
    }

    // Array of inventory slots
    public InventorySlot[] inventorySlots;

    // Reference to ShopManager to get item data
    public ShopManagerScript shopManager;

    // Dictionary of item sprites (you'll need to populate this with your item sprites)
    public List<Sprite> itemSprites = new List<Sprite>();

    // NEW: Prefab references for item images, matches the IDs in ShopManagerScript
    public Sprite[] itemSpritePrefabs;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Find ShopManager if not assigned
        if (shopManager == null)
        {
            shopManager = FindObjectOfType<ShopManagerScript>();
            if (shopManager == null)
            {
                Debug.LogError("No ShopManagerScript found in the scene!");
            }
        }

        // NEW: Initialize item sprites if needed
        if (itemSpritePrefabs != null && itemSpritePrefabs.Length > 0)
        {
            // Clear any existing sprites
            itemSprites.Clear();

            // Add a null sprite at index 0 since our item IDs start at 1
            itemSprites.Add(null);

            // Add all sprites from the prefabs array
            for (int i = 0; i < itemSpritePrefabs.Length; i++)
            {
                itemSprites.Add(itemSpritePrefabs[i]);
            }

            Debug.Log("Initialized " + itemSprites.Count + " item sprites");
        }
        else
        {
            Debug.LogWarning("No item sprite prefabs assigned in Inspector!");
        }

        // Initialize inventory slots
        InitializeInventory();
    }

    // Initialize the inventory slots based on UI
    void InitializeInventory()
    {
        if (inventoryUI == null)
        {
            Debug.LogError("Inventory UI parent is not assigned!");
            return;
        }

        // Count how many child slots we have in the UI
        int slotCount = inventoryUI.transform.childCount;
        maxInventorySlots = Mathf.Min(maxInventorySlots, slotCount);

        // Initialize the inventory slots array
        inventorySlots = new InventorySlot[maxInventorySlots];

        // Set up each inventory slot
        for (int i = 0; i < maxInventorySlots; i++)
        {
            inventorySlots[i] = new InventorySlot();

            // Get the UI slot from the parent
            GameObject slotUI = inventoryUI.transform.GetChild(i).gameObject;
            inventorySlots[i].slotUI = slotUI;

            // Try to find the quantity text in the slot
            inventorySlots[i].quantityText = slotUI.GetComponentInChildren<TextMeshProUGUI>();

            // Try to find the item image in the slot
            // NEW: Look for the Image component in children (not just direct child)
            inventorySlots[i].itemImage = slotUI.transform.GetComponentInChildren<Image>();

            // NEW: If we found multiple images, make sure we get the one meant for items
            if (inventorySlots[i].itemImage != null)
            {
                // If there are multiple images, try to find one named "ItemImage" or similar
                Image[] images = slotUI.GetComponentsInChildren<Image>();
                if (images.Length > 1)
                {
                    foreach (Image img in images)
                    {
                        // Skip the first/background image of the slot
                        if (img.transform == slotUI.transform)
                            continue;

                        // Use this image instead
                        inventorySlots[i].itemImage = img;
                        break;
                    }
                }
            }

            // Clear the slot initially
            ClearSlot(i);
        }

        Debug.Log("Inventory initialized with " + maxInventorySlots + " slots");
    }

    // Add an item to the inventory
    public bool AddItem(int itemID, int quantity = 1)
    {
        Debug.Log("Attempting to add item: " + itemID + " x" + quantity);

        // First check if the item already exists in inventory
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (!inventorySlots[i].isEmpty && inventorySlots[i].itemID == itemID)
            {
                // Item exists, increase quantity
                inventorySlots[i].quantity += quantity;
                UpdateSlotUI(i);
                Debug.Log("Added to existing item stack. New quantity: " + inventorySlots[i].quantity);
                return true;
            }
        }

        // Item doesn't exist, find an empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].isEmpty)
            {
                // Found empty slot, add item here
                inventorySlots[i].isEmpty = false;
                inventorySlots[i].itemID = itemID;
                inventorySlots[i].quantity = quantity;
                UpdateSlotUI(i);
                Debug.Log("Added item to empty slot " + i + " with ID: " + itemID);
                return true;
            }
        }

        // No empty slots found
        Debug.Log("Inventory is full, couldn't add item");
        return false;
    }

    // Remove an item from the inventory
    public bool RemoveItem(int itemID, int quantity = 1)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (!inventorySlots[i].isEmpty && inventorySlots[i].itemID == itemID)
            {
                // Found the item, reduce quantity
                inventorySlots[i].quantity -= quantity;

                // If quantity reaches 0, clear the slot
                if (inventorySlots[i].quantity <= 0)
                {
                    ClearSlot(i);
                }
                else
                {
                    UpdateSlotUI(i);
                }

                return true;
            }
        }

        // Item not found in inventory
        return false;
    }

    // Clear a slot
    private void ClearSlot(int slotIndex)
    {
        inventorySlots[slotIndex].isEmpty = true;
        inventorySlots[slotIndex].itemID = -1;
        inventorySlots[slotIndex].quantity = 0;

        // Update UI
        UpdateSlotUI(slotIndex);
    }

    // Update the UI of a slot
    private void UpdateSlotUI(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Length)
            return;

        InventorySlot slot = inventorySlots[slotIndex];

        // Update quantity text if available
        if (slot.quantityText != null)
        {
            slot.quantityText.text = slot.isEmpty ? "" : slot.quantity.ToString();
        }

        // Update item image if available
        if (slot.itemImage != null)
        {
            if (slot.isEmpty)
            {
                // Make the image transparent or use a placeholder
                Color imageColor = slot.itemImage.color;
                imageColor.a = 0f; // Set alpha to 0 (transparent)
                slot.itemImage.color = imageColor;

                // NEW: Debug info
                Debug.Log("Cleared slot " + slotIndex + " image (made transparent)");
            }
            else
            {
                // NEW: Better debug info and error checking
                Debug.Log("Setting image for item ID: " + slot.itemID + " in slot " + slotIndex);

                // Try to get the correct sprite for this item
                if (itemSprites.Count > slot.itemID && slot.itemID >= 0)
                {
                    Sprite itemSprite = itemSprites[slot.itemID];

                    if (itemSprite != null)
                    {
                        slot.itemImage.sprite = itemSprite;
                        Debug.Log("Successfully set sprite for item " + slot.itemID);
                    }
                    else
                    {
                        Debug.LogWarning("No sprite found for item ID " + slot.itemID);
                    }
                }
                else
                {
                    Debug.LogWarning("Item ID " + slot.itemID + " is out of range for itemSprites list (size: " + itemSprites.Count + ")");
                }

                // Make sure the image is visible
                Color imageColor = slot.itemImage.color;
                imageColor.a = 1f; // Set alpha to 1 (fully visible)
                slot.itemImage.color = imageColor;
            }
        }
        else
        {
            Debug.LogWarning("No image component found for slot " + slotIndex);
        }
    }
}