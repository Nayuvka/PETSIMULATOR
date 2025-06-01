using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    [Header("NPC Info")]
    public string npcName;
    public Sprite npcPortrait;

    [Header("Dialogue Content")]
    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;

    [Header("Phonetic Audio Settings")]
    public AudioClip[] vowelSounds;      // a, e, i, o, u sounds
    public AudioClip[] consonantSounds;  // b, c, d, f, g, etc. sounds
    public AudioClip silenceSound;       // for spaces and punctuation
    [Range(0.5f, 2f)]
    public float voicePitch = 1f;
    [Range(0f, 1f)]
    public float voiceVolume = 1f;

    [Header("Sound Effect Names")]
    public string customStartSound;
    public string customEndSound;
}
