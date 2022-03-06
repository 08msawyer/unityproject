using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manages the health and rewards of Frogs.
/// </summary>
public class NPCFightingController : NetworkBehaviour, IDamageable
{
    private AngryNPCController _angryNpcController;

    public float health = 30f;
    public int xpAmount = 50;
    public GameObject mushroomPrefab;

    /// <summary>
    /// Initializes all required fields when this NPC spawns.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _angryNpcController = GetComponentInChildren<AngryNPCController>();
    }

    /// <summary>
    /// Requests that this NPC should be damaged.
    /// </summary>
    /// <param name="sourceClientId">The network ID of the player which caused this damage.</param>
    /// <param name="damage">How much damage should be dealt.</param>
    public void Damage(ulong sourceClientId, float damage)
    {
        if (IsServer) HandleDamage(sourceClientId, damage);
        else HandleDamageServerRpc(sourceClientId, damage);
    }

    /// <summary>
    /// Acts as a bridge method so that damage can be dealt from clients.
    /// </summary>
    /// <param name="sourceClientId">The network ID of the player which caused this damage.</param>
    /// <param name="damage">How much damage was dealt.</param>
    [ServerRpc(RequireOwnership = false)]
    private void HandleDamageServerRpc(ulong sourceClientId, float damage)
    {
        HandleDamage(sourceClientId, damage);
    }

    /// <summary>
    /// Executed on the server. Damages this NPC and, if it does not already have a target, causes it to become angry at the player.
    /// </summary>
    /// <param name="sourceClientId">The network ID of the player which caused this damage.</param>
    /// <param name="damage">How much damage was dealt.</param>
    private void HandleDamage(ulong sourceClientId, float damage)
    {
        Assert.IsTrue(IsServer);
        var source = NetworkManager.ConnectedClients[sourceClientId].PlayerObject;
        _angryNpcController.SetTarget(source);
        
        TakeDamage(damage);
    }

    /// <summary>
    /// Executed on the server. Handles the actual damage logic, and the death logic if the NPC was killed.
    /// There is a 30% chance of a mushroom being dropped when an NPC is killed.
    /// It also grants [xpAmount] XP to the player.
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(float amount)
    {
        Assert.IsTrue(IsServer);
        health -= amount;
        if (health <= 0)
        {
            if (Random.Range(0, 10) <= 3)
            {
                var mushroom = Instantiate(mushroomPrefab);
                mushroom.transform.position = transform.position;
                mushroom.GetComponent<NetworkObject>().Spawn();
            }

            var target = _angryNpcController.Target;
            if (target != null)
            {
                var hud = target.GetComponent<PlayerHudManager>();
                if (hud != null)
                {
                    hud.AddXp(xpAmount);
                }
            }
            
            NetworkObject.Despawn();
        }
    }
}
