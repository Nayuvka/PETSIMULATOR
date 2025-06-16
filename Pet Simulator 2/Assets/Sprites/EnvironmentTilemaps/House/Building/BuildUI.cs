using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingInventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject buildingInventoryPanel;
    public Transform buttonParent; // Parent object to spawn buttons under
    public GameObject buttonPrefab; // Prefab with Button, Image, and Text components
    
    [Header("References")]
    public InventoryManager inventoryManager;
    public BuildingMechanic buildingMechanic;
    
    private List<GameObject> spawnedButtons = new List<GameObject>();
    
    public void ShowBuildingInventory()
    {
        buildingInventoryPanel.SetActive(true);
        PopulateInventory();
    }
    
    public void HideBuildingInventory()
    {
        buildingInventoryPanel.SetActive(false);
        ClearInventory();
    }
    
    public void RefreshInventory()
    {
        if (buildingInventoryPanel.activeInHierarchy)
        {
            PopulateInventory();
        }
    }
    
    void PopulateInventory()
    {
        ClearInventory();
        
        // Get all build materials from inventory
        List<Item> buildMaterials = inventoryManager.items.FindAll(item => 
            item.isBuildMaterial && item.itemQuantity > 0);
        
        // Create a button for each build material
        foreach (Item item in buildMaterials)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonParent);
            
            // Get components from the button prefab by name (more reliable)
            Button button = buttonObj.GetComponent<Button>();
            
            // Find specific components by name
            Transform itemImageTransform = buttonObj.transform.Find("ItemImage");
            Transform itemTextTransform = buttonObj.transform.Find("ItemText");
            Transform quantityTextTransform = buttonObj.transform.Find("QuantityText");
            
            Image itemImage = itemImageTransform?.GetComponent<Image>();
            TMP_Text itemText = itemTextTransform?.GetComponent<TMP_Text>();
            TMP_Text quantityText = quantityTextTransform?.GetComponent<TMP_Text>();
            
            // Set up the button
            if (itemImage != null) itemImage.sprite = item.icon;
            if (itemText != null) itemText.text = item.itemName;
            if (quantityText != null) quantityText.text = item.itemQuantity.ToString();
            
            // Add click listener
            if (button != null) button.onClick.AddListener(() => SelectBuildMaterial(item));
            
            spawnedButtons.Add(buttonObj);
        }
    }
    
    void ClearInventory()
    {
        foreach (GameObject button in spawnedButtons)
        {
            if (button != null) Destroy(button);
        }
        spawnedButtons.Clear();
    }
    
    void SelectBuildMaterial(Item item)
    {
        // Set the selected tile in the building mechanic
        buildingMechanic.selectedTile = item.tileToPlace;
        buildingMechanic.selectedItem = item; // Add reference to the item
        
        // Start building but DON'T hide the panel
        buildingMechanic.StartBuilding();
    }
}