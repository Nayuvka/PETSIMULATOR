using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private Transform DenDoor;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform StartTransform;
    [SerializeField] private GameObject BuildButton;
    
    [Header("Camera Bounds")]
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private Collider2D regularBounds;
    [SerializeField] private Collider2D denBounds;
    
    private bool inDen;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = playerTransform.position;
        confiner.m_BoundingShape2D = regularBounds;
    }

    public void MoveToScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    // Original method for backwards compatibility
    public void MoveToDen()
    {
        MoveToDen(true);
    }

    // New simplified method
    public void MoveToDen(bool isEntering)
    {
        if (!inDen && isEntering)
        {
            // Entering the den
            playerTransform.position = DenDoor.position;
            BuildButton.SetActive(true);
            confiner.m_BoundingShape2D = denBounds;
            inDen = true;
            
            Debug.Log("Entered den");
        }
        else if (inDen && !isEntering)
        {
            // Exiting the den - always return to StartTransform position
            playerTransform.position = StartTransform.position;
            BuildButton.SetActive(false);
            confiner.m_BoundingShape2D = regularBounds;
            inDen = false;
            
            Debug.Log($"Exited den. Returned to position: {StartTransform.position}");
        }
    }
}