using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyButton : MonoBehaviour
{
    [SerializeField] private Item itemToAdd;
    [SerializeField] private InventoryManager inventory; 
    private Button button;
     [SerializeField] private TMP_Text itemPriceText;
    
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        inventory = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        button.onClick.AddListener(AddItemToInventory);
        itemPriceText.text = "" + itemToAdd.itemPrice;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItemToInventory()
    {
       inventory.AddItem(itemToAdd);
    }
}
