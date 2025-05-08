using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragFood : MonoBehaviour
{
    [SerializeField] private PetManager petManager;
    [SerializeField] private SpawnFood spawnFood;
    private bool isDragging = false;
    private float objectZ;

    void Start()
    {
        petManager = GameObject.Find("Managers")?.GetComponent<PetManager>();
        spawnFood = GameObject.Find("Feed")?.GetComponent<SpawnFood>();
        
        // Store the original Z position
        objectZ = transform.position.z;
    }
    
    private void OnMouseDown()
    {
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            // Get mouse position with proper z depth
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z - objectZ);
            
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            
            // Keep the original z position of the object
            transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, objectZ);
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
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