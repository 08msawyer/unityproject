using Cinemachine;
using Unity.Netcode;

public class CharacterCameraDestroyer : NetworkBehaviour
{
    private void Start()
    {
        if (!IsOwner)
        {
            Destroy(GetComponent<CinemachineFreeLook>());
        }
    }
}
