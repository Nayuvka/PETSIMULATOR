using UnityEngine;
using UnityEngine.UI; // For working with UI elements like Button and InputField
using TMPro; // For TextMeshPro support

public class NameInputHandler : MonoBehaviour
{
    public PetController petController;  // Reference to the PetController (assign this in the Inspector)
    public InputField nameInputField;    // Reference to the InputField where the player types the name
    public Button setNameButton;         // Reference to the Button that submits the name
    public GameObject nameInputPanel;    // Reference to the entire panel that holds the InputField and Button (optional, for hiding the entire UI)

    private void Start()
    {
        // Make sure the button's OnClick listener is hooked up
        setNameButton.onClick.AddListener(OnSetNameButtonClicked);
    }

    // This method is called when the button is clicked
    private void OnSetNameButtonClicked()
    {
        // Get the name from the input field
        string petName = nameInputField.text;

        // Check if the name is not empty (you could add more validation here)
        if (!string.IsNullOrEmpty(petName))
        {
            // Update the pet's name using the PetController
            petController.SetPetName(petName);

            // Hide the Input Field and Button
            if (nameInputPanel != null)
            {
                nameInputPanel.SetActive(false); // Hide the entire input panel
            }
            else
            {
                // Alternatively, if you don't have a separate panel, you can just hide the input and button individually
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