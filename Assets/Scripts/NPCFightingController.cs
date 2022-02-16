using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class NPCFightingController : NetworkBehaviour, IDamageable
{
    private AngryNPCController _angryNpcController;
    
    public float health = 30f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _angryNpcController = GetComponentInChildren<AngryNPCController>();
    }

    public void Damage(ulong sourceClientId, float damage)
    {
        if (IsServer) HandleDamage(sourceClientId, damage);
        else HandleDamageServerRpc(sourceClientId, damage);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleDamageServerRpc(ulong sourceClientId, float damage)
    {
        HandleDamage(sourceClientId, damage);
    }

    private void HandleDamage(ulong sourceClientId, float damage)
    {
        Assert.IsTrue(IsServer);
        var source = NetworkManager.ConnectedClients[sourceClientId].PlayerObject;
        _angryNpcController.SetTarget(source);
        
        TakeDamage(damage);
    }

    public void TakeDamage(float amount)
    {
        Assert.IsTrue(IsServer);
        health -= amount;
        if (health <= 0)
        {
            NetworkObject.Despawn();
        }
    }
}
