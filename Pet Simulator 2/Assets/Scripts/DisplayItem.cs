using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    private Camera mainCam;

    // Static selection manager
    private static DisplayItem currentlySelected = null;
    private bool isSelected = false;

    void Start()
    {
        SlotCanvas = GetComponent<CanvasGroup>();
        inventory = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        slotbutton.onClick.AddListener(SelectItem);
        mainCam = Camera.main;
    }

    void FixedUpdate()
    {
        DisplayTheItem();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && isSelected)
        {
            PlacingItem();
        }
    }

    public void DisplayTheItem()
    {
        if (inventory.items.Count > 0 && index < inventory.items.Count)
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

    void SelectItem()
    {
        if (item != null)
        {
            // Deselect the previously selected item
            if (currentlySelected != null && currentlySelected != this)
            {
                currentlySelected.DeselectItem();
            }
            
            // Select this item
            isSelected = true;
            currentlySelected = this;
            Debug.Log("itemSelected: " + item.itemName);
        }
    }

    void DeselectItem()
    {
        isSelected = false;
        if (currentlySelected == this)
        {
            currentlySelected = null;
        }
        
        Debug.Log("Item deselected");
    }

    public void PlacingItem()
    {
        if (!isSelected)
        {
            return;
        }
        
        // Use the new Input System to get mouse position
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, mainCam.nearClipPlane));
        mouseWorldPos.z = 0;
        
        item = inventory.items[index];
        
        Instantiate(item.itemPrfab, mouseWorldPos, Quaternion.identity);
        
        if (inventory.items.Count > 0 && index < inventory.items.Count && item.itemQuantity == 1)
        {
            item.itemQuantity = item.itemQuantity - 1;
            inventory.items.RemoveAt(index);
            itemImage.sprite = null;
            
            SlotCanvas.alpha = 0;
            itemQunatityText.text = "";
            itemNameText.text = "";
        }
        else if (item.itemQuantity > 0)
        {
            item.itemQuantity = item.itemQuantity - 1;
        }
        
        // Deselect after placing
        DeselectItem();
    }
}