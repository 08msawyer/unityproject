using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class AnimalFightingController : NetworkBehaviour
{
    private static readonly int Attacking = Animator.StringToHash("Attacking");
    private static readonly int Damaged = Animator.StringToHash("Damaged");
    
    private Animator _animator;
    private float _forwardBound;
    private readonly CountdownManager _countdownManager = new();

    internal AnimalFightingController CurrentTarget;
    
    public float dodgeTime = 0.5f;
    public float dodgeCooldown = 5f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _forwardBound = GetComponent<Collider>().bounds.extents.z;
    }

    private void Update()
    {
        if (!IsOwner) return;
        _countdownManager.ElapseTime(Time.deltaTime);
        
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    private void Attack()
    {
        _animator.SetTrigger(Attacking);
        
        if (CurrentTarget != null)
        {
            CurrentTarget.Damage();
        }
    }

    private void Damage()
    {
        _animator.SetTrigger(Damaged);
    }
}