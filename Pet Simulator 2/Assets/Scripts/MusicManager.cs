using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager Instance;
    private AudioSource audioSource;
    public AudioClip backgroundMusic;
    [SerializeField] private Slider musicSlider;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        { 
         Destroy(gameObject);
        }
    }

    private void Start()
    {
        if(backgroundMusic != null) 
        { 
            PlayBackgroundMusic(false,  backgroundMusic);
        }
        //volume slider 
        musicSlider.onValueChanged.AddListener(delegate { SetVolume(musicSlider.value); });
    }
    //volume slider 
    public static void SetVolume(float volume)
    {
        Instance.audioSource.volume = volume;
    }

    public void PlayBackgroundMusic(bool resetSong, AudioClip audioClip = null)
    {

        if(audioClip != null) 
        {
            audioSource.clip = audioClip;
        }
        else if (audioSource.clip != null) 
        {
            if (resetSong) 
            {

                audioSource.Stop();
            }
            audioSource.Play();
        }
    }
}
