using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject fishPrefab;

    public int maxFishCount = 10;
    private int currentFishCount = 0;

    public float spawnRate = 1f; //[spawnrate] number of fish per second
    private float nextSpawn = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn && currentFishCount < maxFishCount)
        {
            SpawnFish();
            nextSpawn = Time.time + 1f / spawnRate;
        }
    }

    void SpawnFish()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(fishPrefab, spawnPosition, Quaternion.identity);
        currentFishCount++;
    }

    Vector3 GetRandomSpawnPosition()
    {
        Camera cam = Camera.main;
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float camHeight = cam.orthographicSize * 2;
        float camWidth = camHeight * screenAspect;

        float x = 0f;
        float y = 0f;

        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0: // Left
                x = -camWidth / 2 - 1;
                y = Random.Range(-camHeight / 2, camHeight / 2);
                break;
            case 1: // Right
                x = camWidth / 2 + 1;
                y = Random.Range(-camHeight / 2, camHeight / 2);
                break;
            case 2: // Top
                x = Random.Range(-camWidth / 2, camWidth / 2);
                y = camHeight / 2 + 1;
                break;
            case 3: // Bottom
                x = Random.Range(-camWidth / 2, camWidth / 2);
                y = -camHeight / 2 - 1;
                break;
        }

        return new Vector3(x, y, 0);
    }

    public void fishDied()
    {
        currentFishCount--;
    }
}
