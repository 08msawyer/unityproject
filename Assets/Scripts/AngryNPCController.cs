using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class AngryNPCController : NetworkBehaviour
{
    private static readonly int Walking = Animator.StringToHash("Walking");

    private readonly NetworkVariable<ulong> _tongueTargetId = new();
    private readonly NetworkVariable<bool> _tongueActive = new();

    private NavMeshAgent _agent;
    private Animator _animator;
    private LineRenderer _tongue;
    private Transform _headTransform;
    private GameObject _target;
    private Rigidbody _targetRigidBody;

    private void Start()
    {
        _tongueTargetId.OnValueChanged = delegate(ulong _, ulong newId)
        {
            _targetRigidBody = NetworkManager.SpawnManager.SpawnedObjects[newId].gameObject
                .GetComponent<Rigidbody>();
        };
        _agent = GetComponentInParent<NavMeshAgent>();
        _animator = GetComponentInParent<Animator>();
        _tongue = GetComponentInParent<LineRenderer>();
        _headTransform = transform.parent.Find("FrogArmature").Find("root").Find("Body").Find("Shoulders").Find("Neck")
            .Find("Head").Find("Head_end");
    }

    public void SetTarget(NetworkObject target)
    {
        Assert.IsTrue(IsServer);
        _target = target.gameObject;
        _tongueTargetId.Value = target.NetworkObjectId;
        _tongueActive.Value = true;
    }

    private void LateUpdate()
    {
        if (!_tongueActive.Value)
        {
            _tongue.enabled = false;
            return;
        }

        _tongue.enabled = true;

        _tongue.SetPosition(0, transform.InverseTransformPoint(_headTransform.position));
        _tongue.SetPosition(1, transform.InverseTransformPoint(_targetRigidBody.worldCenterOfMass));
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;
        _animator.SetBool(Walking, _target != null);
        if (_target != null)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_target.transform.position);
        }
        else
        {
            _agent.isStopped = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsServer || _target != null) return;

        if (other.gameObject.GetComponent<AnimalFightingController>() != null)
        {
            SetTarget(other.GetComponent<NetworkObject>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;
        
        if (other.gameObject == _target)
        {
            _target = null;
            _tongueActive.Value = false;
        }
    }
}