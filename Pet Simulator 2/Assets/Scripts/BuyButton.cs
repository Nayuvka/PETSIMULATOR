using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour
{
    public GameObject shopManager;
    private Button buyButton;
    private bool clickHandled = false;

    void Start()
    {
        // Get the Button component
        buyButton = GetComponent<Button>();

        // If shopManager is not assigned, try to find it in the scene
        if (shopManager == null)
        {
            shopManager = GameObject.Find("ShopManager");
            if (shopManager == null)
            {
                Debug.LogError("BuyButton: Cannot find ShopManager in the scene!");
            }
        }

        if (buyButton != null && shopManager != null)
        {
            // Remove any existing listeners to avoid duplicates
            buyButton.onClick.RemoveAllListeners();

            // Add a listener to call the Buy method when clicked
            buyButton.onClick.AddListener(PurchaseSelectedItem);

            Debug.Log("Buy button listener set up successfully");
        }
        else
        {
            Debug.LogError("BuyButton: Missing Button component or ShopManager reference!");
        }
    }

    void PurchaseSelectedItem()
    {
        // Prevent multiple calls in the same frame
        if (clickHandled) return;
        clickHandled = true;

        Debug.Log("Buy button clicked ONCE, attempting to purchase item");

        if (shopManager != null)
        {
            // Call the Buy method on the ShopManager
            shopManager.GetComponent<ShopManagerScript>().Buy();
        }
        else
        {
            Debug.LogError("ShopManager reference is null!");
        }

        // Reset after a short delay
        StartCoroutine(ResetClickHandled());
    }

    private IEnumerator ResetClickHandled()
    {
        yield return new WaitForSeconds(0.2f);
        clickHandled = false;
    }
}