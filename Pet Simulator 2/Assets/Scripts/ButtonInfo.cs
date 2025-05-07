using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Re-added for Button handling
using TMPro;

public class ButtonInfo : MonoBehaviour
{
    public int ItemID;
    public TextMeshProUGUI PriceTxt;
    public TextMeshProUGUI QuantityTxt;
    public GameObject ShopManager;
    public Button itemSelectButton; // Reference to the item's button component

    void Start()
    {
        // Add a listener to the button to select this item when clicked
        if (itemSelectButton == null)
        {
            // Try to find the button on this GameObject if not assigned
            itemSelectButton = GetComponent<Button>();
        }

        if (itemSelectButton != null)
        {
            itemSelectButton.onClick.AddListener(SelectThisItem);
        }

        // Set initial text values
        UpdatePriceText();
        UpdateQuantityText();
    }

    void Update()
    {
        // We can optionally leave this empty now and only update text when needed
        // Or keep it for real-time updates from other sources
        UpdatePriceText();
    }

    // Method to update the price text
    public void UpdatePriceText()
    {
        if (ShopManager != null && PriceTxt != null)
        {
            PriceTxt.text = "Price: $" + ShopManager.GetComponent<ShopManagerScript>().shopItems[2, ItemID].ToString();
        }
    }

    // Method to update the quantity text
    public void UpdateQuantityText()
    {
        if (ShopManager != null && QuantityTxt != null)
        {
            QuantityTxt.text = ShopManager.GetComponent<ShopManagerScript>().shopItems[3, ItemID].ToString();
        }
    }

    // Method to select this item when clicked
    public void SelectThisItem()
    {
        if (ShopManager != null)
        {
            ShopManager.GetComponent<ShopManagerScript>().SelectItem(ItemID);
        }
    }
}