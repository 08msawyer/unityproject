using Unity.Netcode;
using UnityEngine;

public class StatueSpeedBuff : NetworkBehaviour
{
    private static readonly int RunSpeedMultiplier = Animator.StringToHash("RunSpeedMultiplier");
    
    private bool _activated;

    public int xpThreshold = 200;
    public float speedMultiplier = 1.5f;

    // If the statue collides with something it will be given a speed boost
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        var animal = other.gameObject.GetComponent<AnimalMovementController>();
        var hud = other.gameObject.GetComponent<PlayerHudManager>();
        if (animal != null)
        {
            if (_activated)
            {
                hud.ShowMessage($"The statue has already been used!", 3f);
                return;
            }

            if (hud.Xp < xpThreshold)
            {
                hud.ShowMessage($"You need {xpThreshold}XP to use the statue!", 3f);
                return;
            }

            _activated = true;
            MakeFoxSpeedyClientRpc(new ClientRpcSendParams
            {
                TargetClientIds = new[] {animal.OwnerClientId}
            });
            hud.ShowScreen("FoxStatueScreen", 5f);
        }
    }

    [ClientRpc]
    private void MakeFoxSpeedyClientRpc(ClientRpcSendParams parameters)
    {
        var localPlayer = NetworkManager.LocalClient.PlayerObject;
        localPlayer.GetComponent<AnimalMovementController>().playerSpeed *= speedMultiplier;
        localPlayer.GetComponent<AnimalAnimationController>().SetFloat(RunSpeedMultiplier, speedMultiplier);
    }
}