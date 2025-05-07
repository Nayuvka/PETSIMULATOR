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

    // Removed waypoint-related variables since we're using direct WASD control
    public float foodValue = 50f;
    public float happinessValue = 100f;
    public TextMeshProUGUI happinessText;

    private Coroutine happinessCoroutine;

    // Idleness tracking - to encourage movement
    private float idleTimer = 0f;
    private float idleThreshold = 30f; // Consider pet "idle" after 30 seconds of no movement
    private Vector3 lastPosition;

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

        // Initialize idleness tracking
        lastPosition = pet.transform.position;
    }

    void Update()
    {
        // Track idleness - being idle for too long might affect happiness
        if (Vector3.Distance(pet.transform.position, lastPosition) < 0.01f)
        {
            idleTimer += Time.deltaTime;

            // If idle for too long, slightly decrease happiness
            if (idleTimer > idleThreshold)
            {
                AddHappiness(-2f * Time.deltaTime); // Gradually decrease happiness

                // Occasionally remind the player to move around
                if (Random.value < 0.0005f) // Very small chance per frame
                {
                    // You could add a UI hint here if desired
                    Debug.Log("Pet seems bored. Try moving around!");
                }
            }
        }
        else
        {
            // Reset idle timer when the pet moves
            idleTimer = 0f;
            lastPosition = pet.transform.position;

            // Small happiness boost for moving around
            if (Random.value < 0.01f) // Small chance per frame while moving
            {
                AddHappiness(0.1f);
            }
        }
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

        // Play eat animation
        pet.Eat();
    }

    public void AddHappiness(float amount)
    {
        happinessValue += amount;
        happinessValue = Mathf.Clamp(happinessValue, 0f, 100f); // Use Clamp for both min and max
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
        // Pet is hungry when food is getting low but not critical
        else if (foodValue < 40f && foodValue >= 25f)
        {
            pet.Hungry();
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
        }
        else if (foodValue >= 25f && foodValue < 75f)
        {
            // Slow happiness decrease
            happinessCoroutine = StartCoroutine(DecreaseHappinessSlow());
        }
        else
        {
            // Rapid happiness decrease
            happinessCoroutine = StartCoroutine(DecreaseHappinessRapid());
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
        if (foodValue > 0f) foodValue -= 5f;
        hungerBar.value = foodValue;

        // Food value changed, so may need to update happiness system
        UpdateHappinessSystem();

        UpdatePetMood();
    }

    private void ChangeHappinessText()
    {
        if (happinessText == null) return;

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