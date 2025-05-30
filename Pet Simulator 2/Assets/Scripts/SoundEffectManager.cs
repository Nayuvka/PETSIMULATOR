using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager instance;
    private AudioSource audioSource;
    private SoundEffectLibrary SoundEffectLibrary;
    [SerializeField] private Slider sfxSlider; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            //soundEffectLibrary = GetComponent<SoundEffectLibrary>();
            DontDestroyOnLoad(gameObject);


        }
        else
        {
            Destroy(gameObject);    
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    [Header("Audio")]
    [SerializeField] private AudioClip dialogueTypingSoundClip;
      

    // Update is called once per frame
   

    //play a sound every time add on a character to a dialogue line. 
   // audioSource.PlayOneShot(dialogueTypingSoundClip);
}
