using System.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles the logic relating to an individual shuriken fired by a player.
/// </summary>
public class ShurikenController : NetworkBehaviour
{
    private Vector3 _startDirection;
    private Vector3 _startUp;

    internal NetworkVariable<float> Damage = new();

    public float speed = 50f;
    public float rotationSpeed = 100f;

    /// <summary>
    /// Initializes all the required fields when the shuriken spawns.
    /// Also starts the despawn countdown.
    /// </summary>
    private void Start()
    {
        if (IsServer)
        {
            StartCoroutine(DespawnAfterSeconds(5));
        }
        _startDirection = transform.forward;
        _startUp = transform.up;
    }

    /// <summary>
    /// Causes this shuriken to despawn after the specified time.
    /// </summary>
    /// <param name="seconds">The time after which this shuriken should despawn.</param>
    private IEnumerator DespawnAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NetworkObject.Despawn();
    }

    /// <summary>
    /// Called every frame. Moves and rotates the shuriken.
    /// </summary>
    private void Update()
    {
        if (!IsOwner) return;
        
        var localTransform = transform;
        localTransform.position += _startDirection * (speed * Time.deltaTime);
        localTransform.RotateAroundLocal(_startUp, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Called when this shuriken hits something.
    /// If the object it hits is damageable, damage it.
    /// </summary>
    /// <param name="other">The object which the shuriken hit.</param>
    private void OnCollisionEnter(Collision other)
    {
        var networkObject = other.gameObject.GetComponent<NetworkObject>();
        var animal = other.gameObject.GetComponent<IDamageable>();

        if (networkObject != null && animal != null)
        {
            if (animal is AnimalFightingController && networkObject.OwnerClientId == OwnerClientId) return;
            animal.Damage(OwnerClientId, Damage.Value);
        }
        
        DespawnServerRpc();
    }

    /// <summary>
    /// Executed on the server. Causes this shuriken to despawn.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void DespawnServerRpc()
    {
        NetworkObject.Despawn();
    }
}