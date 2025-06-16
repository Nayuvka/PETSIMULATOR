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
        StartTransform.position = playerTransform.position;
        
        confiner.m_BoundingShape2D = regularBounds;
    }

    public void MoveToScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    public void MoveToDen()
    {
        if (!inDen)
        {
            playerTransform.position = DenDoor.position;
            BuildButton.SetActive(true);
            confiner.m_BoundingShape2D = denBounds;
            inDen = true;
        }
        else
        {
            Vector3 returnPos = StartTransform != null ? StartTransform.position : startPosition;
            playerTransform.position = returnPos;
            BuildButton.SetActive(false);
            confiner.m_BoundingShape2D = regularBounds;
            inDen = false;
        }
    }
}