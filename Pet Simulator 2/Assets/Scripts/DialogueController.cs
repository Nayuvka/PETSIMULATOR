using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; } //singleton instance
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); //only one instance 
    }

  public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show); //Toggle UI Visibility 
    }

    public void SetNPCInfo(string npcName, Sprite portrait)
    {
        nameText.text = npcName;
        portraitImage.sprite = portrait;

    }


    public  void SetDialogueText(string text)
    {
        dialogueText.text = text;   
    }

}
