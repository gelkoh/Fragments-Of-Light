using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Animator m_animator;

    private InputAction moveAction;
    private InputAction jumpAction;

    private Vector2 velocity;
    private Vector2 input;

    private PlayerStats stats;

    public bool grounded { get; private set; }
    public bool onWallLeft { get; private set; }
    public bool onWallRight { get; private set; }
    public bool hitCeiling { get; private set; }
    
    [SerializeField] private LayerMask solidObjectMask;

    private float jumpVelocity;
    private float gravity;

    private bool m_isMovementLocked = false;
    
    [SerializeField] private AudioClip m_jumpSound;

    private float wallSlideSpeed = 2f;
    private float wallJumpForceX = 6f;
    private float wallJumpForceY = 8f;
    private float wallJumpDuration = 0.1f;
    
    private float coyoteTime = 0.15f;
    private float jumpBufferTime = 0.1f;
    
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float wallJumpTimer;
    
    private float groundCheckDistance = 0.01f;
    private float wallCheckDistance = 0.01f;
    private float skinWidth = 0.001f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        m_animator = GetComponent<Animator>();
    }

    void Start()
    {
        stats = GetComponent<Player>().m_playerStats;

        jumpVelocity = (2 * stats.MaxJumpHeight) / (stats.MaxJumpTime / 2f);
        gravity = (-2 * stats.MaxJumpHeight) / Mathf.Pow(stats.MaxJumpTime / 2f, 2);

        var inputActions = InputSystem.actions;
        if (inputActions == null) Debug.Log("inputActions is null");
        moveAction = inputActions.FindAction("Move");
        jumpAction = inputActions.FindAction("Jump");

        if (moveAction == null) Debug.Log("moveAction is null");
        if (jumpAction == null) Debug.Log("jumpAction is null");
        
        moveAction.Enable();
        jumpAction.Enable();

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    
    private void OnEnable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            velocity = Vector2.zero;
        }
    }
    
    private void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (jumpAction != null) jumpAction.Disable();
    }

    private void Update()
    {
        if (m_isMovementLocked) return;

        input = moveAction.ReadValue<Vector2>();

        grounded = CheckGrounded();
        hitCeiling = CheckCeiling();
        onWallLeft = CheckWall(-1);
        onWallRight = CheckWall(1);

        if (grounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        if (jumpAction.WasPressedThisFrame())
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        if (wallJumpTimer > 0)
            wallJumpTimer -= Time.deltaTime;

        HandleMovement();
        HandleJump();
        ApplyGravity();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = velocity;
    }

    private void HandleMovement()
    {
        if (wallJumpTimer > 0)
            return;

        float targetSpeed = input.x * stats.MovementSpeed;
        
        float acceleration = grounded ? stats.MovementSpeed * 8f : stats.MovementSpeed * 5f;
        velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, acceleration * Time.deltaTime);

        Vector3 newScale = transform.localScale;

        if (velocity.x > 0.1f && newScale.x < 0)
        { 
            newScale.x *= -1;
            transform.localScale = newScale;
        }
        else if (velocity.x < -0.1f && newScale.x > 0) 
        {   
            newScale.x *= -1;
            transform.localScale = newScale;
        }
    }

    private void HandleJump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            velocity.y = jumpVelocity;
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
            
            ManagersManager.Get<SFXManager>().PlaySFXClip(m_jumpSound, transform, 1f);
        }
        else if (jumpBufferCounter > 0 && (onWallLeft || onWallRight))
        {
            int wallDirection = onWallLeft ? 1 : -1;
            
            velocity.x = wallDirection * wallJumpForceX;
            velocity.y = wallJumpForceY;
            
            wallJumpTimer = wallJumpDuration;
            jumpBufferCounter = 0;
            
            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Abs(newScale.x) * wallDirection;
            transform.localScale = newScale;
            
            ManagersManager.Get<SFXManager>().PlaySFXClip(m_jumpSound, transform, 1f);
        }

        if (jumpAction.WasReleasedThisFrame() && velocity.y > 0)
        {
            velocity.y *= 0.5f;
        }
    }

    private void ApplyGravity()
    {
        if (grounded && velocity.y <= 0f)
        {
            velocity.y = 0f;
            return;
        }

        if (hitCeiling && velocity.y > 0f)
        {
            velocity.y = 0f;
        }

        bool onWall = onWallLeft || onWallRight;
        bool pushingIntoWall = (onWallLeft && input.x < -0.1f) || (onWallRight && input.x > 0.1f);
        
        if (onWall && !grounded && velocity.y < 0 && pushingIntoWall)
        {
            velocity.y = Mathf.Max(velocity.y, -wallSlideSpeed);
            return;
        }

        bool falling = velocity.y < 0f;
        float multiplier = falling ? 2f : 1f;

        velocity.y += gravity * multiplier * Time.deltaTime;
    }

    private void UpdateAnimations()
    {
        if (m_animator == null) return;

        bool isMovingInput = Mathf.Abs(input.x) > 0.01f;
        bool isWalking = grounded && isMovingInput;
        
        m_animator.SetBool("isWalking", isWalking);
        m_animator.SetBool("isJumping", !grounded);
    }

	private void StopAnimations()
	{
 		if (m_animator == null) return;

        m_animator.SetBool("isWalking", false);
        m_animator.SetBool("isJumping", false);
	}
	
    private bool CheckGrounded()
    {
        Bounds b = col.bounds;
        
        float boxWidth = b.size.x - skinWidth * 2;
        Vector2 boxSize = new Vector2(boxWidth, groundCheckDistance);
        Vector2 origin = new Vector2(b.center.x, b.min.y);

        bool oldQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = false;

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            boxSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            solidObjectMask
        );

        Physics2D.queriesHitTriggers = oldQueriesHitTriggers;
        return hit.collider != null;
    }

    private bool CheckCeiling()
    {
        Bounds b = col.bounds;
        
        float boxWidth = b.size.x - skinWidth * 2;
        Vector2 boxSize = new Vector2(boxWidth, groundCheckDistance);
        Vector2 origin = new Vector2(b.center.x, b.max.y);

        bool oldQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = false;

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            boxSize,
            0f,
            Vector2.up,
            groundCheckDistance,
            solidObjectMask
        );

        Physics2D.queriesHitTriggers = oldQueriesHitTriggers;
        return hit.collider != null;
    }

    private bool CheckWall(int direction)
    {
        Bounds b = col.bounds;
        
        float boxHeight = b.size.y - skinWidth * 4;
        Vector2 boxSize = new Vector2(wallCheckDistance, boxHeight);
        
        float xOffset = direction > 0 ? b.max.x : b.min.x;
        Vector2 origin = new Vector2(xOffset, b.center.y);

        bool oldQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = false;

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            boxSize,
            0f,
            Vector2.right * direction,
            wallCheckDistance,
            solidObjectMask
        );

        Physics2D.queriesHitTriggers = oldQueriesHitTriggers;
        return hit.collider != null;
    }

    public void SetMovementLock(bool locked)
    {
        m_isMovementLocked = locked;

        if (locked)
        {
            velocity = Vector2.zero;
            input = Vector2.zero;
            rb.linearVelocity = Vector2.zero;

			StopAnimations();
        }
    }

    public void ResetMovementState()
    {
        velocity = Vector2.zero;
        input = Vector2.zero;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        coyoteTimeCounter = 0;
        jumpBufferCounter = 0;
        wallJumpTimer = 0;
        
        grounded = false;
        hitCeiling = false;
        onWallLeft = false;
        onWallRight = false;
    }
}