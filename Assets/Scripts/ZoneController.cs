using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles the logic relating to the shrinking zone around the map.
/// </summary>
public class ZoneController : NetworkBehaviour
{
    /// <summary>
    /// Called every frame. Shrinks the zone by a small amount.
    /// </summary>
    private void Update()
    {
        if (IsServer)
        {
            transform.localScale *= 0.99999f;
        }
    }

    /// <summary>
    /// Called when an object leaves the zone. If the object was a player or NPC, kill them.
    /// </summary>
    /// <param name="other">The object which left the zone.</param>
    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;
        
        var animal = other.gameObject.GetComponent<AnimalFightingController>();
        if (animal != null)
        {
            animal.DealDamage(10000);
            return;
        }

        var npc = other.gameObject.GetComponent<NPCFightingController>();
        if (npc != null)
        {
            npc.TakeDamage(10000);
        }
    }
}
