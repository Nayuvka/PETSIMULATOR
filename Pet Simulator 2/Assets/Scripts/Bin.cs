using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bin : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string shakeAnimationTrigger = "Shake";

    private void Awake()
    {
        // If animator is not set in the inspector, try to get it from this GameObject
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void Shake()
    {
        // Trigger the shake animation
        if (animator != null)
        {
            animator.SetTrigger(shakeAnimationTrigger);
        }
        else
        {
            Debug.LogWarning("No Animator component found on Bin");
        }
    }
}



