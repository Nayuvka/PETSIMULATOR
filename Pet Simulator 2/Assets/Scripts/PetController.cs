using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PetController : MonoBehaviour
{
    public Animator petAnimator;
    public float speed = 5f;                  // Speed of movement
    public TextMeshProUGUI nameTag;           // The UI text component
    public RectTransform nameTagRect;         // The RectTransform of the UI text
    public Canvas canvas;                     // The canvas the nameTag is on
    private string petName = "My Pet";        // Default pet name

    // Movement variables
    private Vector2 moveInput;
    private bool isMoving = false;
    private Rigidbody2D rb;                   // Add this for physics-based movement

    // Audio variables
    public AudioSource audioSource;           // Audio source component
    public AudioClip pettingSound;            // Sound to play when pet is petted
    public AudioClip eatingSound;

    // Added to store pet name locally instead of using PetNameManager
    private static string savedPetName = "My Pet";

    private void Awake()
    {
        // Check if we have a saved pet name from previous runs
        if (PlayerPrefs.HasKey("PetName"))
        {
            savedPetName = PlayerPrefs.GetString("PetName");
        }
    }

    private void Start()
    {
        // Get or add Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // Disable gravity
            rb.freezeRotation = true; // Keep the pet from rotating
        }

        // If no AudioSource is assigned, try to get it from the GameObject
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            // If it still doesn't exist, add one
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Add debug statements here
        Debug.Log("PetController Start");

        // Use the saved pet name instead of PetNameManager
        petName = savedPetName;

        if (nameTag != null)
        {
            Debug.Log("Setting name tag to: " + petName);
            nameTag.text = petName;
        }
        else
        {
            Debug.LogWarning("nameTag reference is null! Make sure it's assigned in the Inspector");
        }
    }

    public void TransitionToGameScene()
    {
        // Ensure time scale is restored before changing scenes
        Time.timeScale = 1f;

        // Save the pet name before transitioning
        PlayerPrefs.SetString("PetName", petName);
        PlayerPrefs.Save();

        // Load the game scene (scene 1)
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    private void Update()
    {
        // Get input for WASD movement
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        // Normalize input to prevent faster diagonal movement
        if (moveInput.magnitude > 1f)
        {
            moveInput.Normalize();
        }

        // Set animation parameters if you have them
        isMoving = moveInput.magnitude > 0.1f;

        // Check for right-click input for petting
        if (Input.GetMouseButtonDown(1)) // 0 is left click, 1 is right click
        {
            HandleRightClick();
        }

        // Update the name tag position
        UpdateNameTagPosition();
    }

    private void FixedUpdate()
    {
        // Apply movement in FixedUpdate for physics-based movement
        rb.velocity = moveInput * speed;

        // Face the direction of movement using sprite renderer instead of scale
        if (moveInput.x != 0)
        {
            // Get the SpriteRenderer component
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // Flip the sprite using flipX instead of scale
                spriteRenderer.flipX = moveInput.x < 0;
            }
        }
    }

    private void HandleRightClick()
    {
        // Convert mouse position to world position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z; // Match the z-coordinate

        // Check if the click is on the pet (using collider)
        Collider2D hitCollider = Physics2D.OverlapPoint(mousePos);
        if (hitCollider != null && hitCollider.gameObject == gameObject)
        {
            Petting();
        }
    }

    private void UpdateNameTagPosition()
    {
        if (nameTag == null || nameTagRect == null || canvas == null) return;

        // Convert world position above the pet to screen point
        Vector3 worldPosition = transform.position + Vector3.up * 2f;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint
        );

        // Update only the position, not rotation
        nameTagRect.anchoredPosition = localPoint;

        // Ensure the name tag is always displayed correctly (not flipped)
        nameTagRect.localScale = new Vector3(1, 1, 1);
        nameTagRect.localRotation = Quaternion.identity;
    }

    public void SetPetName(string newName)
    {
        petName = newName;
        savedPetName = newName; // Update the static value too

        if (nameTag != null)
        {
            nameTag.text = newName;
            Debug.Log("Setting pet name to: " + newName);
        }
        else
        {
            Debug.LogWarning("nameTag is null when trying to set name to: " + newName);
        }

        // Save the pet name using PlayerPrefs
        PlayerPrefs.SetString("PetName", newName);
        PlayerPrefs.Save();
    }

    // Petting method
    public void Petting()
    {
        petAnimator.SetTrigger("Petting");
        PlaySound(pettingSound);
    }

    // Animation methods
    public void Happy() => petAnimator.SetTrigger("Happy");
    public void Sad() => petAnimator.SetTrigger("Sad");
    public void Hungry() => petAnimator.SetTrigger("Hungry");
    public void Eat() => petAnimator.SetTrigger("Eat");

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}