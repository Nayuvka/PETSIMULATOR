using UnityEngine;
using UnityEngine.UI; // For working with UI elements like Button and InputField
using TMPro; // For TextMeshPro support
using UnityEngine.SceneManagement; // For accessing scene information

public class NameInputHandler : MonoBehaviour
{
    public PetController petController;  // Reference to the PetController (assign this in the Inspector)
    public InputField nameInputField;    // Reference to the InputField where the player types the name
    public Button setNameButton;         // Reference to the Button that submits the name
    public GameObject nameInputPanel;    // Reference to the entire panel that holds the InputField and Button (optional, for hiding the entire UI)
    private string petName;
    private int currentTag;

    private void Start()
    {
        // Wait for SaveManager to be initialized, or set default value
        if (SaveManager.instance != null)
        {
            currentTag = SaveManager.instance.currentTag;
            Debug.Log("Current tag loaded: " + currentTag);
        }
        else
        {
            Debug.LogWarning("SaveManager instance not found, using default currentTag value of 0");
            currentTag = 0;
        }

        // Check if we already have a name from BOTH PetNameManager AND SaveManager
        bool hasExistingName = false;

        // First check SaveManager (more reliable for persistence)
        if (SaveManager.instance != null && !string.IsNullOrEmpty(SaveManager.instance.petName))
        {
            petName = SaveManager.instance.petName;
            hasExistingName = true;
            Debug.Log("Existing name found in SaveManager: " + petName);

            // Also update PetNameManager to keep them in sync
            if (PetNameManager.Instance != null)
            {
                PetNameManager.Instance.NameTag = petName;
            }
        }
        // Then check PetNameManager as backup
        else if (PetNameManager.Instance != null && !string.IsNullOrEmpty(PetNameManager.Instance.NameTag))
        {
            petName = PetNameManager.Instance.NameTag;
            hasExistingName = true;
            Debug.Log("Existing name found in PetNameManager: " + petName);

            // Update SaveManager to keep them in sync
            if (SaveManager.instance != null)
            {
                SaveManager.instance.petName = petName;
            }
        }

        // Set the pet's name immediately if we have one
        if (hasExistingName && petController != null)
        {
            petController.SetPetName(petName);
            Debug.Log("Pet name set to existing name: " + petName);
        }

        // Only show name input if we're in scene 1 AND don't have an existing name
        if (SceneManager.GetActiveScene().buildIndex == 1 && !hasExistingName)
        {
            // Freeze time and show name input
            Time.timeScale = 0f;
            if (nameInputPanel != null)
            {
                nameInputPanel.SetActive(true);
            }
            Debug.Log("Showing name input - no existing name found");
        }
        else
        {
            // We either have a name already, or we're not in the name input scene
            Time.timeScale = 1f;

            // Hide the name input panel
            if (nameInputPanel != null)
            {
                nameInputPanel.SetActive(false);
                Debug.Log("Hiding name input panel - name already exists or not in scene 1");
            }
        }

        // Check if button reference exists before adding listener
        if (setNameButton != null)
        {
            setNameButton.onClick.AddListener(OnSetNameButtonClicked);
            Debug.Log("Button listener added successfully");
        }
        else
        {
            Debug.LogError("Set Name Button is not assigned in the Inspector!");
        }
    }

    // Public method to save the current tag and pet name
    public void SaveCurrentTag()
    {
        if (SaveManager.instance != null)
        {
            SaveManager.instance.currentTag = currentTag;
            SaveManager.instance.petName = petName; // Save the pet name too
            SaveManager.instance.Save();
            Debug.Log("Current tag and pet name saved: " + currentTag + ", " + petName);
        }
        else
        {
            Debug.LogError("Cannot save - SaveManager instance is null!");
        }
    }

    // This method is called when the button is clicked
    private void OnSetNameButtonClicked()
    {
        Debug.Log("Set Name Button clicked!");

        // Get the name from the input field
        if (nameInputField != null)
        {
            petName = nameInputField.text;
            Debug.Log("Pet name from input field: " + petName);
        }
        else
        {
            Debug.LogError("Name Input Field is not assigned!");
            return;
        }

        // Check if the name is not empty
        if (!string.IsNullOrEmpty(petName))
        {
            // Check if petController exists before using it
            if (petController != null)
            {
                // Update the pet's name using the PetController
                petController.SetPetName(petName);
                Debug.Log("Pet name set to: " + petName);
            }
            else
            {
                Debug.LogError("Pet Controller is not assigned in the Inspector!");
            }

            // Update PetNameManager if it exists
            if (PetNameManager.Instance != null)
            {
                PetNameManager.Instance.NameTag = petName;
                Debug.Log("PetNameManager updated with name: " + petName);
            }

            // Update SaveManager with the pet name
            if (SaveManager.instance != null)
            {
                SaveManager.instance.petName = petName;
                Debug.Log("SaveManager updated with pet name: " + petName);
            }

            Time.timeScale = 1f;

            // Save the current tag and pet name
            SaveCurrentTag();

            // Hide the Input Field and Button
            if (nameInputPanel != null)
            {
                nameInputPanel.SetActive(false); // Hide the entire input panel
                Debug.Log("Name input panel hidden");
            }
            else
            {
                if (nameInputField != null) nameInputField.gameObject.SetActive(false);
                if (setNameButton != null) setNameButton.gameObject.SetActive(false);
                Debug.Log("Individual UI elements hidden");
            }
        }
        else
        {
            Debug.Log("Name cannot be empty");
        }
    }
}