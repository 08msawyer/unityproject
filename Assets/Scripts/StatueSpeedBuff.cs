using Unity.Netcode;
using UnityEngine;

public class StatueSpeedBuff : NetworkBehaviour
{
    private bool _activated;

    // If the statue collides with something it will be given a speed boost
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        var animal = other.gameObject.GetComponent<AnimalMovementController>();
        if (animal != null)
        {
            if (_activated) return;
            _activated = true;
            MakeFoxSpeedyClientRpc(new ClientRpcSendParams
            {
                TargetClientIds = new[] {animal.OwnerClientId}
            });
            other.gameObject.GetComponent<PlayerHudManager>().ShowScreen("FoxStatueScreen", 5f);
        }
    }

    [ClientRpc]
    private void MakeFoxSpeedyClientRpc(ClientRpcSendParams parameters)
    {
        NetworkManager.LocalClient.PlayerObject.GetComponent<AnimalMovementController>().playerSpeed *= 1.5f;
    }
}