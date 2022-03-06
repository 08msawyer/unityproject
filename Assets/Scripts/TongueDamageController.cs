using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles the logic relating to frog tongue collisions.
/// </summary>
public class TongueDamageController : NetworkBehaviour
{
    public float damage = 5f;

    /// <summary>
    /// Called when the end of the tongue hits something. If the other object is a player, damage them.
    /// </summary>
    /// <param name="other">The object which the tongue hit.</param>
    private void OnTriggerEnter(Collider other)
    {
        var animal = other.gameObject.GetComponent<AnimalFightingController>();
        if (animal == null || !IsServer) return;
        animal.DealDamage(damage);
    }
}