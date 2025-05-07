
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

    private void Start()
    {
        // Only freeze time if we're in scene 0 (the name input scene)
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Time.timeScale = 0f;
        }

        // Check if we already have a name from PetNameManager
        if (PetNameManager.Instance != null && !string.IsNullOrEmpty(PetNameManager.Instance.NameTag))
        {
            petName = PetNameManager.Instance.NameTag;

            // If we're in scene 0 and already have a name, we might want to show it in the input field
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                nameInputField.text = petName;
            }
            else
            {
                // If we're in any other scene, make sure time is running
                Time.timeScale = 1f;

                // Hide the input panel in non-initial scenes
                if (nameInputPanel != null)
                {
                    nameInputPanel.SetActive(false);
                }
            }
        }

        // Listener for setName Button to call name setting function
        setNameButton.onClick.AddListener(OnSetNameButtonClicked);
    }

    // This method is called when the button is clicked
    private void OnSetNameButtonClicked()
    {
        // Get the name from the input field
        petName = nameInputField.text;

        // Check if the name is not empty
        if (!string.IsNullOrEmpty(petName))
        {
            // Update the pet's name using the PetController
            petController.SetPetName(petName);
            Time.timeScale = 1f;

            // Hide the Input Field and Button
            if (nameInputPanel != null)
            {
                nameInputPanel.SetActive(false); // Hide the entire input panel
            }
            else
            {
                nameInputField.gameObject.SetActive(false);  // Hide the input field
                setNameButton.gameObject.SetActive(false);   // Hide the button
            }
        }
        else
        {
            Debug.Log("Name cannot be empty");
        }
    }
}