using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PetNameManager : MonoBehaviour
{
    public static PetNameManager Instance { get; private set; }
    public string NameTag { get; set; }


    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
