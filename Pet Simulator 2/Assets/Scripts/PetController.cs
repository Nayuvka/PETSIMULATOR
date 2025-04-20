using UnityEngine;
using TMPro;

public class PetController : MonoBehaviour
{
    public Animator petAnimator;
    private Vector3 destination;
    public float speed;
    public TextMeshProUGUI nameTag;           // The UI text component
    public RectTransform nameTagRect;         // The RectTransform of the UI text
    public Canvas canvas;                     // The canvas the nameTag is on
    private string petName = "My Pet";

    // Add these new variables
    public AudioSource audioSource;           // Audio source component
    public AudioClip pettingSound;            // Sound to play when pet is petted

    private void Start()
    {
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
    }

    private void Update()
    {
        // Move the pet
        if (Vector3.Distance(transform.position, destination) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        }
        UpdateNameTagPosition();

        // Check for right-click input
        if (Input.GetMouseButtonDown(1)) // 0 is left click, 1 is right click
        {
            HandleRightClick();
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
            // Play the petting animation and sound
            Petting();
        }
    }

    private void UpdateNameTagPosition()
    {
        if (nameTag == null || nameTagRect == null || canvas == null) return;
        // Convert world position above the pet to screen point
        Vector3 worldPosition = transform.position + Vector3.up * 2f;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
        // Convert screen point to UI canvas local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint
        );
        nameTagRect.anchoredPosition = localPoint;
    }

    public void Move(Vector3 destination)
    {
        destination.y = transform.position.y;
        destination.z = transform.position.z;
        this.destination = destination;
    }

    public void SetPetName(string newName)
    {
        petName = newName;
        if (nameTag != null)
        {
            nameTag.text = petName;
        }
    }

    // Add the new Petting method
    public void Petting()
    {
        petAnimator.SetTrigger("Petting");
        PlaySound(pettingSound);
    }

    // Keep your original methods
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