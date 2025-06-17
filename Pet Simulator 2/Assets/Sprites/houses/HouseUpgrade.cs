using UnityEngine;
using UnityEngine.UI;

public class HouseUpgrade : MonoBehaviour
{
    [Header("Upgrade Settings")]
    [SerializeField] private int upgradeCost = 200;
    
    [Header("House Sprite")]
    [SerializeField] private SpriteRenderer houseRenderer;
    [SerializeField] private Sprite upgradedHouseSprite;
    
    [Header("UI Management")]
    [SerializeField] private GameObject uiObjectToDestroy;
    [SerializeField] private Button upgradeButton;
    
    [Header("Player Inventory Reference")]
    [SerializeField] private InventoryManager playerInventory;
    
    private bool hasBeenUpgraded = false;
    
    void Start()
    {

    }
    
    public void AttemptUpgrade()
    {
        if (hasBeenUpgraded) return;
        
        if (playerInventory.coins >= upgradeCost)
        {
            playerInventory.coins -= upgradeCost;
            playerInventory.UpdateCoinDisplay();
            PerformUpgrade();
        }
    }
    
    private void PerformUpgrade()
    {
        upgradeButton.interactable = false;
        houseRenderer.sprite = upgradedHouseSprite;
        Destroy(uiObjectToDestroy);
        hasBeenUpgraded = true;
    }
    
    public bool CanUpgrade()
    {
        return !hasBeenUpgraded && playerInventory.coins >= upgradeCost;
    }
    
    public int GetUpgradeCost()
    {
        return upgradeCost;
    }
    
    public bool IsUpgraded()
    {
        return hasBeenUpgraded;
    }
}