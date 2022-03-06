using System;
using Unity.Netcode;
using UnityEngine;

public class AnimalAnimationController : NetworkBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetBool(int id, bool value)
    {
        RequestAnimatorSetBoolServerRpc(id, value);
    }

    public void SetFloat(int id, float value)
    {
        RequestAnimatorSetFloatServerRpc(id, value);
    }
    
    public void SetTrigger(int id)
    {
        if (IsServer) HandleAnimatorSetTriggerClientRpc(id);
        else RequestAnimatorSetTriggerServerRpc(id);
    }
    
    [ServerRpc]
    private void RequestAnimatorSetBoolServerRpc(int id, bool value)
    {
        _animator.SetBool(id, value);
    }
    
    [ServerRpc]
    private void RequestAnimatorSetFloatServerRpc(int id, float value)
    {
        _animator.SetFloat(id, value);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RequestAnimatorSetTriggerServerRpc(int id)
    {
        HandleAnimatorSetTriggerClientRpc(id);
    }
    
    [ClientRpc]
    private void HandleAnimatorSetTriggerClientRpc(int id)
    {
        _animator.SetTrigger(id);
    }
}