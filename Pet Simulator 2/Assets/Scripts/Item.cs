using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps; 

// Base class for all items
[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int itemPrice;
    public string ItemDescription;
    public int itemQuantity;
    
    [Header("Building")]
    public bool isBuildMaterial = false;
    public TileBase tileToPlace; // The tile this item will place when building
    
    //this is the object that will be spawned in in the game world
    public GameObject itemPrefab;
    
    public virtual void Use()
    {

    }
}