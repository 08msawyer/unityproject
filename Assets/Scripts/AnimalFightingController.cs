using Unity.Netcode;
using UnityEngine;

public class AnimalFightingController : NetworkBehaviour
{
    private static readonly int Attacking = Animator.StringToHash("Attacking");
    private static readonly int Damaged = Animator.StringToHash("Damaged");
    private static readonly int Death = Animator.StringToHash("Death");

    private readonly CountdownManager _countdownManager = new();

    private AnimalAnimationController _animator;
    private Rigidbody _rigidbody;
    private Vector3 _originalScale;
    private Camera _camera;
    private bool _canDodge = true;
    private bool _dodging = false;
    private float _timeSinceDodge;

    internal AnimalFightingController CurrentTarget;

    public float dodgeTime = 0.2f;
    public float dodgeCooldown = 5f;
    public float health = 100f;
    public float damage = 10f;
    public GameObject shuriken;

    private void Start()
    {
        _animator = GetComponent<AnimalAnimationController>();
        _rigidbody = GetComponent<Rigidbody>();
        _originalScale = transform.localScale;
        _camera = Camera.main;
    }

    private void Update()
    {
        if (IsServer)
        {
            _countdownManager.ElapseTime(Time.deltaTime);
        }

        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0))
        {
            AttackServerRpc(_camera.transform.position, _camera.transform.rotation);
        }

        // if (Input.GetKeyDown(KeyCode.LeftShift))
        // {
        //     TryDodgeServerRpc();
        // }
        //
        // ApplyDodgeScaleServerRpc();
    }

    [ServerRpc]
    private void AttackServerRpc(Vector3 pos, Quaternion rot)
    {
        _animator.SetTrigger(Attacking);

        var shuriken = Instantiate(this.shuriken, pos + rot * Vector3.forward * 2, rot);
        shuriken.GetComponent<ShurikenController>().Owner = this;
        shuriken.GetComponent<NetworkObject>().Spawn();

        // if (CurrentTarget != null)
        // {
        //     CurrentTarget.DamageServerRpc(damage);
        // }
    }

    [ServerRpc]
    private void TryDodgeServerRpc()
    {
        if (!_canDodge) return;

        _dodging = true;
        _canDodge = false;
        _timeSinceDodge = 0f;
        _countdownManager.AddCountdown(new Countdown(dodgeTime, () =>
        {
            _dodging = false;
            _countdownManager.AddCountdown(new Countdown(dodgeCooldown, () =>
            {
                _canDodge = true;
                transform.localScale = _originalScale;
            }));
        }));
    }

    [ServerRpc]
    private void ApplyDodgeScaleServerRpc()
    {
        if (!_dodging) return;
        
        _timeSinceDodge += Time.deltaTime;
        transform.localScale = _originalScale * (Mathf.Cos(_timeSinceDodge / dodgeTime * 2 * Mathf.PI) + 1) / 2;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DamageServerRpc(float damageDealt)
    {
        _animator.SetTrigger(Damaged);
        health -= damageDealt;
        if (health <= 0)
        {
            _animator.SetTrigger(Death);
        }
    }
}