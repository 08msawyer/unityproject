using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class AngryNPCController : NetworkBehaviour
{
    private static readonly int Walking = Animator.StringToHash("Walking");
    
    private NavMeshAgent _agent;
    private Animator _animator;
    private GameObject target;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _agent = GetComponentInParent<NavMeshAgent>();
        _animator = GetComponentInParent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;
        _animator.SetBool(Walking, target != null);
        if (target != null)
        {
            _agent.isStopped = false;
            _agent.SetDestination(target.transform.position);
        }
        else
        {
            _agent.isStopped = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsServer || target != null) return;

        if (other.gameObject.GetComponent<AnimalFightingController>() != null)
        {
            target = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target)
        {
            target = null;
        }
    }
}