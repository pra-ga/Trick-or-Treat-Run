using UnityEngine;
using System.Collections.Generic;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject[] platformPrefabs; // Array of different platform prefabs to spawn
    public Transform playerTransform;    // Reference to the player's transform
    public float spawnDistance = 90f;    // Distance ahead of the player to spawn new platforms
    public float despawnDistance = 10f; // Distance behind the player to despawn platforms
    public float platformLength = 100f;   // The approximate length of a single platform prefab

    private List<GameObject> activePlatforms = new List<GameObject>();
    private float nextSpawnZ; // Z-coordinate where the next platform should be spawned

    void Start()
    {
        // Initialize the first few platforms
        for (int i = 0; i < 3; i++) // Spawn a few platforms at the start
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        // Spawn new platforms if the player is approaching the end of the current platforms
        if (playerTransform.position.z + spawnDistance > nextSpawnZ - (platformLength / 2f))
        {
            SpawnPlatform();
        }

        // Despawn platforms that are far behind the player
        if (activePlatforms.Count > 0 && playerTransform.position.z - despawnDistance > activePlatforms[0].transform.position.z + (platformLength / 2f))
        {
            DespawnPlatform();
        }
    }

    void SpawnPlatform()
    {
        // Choose a random platform prefab
        int randomIndex = Random.Range(0, platformPrefabs.Length);
        GameObject platform = Instantiate(platformPrefabs[randomIndex], new Vector3(0, 0, nextSpawnZ), Quaternion.identity);
        activePlatforms.Add(platform);

        // Update the next spawn position
        nextSpawnZ += platformLength; 
    }

    void DespawnPlatform()
    {
        // Destroy the oldest platform and remove it from the list
        Destroy(activePlatforms[0]);
        activePlatforms.RemoveAt(0);
    }
}