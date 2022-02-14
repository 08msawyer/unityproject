using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShurikenController : NetworkBehaviour
{
    private Vector3 _startDirection;

    internal AnimalFightingController Owner;

    public float speed = 50f;
    public float rotationSpeed = 100f;

    private void Start()
    {
        if (!IsServer) return;
        
        StartCoroutine(DespawnAfterTime());
        _startDirection = transform.forward;
    }

    private IEnumerator DespawnAfterTime()
    {
        yield return new WaitForSeconds(5);
        NetworkObject.Despawn();
    }

    private void Update()
    {
        if (!IsServer) return;

        var localTransform = transform;
        localTransform.position += _startDirection * (speed * Time.deltaTime);
        var rotation = localTransform.rotation;
        rotation.eulerAngles += Vector3.up * (rotationSpeed * Time.deltaTime);
        localTransform.rotation = rotation;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!IsServer) return;

        var animal = other.gameObject.GetComponent<AnimalFightingController>();
        if (animal == Owner) return;

        NetworkObject.Despawn();
        if (animal != null)
        {
            animal.DamageServerRpc(Owner.damage);
        }
    }
}