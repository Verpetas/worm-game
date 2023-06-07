using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    [SerializeField] Vector2 mapBoundsX;
    [SerializeField] Vector2 mapBoundsY;

    [SerializeField] GameObject[] resources;
    [SerializeField] GameObject[] powerups;

    [SerializeField] GameObject carrot;

    public static ResourceHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InvokeRepeating("SpawnResource", 1f, 2.5f);
    }

    void SpawnResource()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(mapBoundsX.x, mapBoundsX.y),
            Random.Range(mapBoundsY.x, mapBoundsY.y)
        );

        GameObject resourceToSpawn = resources[Random.Range(0, resources.Length)];

        if (Random.value < 0.1f) resourceToSpawn = powerups[Random.Range(0, powerups.Length)];

        Instantiate(resourceToSpawn, spawnPos, Quaternion.identity);
    }

    public void Root(Vector2 position)
    {
        Instantiate(carrot, new Vector2(position.x, 1.4f), Quaternion.identity);
    }
}
