using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PetManager : MonoBehaviour
{
    [SerializeField] private PetController pet;
    [SerializeField] private Slider hungerBar;
    [SerializeField] private Slider happinessBar;
    [SerializeField] private float hungerTime = 60f;
    
    // Happiness decrease timing variables
    [SerializeField] private float slowHappinessDecreaseTime = 120f; // Every 2 minutes when food is 50-75
    [SerializeField] private float rapidHappinessDecreaseTime = 45f; // Every 45 seconds when food is below 25
    
    public float petMoveTimer, originalpetMoveTimer;
    public Transform[] waypoints;
    public float foodValue = 50f;
    public float happinessValue = 100f;
     public TextMeshProUGUI happinessText; 

    
    private Coroutine happinessCoroutine;
    
    private void Awake()
    {
        originalpetMoveTimer = petMoveTimer;
    }

    private void Start()
    {
        // Setup hunger system
        hungerBar.minValue = 0f;  
        hungerBar.maxValue = 100f; 
        hungerBar.value = foodValue;
        
        // Setup happiness system
        happinessBar.minValue = 0f;
        happinessBar.maxValue = 100f;
        happinessBar.value = happinessValue;
        
        StartCoroutine(DecreaseFoodValue());
        
        // Start happiness management based on current food level
        UpdateHappinessSystem();
        ChangeHappinessText();
    }

    void Update()
    {
        if(petMoveTimer > 0)
        {
            petMoveTimer -= Time.deltaTime;
        }
        else
        {
            MovePetToRandomWaypoint();
            petMoveTimer = originalpetMoveTimer;
        }
    }

    private void MovePetToRandomWaypoint()
    {
        int randomWaypoint = Random.Range(0, waypoints.Length);
        Vector3 destination = waypoints[randomWaypoint].position;

        // Lock Y to current pet position to keep pet on the ground
        destination.y = pet.transform.position.y;
        destination.z = pet.transform.position.z; // Optional for 2D
        pet.Move(destination);
    }

    public void AddFood()
    {
        foodValue += 25f;
        foodValue = Mathf.Min(foodValue, 100f); // Cap at 100
        hungerBar.value = foodValue;
        
        // Feeding the pet might make it a bit happier
        AddHappiness(5f);
        
        // Food value changed, so we need to update happiness system
        UpdateHappinessSystem();
        
        UpdatePetMood();
    }
    
    public void AddHappiness(float amount)
    {
        happinessValue += amount;
        happinessValue = Mathf.Min(happinessValue, 100f); // Cap at 100
        happinessBar.value = happinessValue;
        
        UpdatePetMood();
        ChangeHappinessText();
    }
    
    private void UpdatePetMood()
    {
        // Pet is happy when both food and happiness are good
        if (foodValue > 50f && happinessValue > 50f)
        {
            pet.Happy();
        }
        // Pet is sad when either food or happiness is very low
        else if (foodValue < 25f || happinessValue < 25f)
        {
            pet.Sad();
        }
    }

    private IEnumerator DecreaseFoodValue()
    {
        while (true)
        {
            yield return new WaitForSeconds(hungerTime);
            if (foodValue > 0f)
            {
                foodValue -= 25f;
                foodValue = Mathf.Max(foodValue, 0f);
                hungerBar.value = foodValue;
                
                // Food value changed, so we need to update happiness system
                UpdateHappinessSystem();
                
                UpdatePetMood();
            }
        }
    }

    // This method manages which happiness coroutine should be running
    private void UpdateHappinessSystem()
    {
        // Stop any existing happiness coroutine
        if (happinessCoroutine != null)
        {
            StopCoroutine(happinessCoroutine);
            happinessCoroutine = null;
        }
        
        // Start the appropriate coroutine based on food level
        if (foodValue >= 75f)
        {
            // No happiness decrease when food is high
            Debug.Log("Food level high, happiness stable");
        }
        else if (foodValue >= 25f && foodValue < 75f)
        {
            // Slow happiness decrease
            happinessCoroutine = StartCoroutine(DecreaseHappinessSlow());
            Debug.Log("Food level medium, happiness decreasing slowly");
        }
        else
        {
            // Rapid happiness decrease
            happinessCoroutine = StartCoroutine(DecreaseHappinessRapid());
            Debug.Log("Food level low, happiness decreasing rapidly");
        }
        ChangeHappinessText();
    }

    private IEnumerator DecreaseHappinessSlow()
    {
        while (true)
        {
            yield return new WaitForSeconds(slowHappinessDecreaseTime);
            
            if (happinessValue > 0f)
            {
                happinessValue -= 10f;
                happinessValue = Mathf.Max(happinessValue, 0f);
                happinessBar.value = happinessValue;
                
                UpdatePetMood();
            }
        }
    }
    
    private IEnumerator DecreaseHappinessRapid()
    {
        while (true)
        {
            yield return new WaitForSeconds(rapidHappinessDecreaseTime);
            
            if (happinessValue > 0f)
            {
                happinessValue -= 10f; // Same decrease amount, just happens more frequently
                happinessValue = Mathf.Max(happinessValue, 0f);
                happinessBar.value = happinessValue;
                
                UpdatePetMood();
            }
        }
    }
    
    // Methods to play with pet and increase happiness
    
    public void PlayWithPet()
    {
        AddHappiness(25f);
        // Playing uses energy, so decrease food a bit
        foodValue -= 5f;
        foodValue = Mathf.Max(foodValue, 0f);
        hungerBar.value = foodValue;
        
        // Food value changed, so may need to update happiness system
        UpdateHappinessSystem();
        
        UpdatePetMood();
    }

    private void ChangeHappinessText()
    {
        if (happinessValue >= 70)
        {
            happinessText.text = "Carefree";
        }
        else if (happinessValue < 70 && happinessValue >= 30)
        {
            happinessText.text = "Moderate";
        }
        else if (happinessValue < 30)
        {
            happinessText.text = "Neglected";
        }

    }
}