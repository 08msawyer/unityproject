using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class AngryNPCController : NetworkBehaviour
{
    private static readonly int Walking = Animator.StringToHash("Walking");
    
    private NavMeshAgent _agent;
    private Animator _animator;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _agent = GetComponentInParent<NavMeshAgent>();
        _animator = GetComponentInParent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;
        _animator.SetBool(Walking, _agent.desiredVelocity != Vector3.zero);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsServer) return;

        if (other.gameObject.GetComponent<AnimalFightingController>() != null)
        {
            _agent.SetDestination(other.gameObject.transform.position);
        }
    }
}