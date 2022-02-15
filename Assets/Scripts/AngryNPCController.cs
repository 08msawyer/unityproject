using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class AngryNPCController : NetworkBehaviour
{
    private static readonly int Walking = Animator.StringToHash("Walking");

    private readonly NetworkVariable<Vector3> _tongueTarget = new();
    private readonly NetworkVariable<bool> _tongueActive = new();
    private readonly NetworkVariable<bool> _tongueExtending = new();

    private NavMeshAgent _agent;
    private Animator _animator;
    private LineRenderer _tongue;
    private Transform _headTransform;
    private GameObject _target;

    private void Start()
    {
        _agent = GetComponentInParent<NavMeshAgent>();
        _animator = GetComponentInParent<Animator>();
        _tongue = GetComponentInParent<LineRenderer>();
        _headTransform = transform.parent.Find("FrogArmature").Find("root").Find("Body").Find("Shoulders").Find("Neck")
            .Find("Head").Find("Head_end");
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) StartCoroutine(TongueLoopCoroutine());
    }

    private IEnumerator TongueLoopCoroutine()
    {
        while (true)
        {
            if (_target == null) yield return null;
            else
            {
                _tongueExtending.Value = false;
                yield return new WaitForSeconds(3);
                _tongueExtending.Value = true;
                _tongueTarget.Value = _target.GetComponent<Rigidbody>().worldCenterOfMass;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public void SetTarget(NetworkObject target)
    {
        Assert.IsTrue(IsServer);
        _target = target.gameObject;
        // // _tongueTarget.Value = _target.GetComponent<Rigidbody>().worldCenterOfMass;
        _tongueActive.Value = true;
    }

    private void LateUpdate()
    {
        if (!_tongueActive.Value)
        {
            _tongue.enabled = false;
            _tongue.SetPosition(1, _tongue.GetPosition(0));
            return;
        }

        _tongue.enabled = true;

        _tongue.SetPosition(0, transform.InverseTransformPoint(_headTransform.position));
        var target = _tongueExtending.Value ? transform.InverseTransformPoint(_tongueTarget.Value) : _tongue.GetPosition(0);
        _tongue.SetPosition(1, Vector3.MoveTowards(_tongue.GetPosition(1), target, Time.deltaTime * 250));
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