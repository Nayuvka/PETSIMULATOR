using UnityEngine;
using System;

public class Poop : MonoBehaviour
{
    public Action OnCleanedUp;

    private bool isDragging = false;
    private bool isOverBin = false;
    private PetManager petManager;
    private float objectZ; // Store the object's original Z position

    void Awake()
    {
        petManager = GameObject.Find("Managers").GetComponent<PetManager>();
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

        if (isOverBin)
        {
            OnCleanedUp?.Invoke();
            Destroy(gameObject);
            petManager.AddHappiness(5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bin"))
        {
            isOverBin = true;

            // Trigger shake animation via Animator
            Bin bin = other.GetComponent<Bin>();
            if (bin != null)
            {
                bin.Shake();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Bin"))
        {
            isOverBin = false;
        }
    }
}