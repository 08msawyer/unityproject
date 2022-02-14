using Unity.Netcode;
using UnityEngine;

public class AnimalFightingController : NetworkBehaviour
{
    private static readonly int Attacking = Animator.StringToHash("Attacking");
    private static readonly int Damaged = Animator.StringToHash("Damaged");
    private static readonly int Death = Animator.StringToHash("Death");

    private readonly CountdownManager _countdownManager = new();

    private AnimalAnimationController _animator;
    private Camera _camera;

    public float health = 100f;
    public float damage = 10f;
    public GameObject shuriken;

    private void Start()
    {
        _animator = GetComponent<AnimalAnimationController>();
        _camera = Camera.main;
    }

    private void Update()
    {
        if (IsServer)
        {
            _countdownManager.ElapseTime(Time.deltaTime);
        }

        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0))
        {
            AttackServerRpc(_camera.transform.position, _camera.transform.rotation);
        }
    }

    [ServerRpc]
    private void AttackServerRpc(Vector3 pos, Quaternion rot)
    {
        _animator.SetTrigger(Attacking);
        var shuriken = Instantiate(this.shuriken, pos + rot * Vector3.forward * 2, rot);
        shuriken.GetComponent<ShurikenController>().Damage.Value = damage;
        var networkObject = shuriken.GetComponent<NetworkObject>();
        networkObject.SpawnWithOwnership(OwnerClientId);
    }

    [ServerRpc]
    public void DamageServerRpc(float damageDealt)
    {
        _animator.SetTrigger(Damaged);
        health -= damageDealt;
        if (health <= 0)
        {
            _animator.SetTrigger(Death);
        }
    }
}