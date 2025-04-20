using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public GameObject PoopPrefab;
    public float Radius = 1;
    public float MinSpawnTime = 1f;
    public float MaxSpawnTime = 5f;

    private List<GameObject> currentPoops = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnRandomly());
    }

    IEnumerator SpawnRandomly()
    {
        while (true)
        {
            if (currentPoops.Count < 2)
            {
                Vector3 randomPos = (Vector2)transform.position + Random.insideUnitCircle * Radius;

                GameObject newPoop = Instantiate(PoopPrefab, randomPos, Quaternion.identity);
                currentPoops.Add(newPoop);

                // Hook into the poop’s cleanup logic
                Poop poopScript = newPoop.GetComponent<Poop>();
                if (poopScript != null)
                {
                    poopScript.OnCleanedUp += () => currentPoops.Remove(newPoop);
                }
            }

            float waitTime = Random.Range(MinSpawnTime, MaxSpawnTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}