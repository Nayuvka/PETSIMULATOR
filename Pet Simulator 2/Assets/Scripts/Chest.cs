using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    public GameObject itemPrefab; //item that chest drops 
    public Sprite openedSprite;
    [Header("Item Drop Settings")]
    public Vector3 dropOffset = new Vector3(0, 0.5f, 0); // Customizable drop position offset
    public float dropRadius = 1.5f; // Radius around chest to spawn item

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
        if (!CanInteract()) return;
        OpenChest();
    }

    private void OpenChest()
    {
        //SetOpened 
        SetOpened(true);
        //DropItem 
        if (itemPrefab)
        {
            // Option 1: Simple offset (recommended)
            Vector3 spawnPosition = transform.position + dropOffset;

           

            GameObject droppedItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

            // Add debugging
            BounceEffect bounceEffect = droppedItem.GetComponent<BounceEffect>();
            if (bounceEffect != null)
            {
                Debug.Log("BounceEffect found! Starting bounce...");
                bounceEffect.StartBounce();
            }
            else
            {
                Debug.LogError("BounceEffect component not found on " + droppedItem.name);
            }
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