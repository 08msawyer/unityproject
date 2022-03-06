using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHudManager : NetworkBehaviour
{
    private TextMeshProUGUI _xpText;
    private TextMeshProUGUI _messageText;
    private NetworkVariable<int> _xp = new();

    internal int Xp => _xp.Value;

    private void Start()
    {
        var xpObject = GameObject.FindWithTag("HUD").transform.Find("Canvas").Find("XP").gameObject;
        _xpText = xpObject.GetComponent<TextMeshProUGUI>();
        var messageObject = GameObject.FindWithTag("HUD").transform.Find("Canvas").Find("Message").gameObject;
        _messageText = messageObject.GetComponent<TextMeshProUGUI>();
    }

    public override void OnNetworkSpawn()
    {
        ShowScreen("FoxStartScreen", 5f);
    }

    private void FixedUpdate()
    {
        if (!IsClient) return;
        _xpText.text = $"XP: {Xp}";
    }

    public void AddXp(int amount)
    {
        _xp.Value += amount;
    }

    public void ShowMessage(string message, float duration)
    {
        ShowMessageClientRpc(message, duration, new ClientRpcParams
        {
            Send = new ClientRpcSendParams {
                TargetClientIds = new[] {OwnerClientId}
            }
        });
    }

    [ClientRpc]
    private void ShowMessageClientRpc(string message, float duration, ClientRpcParams clientRpcParams = default)
    {
        StartCoroutine(ShowMessageCoroutine(message, duration));
    }

    public void ShowScreen(string screenName, float duration)
    {
        ShowScreenClientRpc(screenName, duration, new ClientRpcParams
        {
            Send = new ClientRpcSendParams {
            TargetClientIds = new[] {OwnerClientId}
            }
        });
    }

    [ClientRpc]
    private void ShowScreenClientRpc(string screenName, float duration, ClientRpcParams clientRpcParams = default)
    {
        StartCoroutine(ShowScreenCoroutine(screenName, duration));
    }

    private IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        _messageText.text = message;
        yield return new WaitForSeconds(duration);
        _messageText.text = string.Empty;
    }

    private IEnumerator ShowScreenCoroutine(string screenName, float duration)
    {
        var screenObject = GameObject.FindWithTag("HUD").transform.Find("Canvas").Find(screenName).gameObject;
        screenObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        screenObject.SetActive(false);
    }
}