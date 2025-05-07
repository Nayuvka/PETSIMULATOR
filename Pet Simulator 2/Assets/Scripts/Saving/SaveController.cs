using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private string saveLocation; 
    void Start()
    {
        //define save location 
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        LoadGame();

    }

    public void SaveGame()
    {

        SaveData saveData = new SaveData
        {
            petPosition = GameObject.FindGameObjectWithTag("Pet").transform.position,
            mapBoundary = FindObjectOfType<CinemachineConfiner>().m_BoundingShape2D.gameObject.name

        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));






    }

    public void LoadGame()
    {

        if (File.Exists(saveLocation))
        {

            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

            GameObject.FindGameObjectWithTag("Pet").transform.position = saveData.petPosition;

            FindObjectOfType<CinemachineConfiner>().m_BoundingShape2D = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();
        }
        else
        {
            SaveGame();
        }
    }
}
