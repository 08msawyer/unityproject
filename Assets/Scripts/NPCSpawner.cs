using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NPCSpawner : NetworkBehaviour
{
    private float _bottomBound;
    
    public GameObject frog;
    public int initialFrogs = 15;
    public float spawnDelay = 20f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _bottomBound = frog.GetComponent<Collider>().bounds.extents.y;
        StartCoroutine(FrogSpawnCoroutine());
    }

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
    
    private Vector3 GetRandomPosition()
    {
        var worldBounds = GameObject.FindWithTag("World").GetComponentInChildren<Collider>().bounds;
        var x = Random.Range(worldBounds.min.x * 0.75f, worldBounds.max.x * 0.75f);
        var z = Random.Range(worldBounds.min.z * 0.75f, worldBounds.max.z * 0.75f);
        var position = new Vector3(x, worldBounds.max.y * 0.5f, z);

        var success = NavMesh.SamplePosition(position, out var hit, worldBounds.max.y, -1);
        if (!success) throw new Exception("AAA");
        return hit.position;

        // var raycastResult = Physics.Raycast(position, Vector3.down, out var hit);
        // if (!raycastResult)
        // {
        //     throw new Exception($"Could not find a spawn for {this}!");
        // }
        //
        // return hit.point + _bottomBound * Vector3.up;
    }
}
