using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// Spawns [initialFrogs] frogs when the game starts, and continuously spawns another frog every [spawnDelay] seconds.
/// </summary>
public class NPCSpawner : NetworkBehaviour
{
    public GameObject frog;
    public int initialFrogs = 10;
    public float spawnDelay = 20f;

    /// <summary>
    /// Begins the spawning when the server starts.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        StartCoroutine(FrogSpawnCoroutine());
    }

    /// <summary>
    /// A coroutine which spawns the initial batch of frogs, and then continuously spawns more every [spawnDelay] seconds.
    /// </summary>
    private IEnumerator FrogSpawnCoroutine()
    {
        for (var i = 0; i < initialFrogs; i++)
        {
            SpawnFrog();
        }
        while (true)
        {
            SpawnFrog();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    /// <summary>
    /// Spawns a single frog at a random location.
    /// </summary>
    private void SpawnFrog()
    {
        try
        {
            var pos = GetRandomPosition();
            var rotation = frog.transform.rotation * Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
            var frogInstance = Instantiate(frog, pos, rotation);
            frogInstance.GetComponent<NetworkObject>().Spawn();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    
    /// <summary>
    /// Finds a random location on the map.
    /// </summary>
    /// <returns>The location.</returns>
    /// <exception cref="Exception">If a spawn could not be found.</exception>
    private Vector3 GetRandomPosition()
    {
        var worldBounds = GameObject.FindWithTag("World").GetComponentInChildren<Collider>().bounds;
        var x = Random.Range(worldBounds.min.x, worldBounds.max.x);
        var z = Random.Range(worldBounds.min.z, worldBounds.max.z);
        var position = new Vector3(x, worldBounds.max.y, z);

        var success = NavMesh.SamplePosition(position, out var hit, worldBounds.max.y, -1);
        if (!success) throw new Exception("AAA");
        return hit.position;
    }
}
