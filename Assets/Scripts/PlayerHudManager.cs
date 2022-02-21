using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHudManager : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        ShowScreen("FoxStartScreen", 5f);
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

    private IEnumerator ShowScreenCoroutine(string screenName, float duration)
    {
        var screenObject = GameObject.FindWithTag("HUD").transform.Find("Canvas").Find(screenName).gameObject;
        screenObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        screenObject.SetActive(false);
    }
}