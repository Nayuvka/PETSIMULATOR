using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void MoveToScene(int sceneID)
    {
        Debug.Log("Button clicked! Trying to load scene: " + sceneID);
        SceneManager.LoadScene(sceneID);
    }



}
