using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles all the logic relating to the fox statue at the center of the map.
/// </summary>
public class StatueSpeedBuff : NetworkBehaviour
{
    private static readonly int RunSpeedMultiplier = Animator.StringToHash("RunSpeedMultiplier");
    
    private bool _activated;

    public int xpThreshold = 200;
    public float speedMultiplier = 1.5f;

    /// <summary>
    /// Called whenever something goes near the statue.
    /// If the other object is a player, they have sufficient XP, and the statue hasn't been used already, give them a speed boost.
    /// </summary>
    /// <param name="other">The object which touched the statue.</param>
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

    /// <summary>
    /// Executed on the client. Increases this player's speed and makes their run animation faster.
    /// </summary>
    /// <param name="parameters">Contains information about which client should have their player sped up.</param>
    [ClientRpc]
    private void MakeFoxSpeedyClientRpc(ClientRpcSendParams parameters)
    {
        var localPlayer = NetworkManager.LocalClient.PlayerObject;
        localPlayer.GetComponent<AnimalMovementController>().playerSpeed *= speedMultiplier;
        localPlayer.GetComponent<AnimalAnimationController>().SetFloat(RunSpeedMultiplier, speedMultiplier);
    }
}