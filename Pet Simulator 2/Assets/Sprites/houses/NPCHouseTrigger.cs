using UnityEngine;
using Cinemachine;

public class NPCHouseTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private bool isEntering = true; // True for entering house, false for exiting
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private GameObject promptText; // UI text that shows interaction prompt
    
    [Header("Player and Positions")]
    [SerializeField] private Transform playerTransform; // Reference to player
    [SerializeField] private Transform insideHousePosition; // Where player spawns inside house
    [SerializeField] private Transform outsideHousePosition; // Where player spawns outside house
    
    [Header("Camera Bounds")]
    [SerializeField] private CinemachineConfiner2D confiner; // Optional camera confiner
    [SerializeField] private Collider2D insideBounds; // Camera bounds for inside house
    [SerializeField] private Collider2D outsideBounds; // Camera bounds for outside house
    
    private PetSimulator2 playerInput;
    private bool playerInRange = false;
    private static bool isInsideHouse = false; // Static so it persists across all house triggers
    
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
            if (isEntering && !isInsideHouse)
            {
                EnterHouse();
            }
            else if (!isEntering && isInsideHouse)
            {
                ExitHouse();
            }
        }
    }
    
    void EnterHouse()
    {
        if (insideHousePosition != null && playerTransform != null)
        {
            // Move player inside
            playerTransform.position = insideHousePosition.position;
            isInsideHouse = true;
            
            // Change camera bounds if available
            if (confiner != null && insideBounds != null)
            {
                confiner.m_BoundingShape2D = insideBounds;
            }
            
        }
    }
    
    void ExitHouse()
    {
        if (outsideHousePosition != null && playerTransform != null)
        {
            // Move player outside
            playerTransform.position = outsideHousePosition.position;
            isInsideHouse = false;
            
            // Change camera bounds if available
            if (confiner != null && outsideBounds != null)
            {
                confiner.m_BoundingShape2D = outsideBounds;
            }
        
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
        }
    }
}