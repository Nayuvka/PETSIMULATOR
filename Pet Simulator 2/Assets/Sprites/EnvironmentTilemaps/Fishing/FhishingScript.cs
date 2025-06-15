using UnityEngine;
using System.Collections;

public class FishingMechanic : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject bobberPrefab;
    public GameObject hookPrefab;
    
    [Header("Fishing Rod")]
    public Transform fishingRodTip;
    public Animator rodAnimator; // Reference to the rod's animator
    public GameObject fishing;
    
    [Header("Bait")]
    public Item baitItem; // The bait scriptable object required for casting
    
    [Header("Camera")]
    public Camera fishingCamera;
    public Camera mainCamera;
    public Material lineMaterial;
    public float lineWidth = 0.1f;
    public Color lineColor = Color.white;
    
    [Header("Player")]
    public MonoBehaviour petController; // Reference to pet controller component
    
    private GameObject currentBobber;
    private GameObject currentHook;
    private LineRenderer fishingLine;
    private bool isLineCast = false;
    private bool inFishingZone = false;
    private bool fishingModeActive = false;
    private InventoryManager inventoryManager;
    private PetSimulator2 PlayerInput;
    
    public static bool isReeling = false;
    
    void Awake()
    {
        PlayerInput = new PetSimulator2();
    }
    
    void Start()
    {
        SetupLineRenderer();
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        
        // Set fishing game object inactive at start
        fishing.SetActive(false);
    }
    
    void OnEnable()
    {
        PlayerInput.Enable();
    }
    
    void OnDisable()
    {
        PlayerInput.Disable();
    }
    
    void Update()
    {
        // Check if player wants to enter/exit fishing mode
        if (PlayerInput.Player.Fish.triggered)
        {
            if (inFishingZone && !fishingModeActive)
            {
                EnterFishingMode();
            }
            else if (fishingModeActive)  // Exits fishing mode if already active
            {
                ExitFishingMode();
            }
        }
        
        // Fishing controls (only when fishing mode is active)
        if (fishingModeActive && Input.GetMouseButtonDown(0))
        {
            if (!isLineCast)
            {
                // Check if player has bait before casting
                if (inventoryManager.HasItem(baitItem))
                {
                    // Trigger casting animation instead of casting directly
                    rodAnimator.SetTrigger("Cast");
                }
            }
            else
            {
                StartCoroutine(ReelInLine());
                rodAnimator.SetBool("rodCasted", false);
            }
        }
        
        if (isLineCast)
        {
            UpdateFishingLine();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FishingZone"))
        {
            inFishingZone = true;
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("FishingZone"))
        {
            inFishingZone = false;
            if (fishingModeActive)
            {
                ExitFishingMode();
            }
        }
    }
    
    void EnterFishingMode()
    {
        fishingModeActive = true;
        
        // Set fishing game object active
        fishing.SetActive(true);
        
        // Turn off pet controller
        petController.enabled = false;
        
        // Switch camera displays
        mainCamera.targetDisplay = 1;
        fishingCamera.targetDisplay = 0;
    }
    
    void ExitFishingMode()
    {
        fishingModeActive = false;
        
        // Reel in line if cast
        if (isLineCast)
        {
            StartCoroutine(ReelInLine());
        }
        
        // Turn on pet controller
        petController.enabled = true;
        
        // Switch camera displays back
        mainCamera.targetDisplay = 0;
        fishingCamera.targetDisplay = 1;
        
        // Set fishing game object inactive
        fishing.SetActive(false);
    }
    
    // This gets called by Animation Event
    public void CastLine()
    {
        // Remove bait from inventory when casting
        inventoryManager.RemoveItem(baitItem, 1);
        
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldPos = fishingCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;
        
        currentBobber = Instantiate(bobberPrefab, worldPos, Quaternion.identity);
        currentHook = Instantiate(hookPrefab, worldPos, Quaternion.identity);
        
        fishingLine.enabled = true;
        UpdateFishingLine();
        
        isLineCast = true;
    }
    
    void SetupLineRenderer()
    {
        fishingLine = gameObject.AddComponent<LineRenderer>();
        fishingLine.material = lineMaterial;
        fishingLine.startWidth = lineWidth;
        fishingLine.endWidth = lineWidth;
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(lineColor, 0.0f), new GradientColorKey(lineColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        fishingLine.colorGradient = gradient;
        
        fishingLine.positionCount = 2;
        fishingLine.useWorldSpace = true;
        fishingLine.sortingLayerName = "Default";
        fishingLine.sortingOrder = 1;
        fishingLine.enabled = false;
    }
    
    void UpdateFishingLine()
    {
        fishingLine.SetPosition(0, fishingRodTip.position);
        fishingLine.SetPosition(1, currentHook.transform.position);
    }
    
    IEnumerator ReelInLine()
    {
        isReeling = true;
        yield return null;
        
        Destroy(currentBobber);
        Destroy(currentHook);
        
        fishingLine.enabled = false;
        
        isLineCast = false;
        isReeling = false;
    }
}