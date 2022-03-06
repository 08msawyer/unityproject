using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles the HUD logic for each player, as well as their XP, as this is closely linked to the HUD.
/// </summary>
public class PlayerHudManager : NetworkBehaviour
{
    private TextMeshProUGUI _xpText;
    private TextMeshProUGUI _messageText;
    private NetworkVariable<int> _xp = new();

    internal int Xp => _xp.Value;

    /// <summary>
    /// Sets up all the required fields when the object is initialized.
    /// </summary>
    private void Start()
    {
        var xpObject = GameObject.FindWithTag("HUD").transform.Find("Canvas").Find("XP").gameObject;
        _xpText = xpObject.GetComponent<TextMeshProUGUI>();
        var messageObject = GameObject.FindWithTag("HUD").transform.Find("Canvas").Find("Message").gameObject;
        _messageText = messageObject.GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Shows the start screen when this player is spawned.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        ShowScreen("FoxStartScreen", 5f);
    }

    /// <summary>
    /// Constantly updates the XP text to reflect the player's current XP.
    /// </summary>
    private void FixedUpdate()
    {
        if (!IsClient) return;
        _xpText.text = $"XP: {Xp}";
    }

    /// <summary>
    /// Adds XP to this player.
    /// </summary>
    /// <param name="amount">The amount of XP to add.</param>
    public void AddXp(int amount)
    {
        _xp.Value += amount;
    }

    /// <summary>
    /// Shows a message to this player.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="duration">The duration for which the message should be shown.</param>
    public void ShowMessage(string message, float duration)
    {
        ShowMessageClientRpc(message, duration, new ClientRpcParams
        {
            Send = new ClientRpcSendParams {
                TargetClientIds = new[] {OwnerClientId}
            }
        });
    }

    /// <summary>
    /// Causes the specified client to display a message.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    /// <param name="duration">The duration for which the message should be displayed.</param>
    /// <param name="clientRpcParams">Contains information about which client should be shown the message.</param>
    [ClientRpc]
    private void ShowMessageClientRpc(string message, float duration, ClientRpcParams clientRpcParams = default)
    {
        StartCoroutine(ShowMessageCoroutine(message, duration));
    }

    /// <summary>
    /// Shows an image screen to this player.
    /// </summary>
    /// <param name="screenName">The name of the screen to show.</param>
    /// <param name="duration">The duration for which to show the screen.</param>
    public void ShowScreen(string screenName, float duration)
    {
        ShowScreenClientRpc(screenName, duration, new ClientRpcParams
        {
            Send = new ClientRpcSendParams {
            TargetClientIds = new[] {OwnerClientId}
            }
        });
    }

    /// <summary>
    /// Causes the specified client to display a screen.
    /// </summary>
    /// <param name="screenName">The name of the screen to be displayed.</param>
    /// <param name="duration">The duration for which the screen should be displayed.</param>
    /// <param name="clientRpcParams">Contains information about which client should be shown the screen.</param>
    [ClientRpc]
    private void ShowScreenClientRpc(string screenName, float duration, ClientRpcParams clientRpcParams = default)
    {
        StartCoroutine(ShowScreenCoroutine(screenName, duration));
    }

    /// <summary>
    /// Executed on the client. Shows a message and then hides it after the specified time.
    /// </summary>
    /// <param name="message">The message to be shown.</param>
    /// <param name="duration">The duration after which the message should be hidden.</param>
    private IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        _messageText.text = message;
        yield return new WaitForSeconds(duration);
        _messageText.text = string.Empty;
    }

    /// <summary>
    /// Executed on the client. Shows a screen and then hides it after the specified time.
    /// </summary>
    /// <param name="screenName">The name of the screen to be shown.</param>
    /// <param name="duration">The duration after which the screen should be hidden.</param>
    private IEnumerator ShowScreenCoroutine(string screenName, float duration)
    {
        var screenObject = GameObject.FindWithTag("HUD").transform.Find("Canvas").Find(screenName).gameObject;
        screenObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        screenObject.SetActive(false);
    }
}