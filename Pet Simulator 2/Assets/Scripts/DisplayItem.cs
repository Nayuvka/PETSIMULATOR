using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayItem : MonoBehaviour
{
    [SerializeField] private int index;
    public InventoryManager inventory; 
    public Image itemImage; 
    private CanvasGroup SlotCanvas;
    public Item item;
    [SerializeField] private TMP_Text itemQunatityText;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private Button slotbutton;

    void Start()
    {
        SlotCanvas = GetComponent<CanvasGroup>();
        inventory = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    void FixedUpdate()
    {
        DisplayTheItem();
    }

    public void DisplayTheItem()
    {
        if (inventory.items.Count > 0 && index < inventory.items.Count )
        {
            item = inventory.items[index];
            if (itemImage.sprite == null)
            {
                Debug.Log("its item icon");
            }
            itemImage.sprite = item.icon; 
            SlotCanvas.alpha = 1;
            itemQunatityText.text = "" + item.itemQuantity;
            itemNameText.text = "" + item.itemName;

        }
        else
        {
            
            itemImage.sprite = null; 
            SlotCanvas.alpha = 0;           
        }
    }

    // you need to call this when you place the item on the grid
    public void PlacingItem()
    {
        item = inventory.items[index];

        if (inventory.items.Count > 0 && index < inventory.items.Count && item.itemQuantity == 0) 
        {
            inventory.items.RemoveAt(index);
        }
        else if (item.itemQuantity > 0)
        {
            item.itemQuantity = item.itemQuantity - 1;
        }
    }
}


