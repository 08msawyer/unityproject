using Unity.Netcode;
using UnityEngine;

public class AnimalMovementController : NetworkBehaviour
{
    private static readonly int Walking = Animator.StringToHash("Walking");
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int Landing = Animator.StringToHash("Landing");

    private NetworkVariable<Vector3> _position;
    private Vector3 _playerVelocity;
    private float _bottomBound;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private bool _jumping;
    
    public float playerSpeed = 3.6f;
    public float jumpHeight = 5f;
    public new Camera camera;
    public float landingMultiplier = 3f;

    private void Start()
    {
        _bottomBound = GetComponent<Collider>().bounds.extents.y;
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (IsOwner)
        {
            _position.Value = transform.position;
        }
        else
        {
            transform.position = _position.Value;
        }
        
        if (Input.GetButtonDown("Jump"))
        {
            _jumping = true;
        }
    }

    private void FixedUpdate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        var velocity = _rigidbody.velocity;
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var groundedPlayer = IsGrounded();

        if (groundedPlayer && _jumping)
        {
            velocity.y += jumpHeight;
            _animator.SetTrigger(Jumping);
        }
        _jumping = false;

        
        if (horizontal != 0 || vertical != 0)
        {
            var cameraTransform = camera.transform;
            var forward = cameraTransform.forward;
            var right = cameraTransform.right;
            
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            
            var desiredMoveDirection = (forward * vertical + right * horizontal).normalized;
            transform.forward = desiredMoveDirection;

            var desiredVelocity = desiredMoveDirection * playerSpeed;
            desiredVelocity.y = velocity.y;
            
            velocity = desiredVelocity;
            
            _animator.SetBool(Walking, true);
        }
        else
        {
            _animator.SetBool(Walking, false);
        }

        _rigidbody.velocity = velocity;
    }

    private bool IsGrounded()
    {
        var nearGround = Physics.Raycast(transform.position, Vector3.down, out var hit, _bottomBound * landingMultiplier);
        var grounded = nearGround && hit.distance <= _bottomBound + 0.0001;
        _animator.SetBool(Landing, nearGround);
        
        return grounded;
    }
}