using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildingMechanic : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase selectedTile; // The tile that will be placed
    public Item selectedItem; // The item being used for building
    public TileBase[] floorTiles; // Array of all possible floor tiles that can be built on
    public Button buildButton; // Serialize the build button
    public BuildingInventoryUI buildingInventoryUI; // Reference to the inventory UI
    public InventoryManager inventoryManager; // Reference to inventory manager
    
    [Header("Grid Visual")]
    public Material gridLineMaterial;
    public Color gridColor = Color.white;
    public float gridLineWidth = 0.05f;
    public int gridWidth = 20; // How many cells wide
    public int gridHeight = 20; // How many cells tall
    public int gridOffsetX = 40; // Offset in tiles (left)
    public int gridOffsetY = 40; // Offset in tiles (down)
    
    private PetSimulator2 PlayerInput;
    private Camera mainCamera;
    private bool isBuilding = false;
    private GameObject gridParent;
    
    void Awake()
    {
        PlayerInput = new PetSimulator2();
        mainCamera = Camera.main;
    }
    
    void Start()
    {
        // Add listener to the build button - now opens inventory instead
        buildButton.onClick.AddListener(OpenBuildingInventory);
        
        // Create grid visual
        CreateGridVisual();
        
        // Make sure grid is hidden at start
        gridParent.SetActive(false);
    }
    
    void CreateGridVisual()
    {
        gridParent = new GameObject("GridVisual");
        
        Vector3 cellSize = tilemap.layoutGrid.cellSize;
        Vector3 gridOrigin = tilemap.layoutGrid.transform.position - new Vector3(gridOffsetX * cellSize.x, gridOffsetY * cellSize.y, 0);
        
        // Create vertical lines
        for (int x = 0; x <= gridWidth; x++)
        {
            GameObject lineObj = new GameObject("VerticalLine_" + x);
            lineObj.transform.SetParent(gridParent.transform);
            
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = gridLineMaterial;
            lr.startColor = gridColor;
            lr.endColor = gridColor;
            lr.startWidth = gridLineWidth;
            lr.endWidth = gridLineWidth;
            lr.positionCount = 2;
            lr.useWorldSpace = true;
            lr.sortingOrder = 10;
            
            Vector3 startPos = gridOrigin + new Vector3(x * cellSize.x, 0, 0);
            Vector3 endPos = gridOrigin + new Vector3(x * cellSize.x, gridHeight * cellSize.y, 0);
            
            lr.SetPosition(0, startPos);
            lr.SetPosition(1, endPos);
        }
        
        // Create horizontal lines
        for (int y = 0; y <= gridHeight; y++)
        {
            GameObject lineObj = new GameObject("HorizontalLine_" + y);
            lineObj.transform.SetParent(gridParent.transform);
            
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = gridLineMaterial;
            lr.startColor = gridColor;
            lr.endColor = gridColor;
            lr.startWidth = gridLineWidth;
            lr.endWidth = gridLineWidth;
            lr.positionCount = 2;
            lr.useWorldSpace = true;
            lr.sortingOrder = 10;
            
            Vector3 startPos = gridOrigin + new Vector3(0, y * cellSize.y, 0);
            Vector3 endPos = gridOrigin + new Vector3(gridWidth * cellSize.x, y * cellSize.y, 0);
            
            lr.SetPosition(0, startPos);
            lr.SetPosition(1, endPos);
        }
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
        // Check for exit building input - now works if panel is open OR if building
        if (PlayerInput.Player.ExitBuild.triggered)
        {
            if (isBuilding)
            {
                ExitBuildingMode();
            }
            else if (buildingInventoryUI.buildingInventoryPanel.activeInHierarchy)
            {
                // Close panel even if not building yet
                buildingInventoryUI.HideBuildingInventory();
            }
        }
        
        // Only allow building when in building mode
        if (isBuilding && Input.GetMouseButtonDown(0))
        {
            PlaceTile();
        }
    }
        
    public void OpenBuildingInventory()
    {
        buildingInventoryUI.ShowBuildingInventory();
    }
    
    public void StartBuilding()
    {
        isBuilding = true;
        gridParent.SetActive(true); // Show grid when building starts
    }
    
    public void ExitBuildingMode()
    {
        isBuilding = false;
        selectedTile = null; // Clear selected tile
        selectedItem = null; // Clear selected item
        gridParent.SetActive(false); // Hide grid when building ends
        
        // Always hide the inventory panel when exiting build mode
        if (buildingInventoryUI != null && buildingInventoryUI.buildingInventoryPanel != null)
        {
            buildingInventoryUI.HideBuildingInventory();
        }
    }
    
    void PlaceTile()
    {
        if (selectedItem == null || !inventoryManager.HasItem(selectedItem, 1))
            return;
        
        // Use the SAME calculation as your visual grid
        Vector3 cellSize = tilemap.layoutGrid.cellSize;
        Vector3 gridOrigin = tilemap.layoutGrid.transform.position - new Vector3(gridOffsetX * cellSize.x, gridOffsetY * cellSize.y, 0);
        
        // Get mouse world position
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        
        // Calculate which grid cell this world position is in
        Vector3 relativePos = worldPos - gridOrigin;
        int gridX = Mathf.FloorToInt(relativePos.x / cellSize.x);
        int gridY = Mathf.FloorToInt(relativePos.y / cellSize.y);
        
        // Convert to grid position (accounting for your offset)
        Vector3Int gridPos = new Vector3Int(gridX - gridOffsetX, gridY - gridOffsetY, 0);
        
        // Get the current tile at this position
        TileBase currentTile = tilemap.GetTile(gridPos);
        
        Debug.Log($"Mouse world: {worldPos}, Grid pos: {gridPos}");
        Debug.Log($"Current tile at position: {currentTile}");
        
        // Check if current tile is a valid floor tile
        if (IsFloorTile(currentTile))
        {
            // Place the selected tile
            tilemap.SetTile(gridPos, selectedTile);
            
            // Remove from inventory
            if (inventoryManager.RemoveItem(selectedItem, 1))
            {
                buildingInventoryUI.RefreshInventory();
            }
        }
        else
        {
            Debug.Log("Cannot place here - no valid floor tile found!");
        }
    }

    bool IsFloorTile(TileBase tile)
    {
        Debug.Log($"Checking if tile is floor tile. Current tile: {tile}");
        Debug.Log($"Floor tiles array length: {floorTiles.Length}");
        
        for (int i = 0; i < floorTiles.Length; i++)
        {
            Debug.Log($"Floor tile {i}: {floorTiles[i]}");
            if (tile == floorTiles[i])
            {
                Debug.Log($"Match found with floor tile {i}!");
                return true;
            }
        }
        Debug.Log("No match found in floor tiles array");
        return false;
    }
}