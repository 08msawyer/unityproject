using Unity.Netcode;
using UnityEngine;

public class TongueDamageController : NetworkBehaviour
{
    public float damage = 5f;

    private void OnTriggerEnter(Collider other)
    {
        var animal = other.gameObject.GetComponent<AnimalFightingController>();
        if (animal == null || !IsServer) return;
        animal.DealDamage(damage);
    }
}