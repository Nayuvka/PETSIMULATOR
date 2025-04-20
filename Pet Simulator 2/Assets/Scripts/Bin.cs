using UnityEngine;

public class Bin : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Shake()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Shake"); // optional but helps if it bugs
            animator.SetTrigger("Shake");
        }
    }

}

