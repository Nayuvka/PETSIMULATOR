using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWaypoints = true;

    private Transform[] waypoints;
    private int currentwaypointIndex;
    private bool isWaiting;
    private Animator animator;
    private SpriteRenderer spriteRenderer; // Add this for sprite flipping

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component

        waypoints = new Transform[waypointParent.childCount];
        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }
    }

    void Update()
    {
        if (PauseController.IsGamePaused || isWaiting)
        {
            animator.SetBool("isWalking", false);
            return;
        }
        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        Transform target = waypoints[currentwaypointIndex];
        Vector2 direction = (target.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Set animator parameters
        animator.SetFloat("InputX", direction.x);
        animator.SetBool("isWalking", direction.magnitude > 0f);

        // Flip the sprite based on movement direction
        FlipSprite(direction.x);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    void FlipSprite(float directionX)
    {
        if (spriteRenderer == null) return;

        // Only flip if there's significant horizontal movement
        if (Mathf.Abs(directionX) > 0.1f)
        {
            // Flip the sprite: true = facing left, false = facing right
            spriteRenderer.flipX = directionX < 0;
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(waitTime);

        //if looping enabled - increment currentwaypointindex and wrap around if needed 
        //if not looping- increment currentwaypointindex but dont exceed last waypoint
        currentwaypointIndex = loopWaypoints ? (currentwaypointIndex + 1) % waypoints.Length : Mathf.Min(currentwaypointIndex + 1, waypoints.Length - 1);
        isWaiting = false;
    }
}