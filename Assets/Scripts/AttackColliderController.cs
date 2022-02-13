using System;
using UnityEngine;

public class AttackColliderController : MonoBehaviour
{
    private AnimalFightingController _parent;
    private void Start()
    {
        _parent = gameObject.GetComponentInParent<AnimalFightingController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherAnimal = other.gameObject.GetComponent<AnimalFightingController>();
        if (otherAnimal != null)
        {
            _parent.CurrentTarget = otherAnimal;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var otherAnimal = other.gameObject.GetComponent<AnimalFightingController>();
        if (_parent.CurrentTarget == otherAnimal)
        {
            _parent.CurrentTarget = null;
        }
    }
}