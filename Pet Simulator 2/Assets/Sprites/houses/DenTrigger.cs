using UnityEngine;

public class DenTrigger : MonoBehaviour
{
    [SerializeField] private ChangeScene changeScene; // Reference to the ChangeScene script
    [SerializeField] private GameObject promptText; // UI text that shows interaction prompt
    [SerializeField] private string playerTag = "Player"; // Tag to identify the player
    [SerializeField] private bool isEntering = true; // True for entering den, false for exiting
    
    private PetSimulator2 playerInput;
    private bool playerInRange = false;
    
    void Awake()
    {
        playerInput = new PetSimulator2();
    }
    
    void Start()
    {
        // Make sure prompt text is hidden at start
        if (promptText != null)
        {
            promptText.SetActive(false);
        }
    }
    
    void OnEnable()
    {
        playerInput.Enable();
    }
    
    void OnDisable()
    {
        playerInput.Disable();
    }
    
    void Update()
    {
        // Check for interact input when player is in range
        if (playerInRange && playerInput.Player.Interact.triggered)
        {
            changeScene.MoveToDen(isEntering);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            
            // Show prompt text
            if (promptText != null)
            {
                promptText.SetActive(true);
            }
            
            Debug.Log($"Player entered {(isEntering ? "den entrance" : "den exit")} trigger area - press interact to {(isEntering ? "enter" : "exit")} den");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            
            // Hide prompt text
            if (promptText != null)
            {
                promptText.SetActive(false);
            }
            
            Debug.Log($"Player left {(isEntering ? "den entrance" : "den exit")} trigger area");
        }
    }
}