using UnityEngine;

public class AnimalController : MonoBehaviour
{
    private static readonly int Walking = Animator.StringToHash("Walking");
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int Landing = Animator.StringToHash("Landing");
    
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
            // transform.rotation = Quaternion.LookRotation(forward);
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

        
        // if (transform.position.y > _maxHeight)
        // {
        //     Debug.Log(transform.position.y);
        //     _maxHeight = transform.position.y;
        // }
    }

    private bool IsGrounded()
    {
        var nearGround = Physics.Raycast(transform.position, Vector3.down, out var hit, _bottomBound * landingMultiplier);
        var grounded = nearGround && hit.distance <= _bottomBound + 0.0001;
        _animator.SetBool(Landing, nearGround);
        // if (!grounded)
        // {
        //     var animationState = _animator.GetCurrentAnimatorStateInfo(0);
        //     var a = 4.905f;
        //     var b = -_rigidbody.velocity.y;
        //     var c = -hit.distance + _bottomBound;
        //     var t = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
        //     var timeLeft = animationState.length / _animator.GetFloat(JumpAnimationSpeed) *
        //                    (animationState.normalizedTime % 1);
        //
        //     var speed = timeLeft / (float) t;
        //     if (speed > 0)
        //     {
        //         _animator.SetFloat(JumpAnimationSpeed, _jumpAnimationSpeed = speed);
        //     }
        // }
        // else
        // {
        //     _animator.SetFloat(JumpAnimationSpeed, _jumpAnimationSpeed = 1f);
        // }

        return grounded;
    }

    // private float _maxHeight;
}