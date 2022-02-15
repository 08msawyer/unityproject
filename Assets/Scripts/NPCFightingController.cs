using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NPCFightingController : NetworkBehaviour, IDamageable
{
    public void Damage(float damage)
    {
        NetworkObject.Despawn();
    }
}
