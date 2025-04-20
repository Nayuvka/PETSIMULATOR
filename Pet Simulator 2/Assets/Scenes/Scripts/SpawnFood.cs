using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpawnFood : MonoBehaviour
{
    [SerializeField] private PetManager petManager;
    public Button feedButton; 
    public bool canSpawnFood = true;
    public GameObject foodPrefab; 
    public Transform foodSpawner;

    private void Start()
    {
        feedButton.onClick.AddListener(SpawnFoodPrefab);
    }

    void SpawnFoodPrefab()
    {
        if (canSpawnFood == true && petManager.foodValue < 100f)
        {
            Instantiate(foodPrefab, foodSpawner.position, foodSpawner.rotation);
            canSpawnFood = false;
        }
    }

}
