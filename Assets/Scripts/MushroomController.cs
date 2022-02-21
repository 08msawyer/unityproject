using Unity.Netcode;
using UnityEngine;

public class MushroomController : NetworkBehaviour
{
    public float healingAmount = 15f;
    
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
