using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    public GameObject itemPrefab; // Assign your Pill or Bottle prefab here

    [Header("Spawn Settings")]
    public float respawnDelay = 1.0f; // Time in seconds before a new one appears

    private GameObject currentSpawnedItem;
    private bool isOccupied = false;

    void Start()
    {
        SpawnItem();
    }

    void SpawnItem()
    {
        if (itemPrefab == null) return;

        // Spawn the item at the exact position and rotation of this spawner object
        currentSpawnedItem = Instantiate(itemPrefab, transform.position, transform.rotation);
        isOccupied = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object leaving the zone is the one we spawned
        if (isOccupied && other.gameObject == currentSpawnedItem)
        {
            isOccupied = false;
            currentSpawnedItem = null;

            // Start a timer to spawn the next one so it doesn't pop in instantly
            StartCoroutine(RespawnTimer());
        }
    }

    IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnItem();
    }
}