using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetManager : MonoBehaviour
{
    [SerializeField] private PetController pet;
    [SerializeField] private Slider hungerBar;
    [SerializeField] private float hungerTime = 60f;
    public float petMoveTimer, originalpetMoveTimer;
    public Transform[] waypoints;
    public float foodValue = 50f;
    
     private void Awake()
    {
        originalpetMoveTimer = petMoveTimer;
    }

    private void Start()
    {
        
        hungerBar.minValue = 0f;  
        hungerBar.maxValue = 100f; 
        hungerBar.value = foodValue;  
        StartCoroutine(DecreaseFoodValue());

    }
    // Update is called once per frame
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
        hungerBar.value = foodValue;
        if (foodValue > 50f)
        {
            pet.Happy();
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
            }

            if (foodValue < 50f)
            {
                pet.Sad();
            }

        }
    }

}
