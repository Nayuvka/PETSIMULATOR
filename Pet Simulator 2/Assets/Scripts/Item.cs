using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Base class for all items
[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int itemPrice;
    public string ItemDescription;
    public int itemQuantity;
    public virtual void Use()
    {

    }
}
