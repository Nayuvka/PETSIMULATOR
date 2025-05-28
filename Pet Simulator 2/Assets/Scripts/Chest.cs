using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    public GameObject itemPrefab; //item that chest drops 
    public Sprite openedSprite; 
    // Start is called before the first frame update
    void Start()
    {
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject); //UniqueID  
    }

    

    public bool CanInteract()
    { 
    return !IsOpened;
    }

    public void Interact()
    {
        if(!CanInteract()) return;
        OpenChest();
    }

    private void OpenChest()
    {
        //SetOpened 
        SetOpened(true);


        //DropItem 
        if (itemPrefab)
        { 
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
            //droppedItem.GetComponet<BounceEffect>().StartBounce(); 
        }
    }

    public void SetOpened(bool opened)
    {
        IsOpened = opened;
        if (IsOpened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;    
        }
    }
}
