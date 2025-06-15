using UnityEngine;

public class FishScript : MonoBehaviour
{
    [Header("Movement")]
    public float swimSpeed = 2f;
    public float directionChangeInterval = 3f;
    
    [Header("Fish Data")]
    public Item fishItem;
    
    private Vector3 currentDirection;
    private float nextDirectionChange;
    private Bounds movementBounds;
    private SpriteRenderer spriteRenderer;
    private InventoryManager inventoryManager;
    private bool isTouchingHook = false;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        ChooseRandomDirection();
        nextDirectionChange = Time.time + directionChangeInterval;
    }
    
    void Update()
    {
        transform.position += currentDirection * swimSpeed * Time.deltaTime;
        
        CheckBounds();
        
        if (Time.time >= nextDirectionChange)
        {
            ChooseRandomDirection();
            nextDirectionChange = Time.time + Random.Range(directionChangeInterval * 0.5f, directionChangeInterval * 1.5f);
        }
        
        if (currentDirection.x > 0)
            spriteRenderer.flipX = false;
        else if (currentDirection.x < 0)
            spriteRenderer.flipX = true;
        
        // Check if reeling while touching hook
        if (isTouchingHook && FishingMechanic.isReeling)
        {
            CatchFish();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hook"))
        {
            isTouchingHook = true;
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hook"))
        {
            isTouchingHook = false;
        }
    }
    
    void CatchFish()
    {
        inventoryManager.AddItem(fishItem);
        Destroy(gameObject);
    }
    
    void ChooseRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        currentDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0).normalized;
    }
    
    void CheckBounds()
    {
        Vector3 pos = transform.position;
        
        if (pos.x <= movementBounds.min.x || pos.x >= movementBounds.max.x)
        {
            currentDirection.x = -currentDirection.x;
            pos.x = Mathf.Clamp(pos.x, movementBounds.min.x, movementBounds.max.x);
        }
        
        if (pos.y <= movementBounds.min.y || pos.y >= movementBounds.max.y)
        {
            currentDirection.y = -currentDirection.y;
            pos.y = Mathf.Clamp(pos.y, movementBounds.min.y, movementBounds.max.y);
        }
        
        transform.position = pos;
    }
    
    public void SetMovementBounds(Bounds bounds)
    {
        movementBounds = bounds;
    }
}