using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles the healing logic of mushrooms.
/// </summary>
public class MushroomController : NetworkBehaviour
{
    public float healingAmount = 15f;
    
    /// <summary>
    /// Called when another object touches this mushroom.
    /// If the other object is a player, we will heal them by [healingAmount]
    /// </summary>
    /// <param name="other">The object which touched this mushroom.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        var animal = other.GetComponent<AnimalFightingController>();
        if (animal != null)
        {
            animal.Heal(healingAmount);
            NetworkObject.Despawn();
        }
    }
}
