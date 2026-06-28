using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    public GameObject itemPrefab;

    [Header("Spawn Settings")]
    public float respawnDelay = 1.0f;
    // How far the item needs to move before we consider it "taken"
    public float distanceToTriggerRespawn = 0.2f;

    private GameObject currentSpawnedItem;
    private bool isOccupied = false;

    void Start()
    {
        SpawnItem();
    }

    void Update()
    {
        // If we have an item, check how far it is from the spawner
        if (isOccupied && currentSpawnedItem != null)
        {
            float distance = Vector3.Distance(transform.position, currentSpawnedItem.transform.position);

            // If the item was moved away (picked up or knocked over)
            if (distance > distanceToTriggerRespawn)
            {
                isOccupied = false;
                currentSpawnedItem = null;
                StartCoroutine(RespawnTimer());
            }
        }
        // If the item was destroyed (e.g., dropped in the bottle) while still sitting on the spawner
        else if (isOccupied && currentSpawnedItem == null)
        {
            isOccupied = false;
            StartCoroutine(RespawnTimer());
        }
    }

    void SpawnItem()
    {
        if (itemPrefab == null) return;
        currentSpawnedItem = Instantiate(itemPrefab, transform.position, transform.rotation);
        isOccupied = true;
    }

    IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnItem();
    }
}