using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

/// <summary>
/// Handles most of the logic relating to Frogs' movement and behaviour.
/// </summary>
public class AngryNPCController : NetworkBehaviour
{
    private static readonly int Walking = Animator.StringToHash("Walking");

    private readonly NetworkVariable<Vector3> _tongueTarget = new();
    private readonly NetworkVariable<bool> _tongueActive = new();
    private readonly NetworkVariable<bool> _tongueExtending = new();

    private NavMeshAgent _agent;
    private Animator _animator;
    private LineRenderer _tongue;
    private GameObject _tongueHitbox;
    private Transform _headTransform;
    
    internal GameObject Target;

    /// <summary>
    /// Initializes all the required fields when the NPC spawns.
    /// </summary>
    private void Start()
    {
        _agent = GetComponentInParent<NavMeshAgent>();
        _animator = GetComponentInParent<Animator>();
        _tongue = GetComponentInParent<LineRenderer>();
        _headTransform = transform.parent.Find("FrogArmature").Find("root").Find("Body").Find("Shoulders").Find("Neck")
            .Find("Head").Find("Head_end");
        _tongueHitbox = transform.parent.GetComponentInChildren<TongueDamageController>().gameObject;
    }

    /// <summary>
    /// Starts the logic for the tongue animation on the server.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        if (IsServer) StartCoroutine(TongueLoopCoroutine());
    }

    /// <summary>
    /// Continuously extends and retracts this frog's tongue.
    /// </summary>
    private IEnumerator TongueLoopCoroutine()
    {
        while (true)
        {
            if (Target == null) yield return null;
            else if (_tongueExtending.Value)
            {
                _tongueExtending.Value = false;
                yield return new WaitForSeconds(3);
            }
            else
            {
                _tongueExtending.Value = true;
                _tongueTarget.Value = Target.GetComponent<Rigidbody>().worldCenterOfMass;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    /// <summary>
    /// Called when a player angers this frog by attacking or going near it. Causes the frog to start attacking back.
    /// </summary>
    /// <param name="target">The player which angered the frog.</param>
    public void SetTarget(NetworkObject target)
    {
        Assert.IsTrue(IsServer);
        Target = target.gameObject;
        _tongueActive.Value = true;
        _tongueExtending.Value = true;
    }

    /// <summary>
    /// Called every frame. Updates the begin and end points of the tongue when extending and retracting.
    /// </summary>
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
        var newTongueEnd = Vector3.MoveTowards(_tongue.GetPosition(1), target, Time.deltaTime * 250);
        _tongue.SetPosition(1, newTongueEnd);
        _tongueHitbox.transform.position = transform.TransformPoint(newTongueEnd);
    }

    /// <summary>
    /// Called every tick. Handles the animation and movement of the frog.
    /// </summary>
    private void FixedUpdate()
    {
        if (!IsServer) return;
        _animator.SetBool(Walking, Target != null);
        if (Target != null)
        {
            _agent.isStopped = false;
            _agent.SetDestination(Target.transform.position);
        }
        else
        {
            _agent.isStopped = true;
        }
    }

    /// <summary>
    /// Called when a player stays near this frog. If we don't have an active target, make it this player.
    /// </summary>
    /// <param name="other">The player which is near the frog.</param>
    private void OnTriggerStay(Collider other)
    {
        if (!IsServer || Target != null) return;

        if (other.gameObject.GetComponent<AnimalFightingController>() != null)
        {
            SetTarget(other.GetComponent<NetworkObject>());
        }
    }

    /// <summary>
    /// Called when a player leaves this frog's attack radius. If this player was our target, forget about them.
    /// </summary>
    /// <param name="other">The player which left the frog's attack radius.</param>
    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;
        
        if (other.gameObject == Target)
        {
            Target = null;
            _tongueActive.Value = false;
        }
    }
}