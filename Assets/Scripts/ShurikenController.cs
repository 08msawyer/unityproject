using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ShurikenController : NetworkBehaviour
{
    private Vector3 _startDirection;
    private Vector3 _startUp;

    internal NetworkVariable<float> Damage = new();

    public float speed = 50f;
    public float rotationSpeed = 100f;

    private void Start()
    {
        if (IsServer)
        {
            StartCoroutine(DespawnAfterSeconds(5));
        }
        _startDirection = transform.forward;
        _startUp = transform.up;
    }

    private IEnumerator DespawnAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NetworkObject.Despawn();
    }

    private void Update()
    {
        if (!IsOwner) return;
        
        var localTransform = transform;
        localTransform.position += _startDirection * (speed * Time.deltaTime);
        localTransform.RotateAroundLocal(_startUp, rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        var networkObject = other.gameObject.GetComponent<NetworkObject>();
        var animal = other.gameObject.GetComponent<IDamageable>();

        if (networkObject != null && animal != null)
        {
            if (animal is AnimalFightingController && networkObject.OwnerClientId == OwnerClientId) return;
            animal.Damage(Damage.Value);
        }
        
        DespawnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnServerRpc()
    {
        NetworkObject.Despawn();
    }
}