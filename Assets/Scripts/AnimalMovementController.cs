using System;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Handles most of the logic relating to players' movement and animations.
/// </summary>
public class AnimalMovementController : NetworkBehaviour
{
    private static readonly int Walking = Animator.StringToHash("Walking");
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int Landing = Animator.StringToHash("Landing");

    private Camera _camera;
    private Vector3 _playerVelocity;
    private float _bottomBound;
    private Rigidbody _rigidbody;
    private AnimalAnimationController _animator;
    private bool _jumping;

    public float playerSpeed = 3.6f;
    public float jumpHeight = 5f;
    public float landingMultiplier = 3f;

    /// <summary>
    /// Initializes all the required fields when this player spawns.
    /// </summary>
    private void Start()
    {
        _camera = Camera.main;
        _bottomBound = GetComponent<Collider>().bounds.extents.y;
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<AnimalAnimationController>();
        // SpawnAtRandomPosition();
    }

    /// <summary>
    /// Moves this player to a random point on the map.
    /// </summary>
    /// <exception cref="Exception">If no spawn could be found.</exception>
    private void SpawnAtRandomPosition()
    {
        var worldBounds = GameObject.FindWithTag("World").GetComponentInChildren<Collider>().bounds;
        var x = Random.Range(worldBounds.min.x, worldBounds.max.x);
        var z = Random.Range(worldBounds.min.z, worldBounds.max.z);
        var position = new Vector3(x, worldBounds.max.y, z);
        
        var raycastResult = Physics.Raycast(position, Vector3.down, out var hit);
        if (!raycastResult)
        {
            throw new Exception($"Could not find a spawn for {this}!");
        }
        
        transform.position = hit.point + _bottomBound * Vector3.up;
    }

    /// <summary>
    /// Called every frame. Registers that a jump input was pressed.
    /// </summary>
    private void Update()
    {
        if (IsOwner && Input.GetButtonDown("Jump"))
        {
            _jumping = true;
        }
    }

    /// <summary>
    /// Called every tick. Handles the actual movement logic based on the player's inputs.
    /// </summary>
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        
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
            var cameraTransform = _camera.transform;
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

    /// <summary>
    /// Causes this player to look forward relative to its camera.
    /// </summary>
    public void LookForwardRelativeToCamera()
    {
        var cameraTransform = _camera.transform;
        var forward = cameraTransform.forward;

        forward.y = 0f;
        forward.Normalize();

        transform.forward = forward;
    }

    /// <summary>
    /// Checks if this player is on the ground and can therefore jump.
    /// </summary>
    /// <returns>Whether the player is on the ground.</returns>
    private bool IsGrounded()
    {
        var nearGround = Physics.Raycast(transform.position, Vector3.down, out var hit, _bottomBound * landingMultiplier);
        var grounded = nearGround && hit.distance <= _bottomBound + 0.0001;
        
        if (nearGround && _rigidbody.velocity.y < -0.001f)
        {
            _animator.SetTrigger(Landing);
        }

        return grounded;
    }
}