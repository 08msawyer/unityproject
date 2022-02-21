using System;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class AnimalFightingController : NetworkBehaviour, IDamageable
{
    private static readonly int Attacking = Animator.StringToHash("Attacking");
    private static readonly int Damaged = Animator.StringToHash("Damaged");
    private static readonly int Death = Animator.StringToHash("Death");

    private readonly CountdownManager _countdownManager = new();
    private readonly NetworkVariable<float> _health = new();
    private ClientRpcParams _sendToOwner;

    private AnimalAnimationController _animator;
    private AnimalMovementController _movementController;
    private Camera _camera;
    private Slider _healthBar;

    public float maxHealth = 100f;
    public float damage = 10f;
    public GameObject shuriken;

    private void Start()
    {
        _animator = GetComponent<AnimalAnimationController>();
        _movementController = GetComponent<AnimalMovementController>();
        _camera = Camera.main;
        _healthBar = GameObject.FindWithTag("HUD").GetComponentInChildren<Slider>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        _health.Value = maxHealth;
        _sendToOwner = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] {OwnerClientId}
            }
        };
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
            _movementController.LookForwardRelativeToCamera();
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

    public void Damage(ulong sourceClientId, float damage)
    {
        DamageServerRpc(damage);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DamageServerRpc(float damageDealt)
    {
        DealDamage(damageDealt);
    }

    public void DealDamage(float damageDealt)
    {
        _animator.SetTrigger(Damaged);
        _health.Value -= damageDealt;
        if (_health.Value <= 0)
        {
            _animator.SetTrigger(Death);
            StartCoroutine(DieCoroutine());
        }

        HealthUpdateClientRpc(_health.Value, _sendToOwner);
    }

    private IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (NetworkManager.ConnectedClients.Count(it => it.Value.PlayerObject != null) == 2)
        {
            // Only one player left
            var lastPlayer = NetworkManager.ConnectedClients.Values.First().PlayerObject;
            lastPlayer.GetComponent<PlayerHudManager>().ShowScreen("FoxWinScreen", float.MaxValue);
        }

        NetworkObject.Despawn();
    }

    public void Heal(float amount)
    {
        Assert.IsTrue(IsServer);
        _health.Value = Math.Max(_health.Value + amount, maxHealth);
        HealthUpdateClientRpc(_health.Value, _sendToOwner);
    }

    [ClientRpc]
    private void HealthUpdateClientRpc(float newHealth, ClientRpcParams clientRpcParams = default)
    {
        _healthBar.value = newHealth / maxHealth;
        if (newHealth <= 0)
        {
            // gameObject.GetComponent<PlayerHudManager>().ShowScreen();
        }
    }
}