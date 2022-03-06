using System;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Handles all of the logic relating to players' health and attacks.
/// </summary>
public class AnimalFightingController : NetworkBehaviour, IDamageable
{
    private static readonly int Attacking = Animator.StringToHash("Attacking");
    private static readonly int Damaged = Animator.StringToHash("Damaged");
    private static readonly int Death = Animator.StringToHash("Death");
    
    private readonly NetworkVariable<float> _health = new();
    private ClientRpcParams _sendToOwner;

    private AnimalAnimationController _animator;
    private AnimalMovementController _movementController;
    private Camera _camera;
    private Slider _healthBar;

    public float maxHealth = 100f;
    public float damage = 10f;
    public GameObject shuriken;

    /// <summary>
    /// Initializes all the required fields when the object spawns.
    /// </summary>
    private void Start()
    {
        _animator = GetComponent<AnimalAnimationController>();
        _movementController = GetComponent<AnimalMovementController>();
        _camera = Camera.main;
        _healthBar = GameObject.FindWithTag("HUD").GetComponentInChildren<Slider>();
    }

    /// <summary>
    /// Sets up the player's health and remembers the local client's ID for later network requests.
    /// </summary>
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

    /// <summary>
    /// Called every frame. Fires a shuriken if the player left clicks.
    /// </summary>
    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0))
        {
            _movementController.LookForwardRelativeToCamera();
            AttackServerRpc(_camera.transform.position, _camera.transform.rotation);
        }
    }

    /// <summary>
    /// Executed on the server. Spawns a shuriken at the desired location with the desired rotation.
    /// </summary>
    /// <param name="pos">The location at which to spawn the shuriken.</param>
    /// <param name="rot">The rotation with which to spawn the shuriken.</param>
    [ServerRpc]
    private void AttackServerRpc(Vector3 pos, Quaternion rot)
    {
        _animator.SetTrigger(Attacking);
        var shuriken = Instantiate(this.shuriken, pos + rot * Vector3.forward * 2, rot);
        shuriken.GetComponent<ShurikenController>().Damage.Value = damage;
        var networkObject = shuriken.GetComponent<NetworkObject>();
        networkObject.SpawnWithOwnership(OwnerClientId);
    }

    /// <summary>
    /// Requests that this player should be damaged.
    /// </summary>
    /// <param name="sourceClientId">The network ID of the player which caused this damage.</param>
    /// <param name="damage">How much damage was dealt.</param>
    public void Damage(ulong sourceClientId, float damage)
    {
        DamageServerRpc(damage);
    }
    
    /// <summary>
    /// Acts as a bridge method to allow damage to be dealt from clients.
    /// </summary>
    /// <param name="damageDealt">How much damage was dealt.</param>

    [ServerRpc(RequireOwnership = false)]
    private void DamageServerRpc(float damageDealt)
    {
        DealDamage(damageDealt);
    }

    /// <summary>
    /// Handles the actual damage logic and animations, and if the player was killed, the death logic and animations.
    /// </summary>
    /// <param name="damageDealt">How much damage was dealt.</param>
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

    /// <summary>
    /// Causes the player to die after 0.5 seconds. Also triggers the win screen for the other player if they are the only one left.
    /// </summary>
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

    /// <summary>
    /// Heals this player by [amount] up to a maximum of [maxHealth].
    /// </summary>
    /// <param name="amount">The amount by which to heal the player.</param>
    public void Heal(float amount)
    {
        Assert.IsTrue(IsServer);
        _health.Value = Math.Min(_health.Value + amount, maxHealth);
        HealthUpdateClientRpc(_health.Value, _sendToOwner);
    }

    /// <summary>
    /// Executed on the client. Synchronizes the health bar with their current health, and may in the future show a death screen.
    /// </summary>
    /// <param name="newHealth">The new health value for this player.</param>
    /// <param name="clientRpcParams">Contains information about which client has had its health changed.</param>
    [ClientRpc]
    private void HealthUpdateClientRpc(float newHealth, ClientRpcParams clientRpcParams = default)
    {
        _healthBar.value = newHealth / maxHealth;
        if (newHealth <= 0)
        {
            // TODO: possible death screen?
        }
    }
}