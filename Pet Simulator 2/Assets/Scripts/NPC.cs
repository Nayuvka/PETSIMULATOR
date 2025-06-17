using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("Dialogue Settings")]
    public NPCDialogue dialogueData;
    private DialogueController dialogueUI;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    private PetSimulator2 playerInput;

    [Header("Shop Settings")]
    [SerializeField] private bool hasShop = false;
    [SerializeField] private GameObject shopMenu; // Reference to shop UI panel
    [SerializeField] private GameObject shopIcon; // Optional icon to show above NPC
    [SerializeField] private string shopOpenSoundName = "shop_open";
    [SerializeField] private string shopCloseSoundName = "shop_close";
    private bool isShopActive = false;

    [Header("Fish Selling")]
    [SerializeField] private bool buysFish = false;
    [SerializeField] private Item fishItem; // Reference to the Fish item ScriptableObject
    [SerializeField] private int coinsPerFish = 25;
    [SerializeField] private InventoryManager playerInventory;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private string dialogueStartSoundName = "dialogue_open";
    [SerializeField] private string dialogueEndSoundName = "dialogue_close";

    // Define vowels and consonants
    private readonly char[] vowels = { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' };
    private readonly char[] consonants = {'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z',
                                         'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z'};

    // Store the current text being typed
    private string currentTypedText = "";

    private void Start()
    {
        dialogueUI = DialogueController.Instance;
        playerInput = new PetSimulator2();
        
        // Set up shop icon visibility
        if (shopIcon != null)
        {
            shopIcon.SetActive(hasShop);
        }
        
        // Make sure shop menu is hidden at start
        if (shopMenu != null)
        {
            shopMenu.SetActive(false);
        }
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public bool CanInteract()
    {
        return !isDialogueActive && !isShopActive;
    }

    public void Interact()
    {
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive && !isShopActive))
            return;

        if (isShopActive)
        {
            CloseShop();
        }
        else if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);
        PauseController.SetPause(true);

        if (!string.IsNullOrEmpty(dialogueStartSoundName))
        {
            SoundEffectManager.Play(dialogueStartSoundName);
        }

        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            // Show the complete line immediately
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        currentTypedText = ""; // Reset the typed text
        dialogueUI.SetDialogueText(""); // Clear the dialogue

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            // Add letter to our current typed text
            currentTypedText += letter;

            // Update the dialogue UI with the current typed text
            dialogueUI.SetDialogueText(currentTypedText);

            // Play phonetic sound for each character
            PlayPhoneticSound(letter);

            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    private void PlayPhoneticSound(char character)
    {
        if (audioSource == null || dialogueData == null) return;

        AudioClip soundToPlay = null;

        // Check if it's a vowel
        if (System.Array.IndexOf(vowels, character) >= 0)
        {
            soundToPlay = GetVowelSound(character);
        }
        // Check if it's a consonant
        else if (System.Array.IndexOf(consonants, character) >= 0)
        {
            soundToPlay = GetConsonantSound(character);
        }
        // Skip spaces and punctuation (no sound)

        // Play the sound
        if (soundToPlay != null)
        {
            audioSource.pitch = dialogueData.voicePitch + Random.Range(-0.05f, 0.05f); // Add slight variation
            audioSource.volume = dialogueData.voiceVolume;
            audioSource.PlayOneShot(soundToPlay);
        }
    }

    private AudioClip GetVowelSound(char vowel)
    {
        if (dialogueData.vowelSounds == null || dialogueData.vowelSounds.Length == 0)
            return null;

        // Map specific vowels to specific sounds based on your array order
        switch (char.ToLower(vowel))
        {
            case 'a': return dialogueData.vowelSounds.Length > 0 ? dialogueData.vowelSounds[0] : null; // VOWEL-A SOUND
            case 'e': return dialogueData.vowelSounds.Length > 1 ? dialogueData.vowelSounds[1] : null; // VOWEL-E SOUND
            case 'i': return dialogueData.vowelSounds.Length > 2 ? dialogueData.vowelSounds[2] : null; // VOWEL-I SOUND
            case 'o': return dialogueData.vowelSounds.Length > 4 ? dialogueData.vowelSounds[4] : null; // VOWEL-O SOUND
            case 'u': return dialogueData.vowelSounds.Length > 5 ? dialogueData.vowelSounds[5] : null; // VOWEL-U SOUND
            default: return dialogueData.vowelSounds[Random.Range(0, dialogueData.vowelSounds.Length)];
        }
    }

    private AudioClip GetConsonantSound(char consonant)
    {
        if (dialogueData.consonantSounds == null || dialogueData.consonantSounds.Length == 0)
            return null;

        // For now, use random consonant sounds. You can expand this later for specific consonant mapping
        return dialogueData.consonantSounds[Random.Range(0, dialogueData.consonantSounds.Length)];
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);

        // Check if this NPC buys fish first
        if (buysFish)
        {
            HandleFishSelling();
        }
        // Then check if this NPC has a shop and should open it after dialogue
        else if (hasShop)
        {
            OpenShop();
        }
        else
        {
            // If no shop or fish buying, unpause the game
            PauseController.SetPause(false);
        }
    }

    public void OpenShop()
    {
        if (!hasShop || shopMenu == null) return;

        isShopActive = true;
        shopMenu.SetActive(true);
        // Keep game paused while shop is open
        PauseController.SetPause(true);
    }

    public void CloseShop()
    {
        if (shopMenu == null) return;

        isShopActive = false;
        shopMenu.SetActive(false);
        PauseController.SetPause(false);
    }

    private void HandleFishSelling()
    {
        int fishQuantity = GetFishQuantity();
        
        if (fishQuantity > 0)
        {
            int totalCoins = fishQuantity * coinsPerFish;
            
            // Remove all fish from inventory
            playerInventory.RemoveItem(fishItem, fishQuantity);
            
            // Add coins to player
            playerInventory.coins += totalCoins;
            playerInventory.UpdateCoinDisplay();
            
            // Show message through dialogue system
            string sellMessage = $"You have {fishQuantity} Fish in your inventory. Here is {totalCoins} coins!";
            StartCoroutine(ShowFishSellMessage(sellMessage));
        }
        else
        {
            // No fish to sell, just unpause
            PauseController.SetPause(false);
        }
    }

    private int GetFishQuantity()
    {
        Item playerFish = playerInventory.items.Find(i => i.itemName == fishItem.itemName);
        return playerFish != null ? playerFish.itemQuantity : 0;
    }

    private IEnumerator ShowFishSellMessage(string message)
    {
        // Show the fish selling message
        string currentText = "";
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(true);
        
        // Type out the message
        foreach (char letter in message)
        {
            currentText += letter;
            dialogueUI.SetDialogueText(currentText);
            PlayPhoneticSound(letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }
        
        // Wait for player to interact to close (using existing playerInput)
        bool waitingForInput = true;
        while (waitingForInput)
        {
            if (playerInput.Player.Interact.triggered)
            {
                waitingForInput = false;
            }
            yield return null;
        }
        
        // Close dialogue and unpause
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);
    }

    // Public methods for external scripts (like UI buttons)
    public bool HasShop()
    {
        return hasShop;
    }

    public bool IsShopOpen()
    {
        return isShopActive;
    }
}