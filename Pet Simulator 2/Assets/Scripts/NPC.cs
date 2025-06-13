using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    private DialogueController dialogueUI;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private string dialogueStartSoundName = "dialogue_open";
    [SerializeField] private string dialogueEndSoundName = "dialogue_close";

    // Define vowels and consonants
    private readonly char[] vowels = { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' };
    private readonly char[] consonants = {'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z',
                                         'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z'};
    private void Start()
    {
        dialogueUI = DialogueController.Instance;
    }
    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
            return;

        if (isDialogueActive)
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
        //nameText.SetText(dialogueData.npcName);
        //portraitImage.sprite = dialogueData.npcPortrait;
        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait); 
        dialogueUI.ShowDialogueUI(true);
       // dialoguePanel.SetActive(true);
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
            // dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
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
        dialogueUI.SetDialogueText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);

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
        PauseController.SetPause(false);

        if (!string.IsNullOrEmpty(dialogueEndSoundName))
        {
            SoundEffectManager.Play(dialogueEndSoundName);
        }
    }
}