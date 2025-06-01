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
    void Start()
    {
        waypoints = new Transform[waypointParent.childCount];

        for(int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent .GetChild(i); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(PauseController.IsGamePaused || isWaiting)
        {
            return;
        }

        MoveToWaypoint();
    }
    void MoveToWaypoint()
    {
        Transform target = waypoints[currentwaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);  
        
        if(Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        //if looping enabled - increment currentwaypointindex and wrap around if needed 
        //if not looping- increment currentwaypointindex but dont exceed last waypoint
        currentwaypointIndex = loopWaypoints ? (currentwaypointIndex +1) % waypoints.Length : Mathf.Min(currentwaypointIndex + 1, waypoints.Length - 1);

        isWaiting = false; 
            }
}
