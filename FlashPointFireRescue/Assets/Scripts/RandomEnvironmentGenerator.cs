using UnityEngine;
using System.Collections.Generic;

public class RandomEnvironmentGenerator : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject prefab;
        public int minCount;
        public int maxCount;
    }

    public List<SpawnableObject> spawnableObjects;
    public GameObject worldFloor;
    public int maxSpawnAttempts = 30;
    public float bufferZone = 5f; // Buffer zone around the floor to prevent spawn on it

    // World plane dimensions and boundaries
    private float minX = -80f;
    private float maxX = 140f;
    private float minZ = -70f;
    private float maxZ = 150f;

    // Floor dimensions
    private Vector2 floorSize = new Vector2(60f, 80f);
    private Vector3 floorCenter = new Vector3(30f, 0f, 40f);

    void Start()
    {
        GenerateEnvironment();
    }

    public void GenerateEnvironment()
    {
        foreach (SpawnableObject obj in spawnableObjects)
        {
            int count = Random.Range(obj.minCount, obj.maxCount + 1);
            for (int i = 0; i < count; i++)
            {
                Vector3? validPosition = GetValidRandomPosition();
                if (validPosition.HasValue)
                {
                    GameObject spawnedObj = Instantiate(obj.prefab, validPosition.Value, Quaternion.identity, transform);

                }
                else
                {
                    Debug.LogWarning($"Couldn't find a valid position for {obj.prefab.name} after {maxSpawnAttempts} attempts.");
                }
            }
        }
    }

    private Vector3? GetValidRandomPosition()
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector3 randomPosition = GetRandomPosition();

            if (IsValidSpawnPosition(randomPosition))
            {
                return randomPosition;
            }
        }
        return null;
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);
        float y = worldFloor.transform.position.y;

        RaycastHit hit;
        if (Physics.Raycast(new Vector3(x, y + 100f, z), Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == worldFloor)
            {
                y = hit.point.y;
            }
        }

        return new Vector3(x, y, z);
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        // Check if the position is within the world bounds
        if (position.x < minX || position.x > maxX ||
            position.z < minZ || position.z > maxZ)
        {
            return false;
        }

        // Check if the position is on or near the floor (including buffer zone)
        if (position.x >= floorCenter.x - floorSize.x / 2f - bufferZone &&
            position.x <= floorCenter.x + floorSize.x / 2f + bufferZone &&
            position.z >= floorCenter.z - floorSize.y / 2f - bufferZone &&
            position.z <= floorCenter.z + floorSize.y / 2f + bufferZone)
        {
            return false;
        }

        return true;
    }
}