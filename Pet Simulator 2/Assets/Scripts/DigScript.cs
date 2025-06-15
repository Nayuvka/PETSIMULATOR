using UnityEngine;
using UnityEngine.Tilemaps;

public class DiggingMechanic : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase dugTile; // The hole tile
    public TileBase[] grassTiles = new TileBase[9]; // Array of 9 grass tiles
    public GameObject dustPrefab; // Dust effect prefab
    
    [Header("Coin System")]
    public float coinChance = 0.3f; // 30% chance to get coins
    public int minCoins = 1;
    public int maxCoins = 10;
    
    private PetSimulator2 PlayerInput;
    private InventoryManager inventoryManager;
    
    void Awake()
    {
        PlayerInput = new PetSimulator2();
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
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
        if (PlayerInput.Player.Dig.triggered)
        {
            DigCurrentTile();
        }
    }
    
    void DigCurrentTile()
    {
        // Get player's current position
        Vector3 playerPos = transform.position;
        
        // Convert to grid position
        Vector3Int gridPos = tilemap.layoutGrid.WorldToCell(playerPos);
        
        // Get the current tile at this position
        TileBase currentTile = tilemap.GetTile(gridPos);
        
        Vector3 tileCenter = tilemap.layoutGrid.CellToWorld(gridPos) + tilemap.layoutGrid.cellSize / 2;
        
        // Instantiate dust effect and play particle system
        GameObject dustEffect = Instantiate(dustPrefab, tileCenter, Quaternion.identity);
        ParticleSystem particles = dustEffect.GetComponent<ParticleSystem>();
        particles.Play();
        
        // Destroy the dust effect after the particle system finishes
        Destroy(dustEffect, particles.main.duration + particles.main.startLifetime.constantMax);
        
        if (currentTile == dugTile)
        {
            // Cover up the hole with random grass
            TileBase randomGrass = grassTiles[Random.Range(0, grassTiles.Length)];
            tilemap.SetTile(gridPos, randomGrass);
        }
        else
        {
            // Dig any tile that's not a hole
            tilemap.SetTile(gridPos, dugTile);

            // Check for coin drop when digging
            if (Random.Range(0f, 1f) <= coinChance)
            {
                int coinsFound = Random.Range(minCoins, maxCoins + 1);
                inventoryManager.coins += coinsFound;
            }
        }
    }
    
    bool IsGrassTile(TileBase tile)
    {
        // Check if the current tile is one of our grass tiles
        foreach (TileBase grassTile in grassTiles)
        {
            if (tile == grassTile)
                return true;
        }
        return false;
    }
}