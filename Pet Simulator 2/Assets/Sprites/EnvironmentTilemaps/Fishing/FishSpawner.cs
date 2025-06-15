using UnityEngine;
using System.Collections.Generic;

public class FishSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject fishPrefab;
    public int maxFishCount = 10;
    public float spawnInterval = 2f;
    
    [Header("Spawn Area")]
    public Vector2 spawnAreaSize = new Vector2(10f, 6f);
    public Transform spawnCenter;
    
    private List<GameObject> spawnedFish = new List<GameObject>();
    private float nextSpawnTime;
    
    void Start()
    {
        if (spawnCenter == null)
            spawnCenter = transform;
    }
    
    void Update()
    {
        spawnedFish.RemoveAll(fish => fish == null);
        
        if (spawnedFish.Count < maxFishCount && Time.time >= nextSpawnTime)
        {
            SpawnFish();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
    
    void SpawnFish()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject newFish = Instantiate(fishPrefab, spawnPos, Quaternion.identity);
        
        FishScript fishScript = newFish.GetComponent<FishScript>();
        fishScript.SetMovementBounds(GetSpawnBounds());
        
        spawnedFish.Add(newFish);
    }
    
    Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float y = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        
        return spawnCenter.position + new Vector3(x, y, 0);
    }
    
    Bounds GetSpawnBounds()
    {
        return new Bounds(spawnCenter.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 1));
    }
    
    void OnDrawGizmosSelected()
    {
        Vector3 center = spawnCenter ? spawnCenter.position : transform.position;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 1));
    }
}