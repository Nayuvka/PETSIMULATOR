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
    private Camera mainCam;

    // Static selection manager
    private static GameObject draggedItem = null;
    private static DisplayItem currentlySelected = null;
    private static bool isDragging = false;
    private static float draggedItemZ = 0f;

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
        // Handle dragging
        if (isDragging && draggedItem != null)
        {
            // Make the item follow the mouse (using the same logic as Poop script)
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = Mathf.Abs(mainCam.transform.position.z - draggedItemZ);
            
            Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mouseScreenPos);
            draggedItem.transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, draggedItemZ);
            
            // Place the item on left click
            if (Input.GetMouseButtonDown(0))
            {
                PlaceItem();
            }
            
            // Cancel placement on right click
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
    }

    public void DisplayTheItem()
    {
        if (inventory.items.Count > index)
        {
            item = inventory.items[index];
            
            // Update UI elements
            itemImage.sprite = item.icon;
            SlotCanvas.alpha = 1;
            itemQunatityText.text = item.itemQuantity.ToString();
            itemNameText.text = item.itemName;
        }
        else
        {
            // Clear the slot when there's no item at this index
            item = null;
            itemImage.sprite = null;
            SlotCanvas.alpha = 0;
            itemQunatityText.text = "";
            itemNameText.text = "";
        }
    }

    void SelectItem()
    {
        if (item != null && !isDragging)
        {
            // Cancel any existing placement
            if (draggedItem != null)
            {
                CancelPlacement();
            }
            
            // Start dragging
            currentlySelected = this;
            isDragging = true;
            
            // Instantiate the item at mouse position (using proper z-depth calculation)
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = 10f; // Default z distance from camera
            
            Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mouseScreenPos);
            
            draggedItem = Instantiate(item.itemPrefab, mouseWorldPos, Quaternion.identity);
            
            // Store the z position of the dragged item
            draggedItemZ = draggedItem.transform.position.z;
        }
    }

    void PlaceItem()
    {
        if (currentlySelected != this || draggedItem == null) return;
        
        // Update the quantity/remove the item BEFORE the display update
        if (currentlySelected.item.itemQuantity <= 1)
        {
            currentlySelected.inventory.items.RemoveAt(currentlySelected.index);
        }
        else
        {
            currentlySelected.item.itemQuantity--;
        }
        
        // Force all display items to update immediately
        DisplayItem[] allDisplayItems = FindObjectsOfType<DisplayItem>();
        foreach (var displayItem in allDisplayItems)
        {
            displayItem.DisplayTheItem();
        }
        
        // Reset dragging state
        isDragging = false;
        draggedItem = null;
        currentlySelected = null;
    }

    void CancelPlacement()
    {
        if (draggedItem != null)
        {
            Destroy(draggedItem);
            draggedItem = null;
        }
        
        isDragging = false;
        currentlySelected = null;
    }
}