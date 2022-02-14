using Unity.Netcode;
using UnityEngine;

public class ZoneController : NetworkBehaviour
{
    private void Update()
    {
        if (IsServer)
        {
            transform.localScale *= 0.99999f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;
        
        var animal = other.gameObject.GetComponent<AnimalFightingController>();
        if (animal != null)
        {
            animal.DealDamage(50);
        }
    }
}
