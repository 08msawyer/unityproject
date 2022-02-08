using System;
using System.Linq;
using UnityEngine;

public class AnimalFightingController : MonoBehaviour
{
    private static readonly int Attacking = Animator.StringToHash("Attacking");
    private static readonly int Damaged = Animator.StringToHash("Damaged");
    
    private Animator _animator;
    private float _forwardBound;
    private CountdownManager _countdownManager;

    internal AnimalFightingController CurrentTarget;

    public bool respondToInput;
    public float dodgeTime = 0.5f;
    public float dodgeCooldown = 5f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _forwardBound = GetComponent<Collider>().bounds.extents.z;
    }

    private void Update()
    {
        if (!respondToInput) return;
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