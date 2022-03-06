using System;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// A wrapper around Unity's [Animator] which allows the server to set parameters and syncs them to the clients.
/// </summary>
public class AnimalAnimationController : NetworkBehaviour
{
    private Animator _animator;

    /// <summary>
    /// Gets the backing [Animator] component.
    /// </summary>
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Sets a boolean parameter.
    /// </summary>
    /// <param name="id">The ID of the parameter to set.</param>
    /// <param name="value">The new value for the parameter.</param>
    public void SetBool(int id, bool value)
    {
        RequestAnimatorSetBoolServerRpc(id, value);
    }

    /// <summary>
    /// Sets a float parameter.
    /// </summary>
    /// <param name="id">The ID of the parameter to set.</param>
    /// <param name="value">The new value for the parameter.</param>
    public void SetFloat(int id, float value)
    {
        RequestAnimatorSetFloatServerRpc(id, value);
    }
    
    /// <summary>
    /// Sets a trigger parameter.
    /// </summary>
    /// <param name="id">The ID of the parameter to set.</param>
    public void SetTrigger(int id)
    {
        if (IsServer) HandleAnimatorSetTriggerClientRpc(id);
        else RequestAnimatorSetTriggerServerRpc(id);
    }
    
    /// <summary>
    /// Executed on the server. Actually sets the requested boolean parameter.
    /// </summary>
    /// <param name="id">The ID of the parameter to set.</param>
    /// <param name="value">The new value for the parameter.</param>
    [ServerRpc]
    private void RequestAnimatorSetBoolServerRpc(int id, bool value)
    {
        _animator.SetBool(id, value);
    }
    
    /// <summary>
    /// Executed on the server. Actually sets the requested float parameter.
    /// </summary>
    /// <param name="id">The ID of the parameter to set.</param>
    /// <param name="value">The new value for the parameter.</param>
    [ServerRpc]
    private void RequestAnimatorSetFloatServerRpc(int id, float value)
    {
        _animator.SetFloat(id, value);
    }
    
    /// <summary>
    /// Executed on the server. Causes all clients to set this trigger parameter.
    /// </summary>
    /// <param name="id">The ID of the parameter to set.</param>
    [ServerRpc(RequireOwnership = false)]
    private void RequestAnimatorSetTriggerServerRpc(int id)
    {
        HandleAnimatorSetTriggerClientRpc(id);
    }
    
    /// <summary>
    /// Executed on the client. Causes this client to set this trigger parameter.
    /// </summary>
    /// <param name="id">The ID of the parameter to set.</param>
    [ClientRpc]
    private void HandleAnimatorSetTriggerClientRpc(int id)
    {
        _animator.SetTrigger(id);
    }
}