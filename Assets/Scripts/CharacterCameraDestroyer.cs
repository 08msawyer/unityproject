using Cinemachine;
using Unity.Netcode;

/// <summary>
/// This ensures each client's scene contains only their own camera, and not those of other players.
/// </summary>
public class CharacterCameraDestroyer : NetworkBehaviour
{
    /// <summary>
    /// Destroys this camera if it is not owned by the current client.
    /// </summary>
    private void Start()
    {
        if (!IsOwner)
        {
            Destroy(GetComponent<CinemachineFreeLook>());
        }
    }
}
