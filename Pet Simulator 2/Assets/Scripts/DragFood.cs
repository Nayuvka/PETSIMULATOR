using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DragFood : MonoBehaviour
{
    [SerializeField] private PetManager petManager;
    [SerializeField] private SpawnFood spawnFood;
    private bool isDragging = false;
    private Camera mainCam;
    public LayerMask FoodLayer;

    void Start()
    {
        mainCam = Camera.main;
        petManager = GameObject.Find("Managers")?.GetComponent<PetManager>();
        spawnFood = GameObject.Find("Feed")?.GetComponent<SpawnFood>();
    }
    void Update()
    {
        DragObject();
    }
    private void DragObject()
    {
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D click = Physics2D.OverlapPoint(mousePos);
            if (click != null && click.gameObject == gameObject)
            {
                isDragging = true;
            }
        }
        if (isDragging)
        {
            Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = transform.position.z;
            transform.position = mousePos;
        }
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
        }
    }
    void OnTriggerEnter2D(Collider2D pet)
    {
        if (pet.CompareTag("Pet") && isDragging)
        {
            // Get the pet controller
            PetController petController = pet.GetComponent<PetController>();
            if (petController != null)
            {
                // Call the Eat method which will trigger animation
                petController.Eat();

                // Play eating sound
                AudioClip eatingSound = petController.eatingSound;
                if (petController.audioSource != null && eatingSound != null)
                {
                    petController.audioSource.clip = eatingSound;
                    petController.audioSource.Play();
                }
            }

            petManager.AddFood();
            spawnFood.canSpawnFood = true;
            Destroy(gameObject);
        }
    }
}
