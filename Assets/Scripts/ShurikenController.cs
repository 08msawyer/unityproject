using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShurikenController : MonoBehaviour
{
    private Vector3 _startDirection;

    internal AnimalFightingController Owner;

    public float speed = 50f;
    public float rotationSpeed = 100f;

    private void Start()
    {
        Destroy(gameObject, 5f);
        _startDirection = transform.forward;
    }

    private void Update()
    {
        var localTransform = transform;
        localTransform.position += _startDirection * (speed * Time.deltaTime);
        localTransform.Rotate(localTransform.up, rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        var animal = other.gameObject.GetComponent<AnimalFightingController>();
        if (animal == Owner) return;

        Destroy(gameObject);
        if (animal != null)
        {
            animal.DamageServerRpc(Owner.damage);
        }
    }
}