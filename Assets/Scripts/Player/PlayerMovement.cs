using UnityEngine;
using UnityEngine.InputSystem;

/*[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    // Components
    private Rigidbody2D rb;
    private BoxCollider2D col;

    // Input
    private InputAction moveAction;
    private InputAction jumpAction;

    // Movement
    private Vector2 velocity;
    private Vector2 input;

    // Player stats
    private PlayerStats stats;

    // Ground check
    public bool grounded { get; private set; }
    public LayerMask groundMask;

    // Gravity & jump
    private float jumpVelocity;
    private float gravity;

	private bool m_isMovementLocked = false;
	
	// Sound
	[SerializeField] private AudioClip m_jumpSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        stats = GetComponent<Player>().m_playerStats;

        // Compute jump physics from desired params
        jumpVelocity = (2 * stats.MaxJumpHeight) / (stats.MaxJumpTime / 2f);
        gravity = (-2 * stats.MaxJumpHeight) / Mathf.Pow(stats.MaxJumpTime / 2f, 2);

        // Setup input
        var inputActions = InputSystem.actions;
        if (inputActions == null) Debug.Log("inputActions is null");
        moveAction = inputActions.FindAction("Move");
        jumpAction = inputActions.FindAction("Jump");

        if (moveAction == null) Debug.Log("moveAction is null");
        if (jumpAction == null) Debug.Log("jumpAction is null");
        
     	moveAction.Enable();
        jumpAction.Enable();
    }
	
    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

   	private void Update()
	{
		if (m_isMovementLocked) return;

		input = moveAction.ReadValue<Vector2>();

    	grounded = CheckGrounded();
    	HorizontalMovement();

    	if (grounded)
        	HandleJump();

    	ApplyGravity();
	}

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    // -----------------------------
    // Movement Logic
    // -----------------------------
    private void HorizontalMovement()
    {
        float targetSpeed = input.x * stats.MovementSpeed;
        velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, stats.MovementSpeed * 8f * Time.deltaTime);

        // Flip sprite
		Vector3 newScale = transform.localScale;

		if (velocity.x > 0 && newScale.x < 0)
    	{ 
       	 	newScale.x *= -1;
        	transform.localScale = newScale;
    	}
    
    	else if (velocity.x < 0 && newScale.x > 0) 
    	{	
        	newScale.x *= -1;
        	transform.localScale = newScale;
    	}
    }

    private void HandleJump()
	{
    	// Falls der Spieler gerade nach unten fällt beim Landen:
    	if (velocity.y < 0f)
        	velocity.y = 0f;

    	if (jumpAction.WasPressedThisFrame())
    	{
        	velocity.y = jumpVelocity;

        	ManagersManager.Get<SFXManager>().PlaySFXClip(m_jumpSound, transform, 1f);
    	}
	}


    private void ApplyGravity()
	{
    	if (grounded && velocity.y <= 0f)
    	{
        	velocity.y = 0f;
        	return;
    	}

    	bool falling = velocity.y < 0f;
    	float multiplier = falling ? 2f : 1f;

    	velocity.y += gravity * multiplier * Time.deltaTime;
	}

    // -----------------------------
    // Collision Checks
    // -----------------------------
    private bool CheckGrounded()
    {
        Bounds b = col.bounds;
        Vector2 origin = new Vector2(b.center.x, b.min.y - 0.05f);

        // Nutze Physics2D.queriesHitTriggers = false, um Trigger beim Boden-Check zu ignorieren
        bool oldQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = false;

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            new Vector2(b.size.x * 0.9f, 0.1f),
            0f,
            Vector2.down,
            0.01f,
            groundMask
        );

        Physics2D.queriesHitTriggers = oldQueriesHitTriggers;
        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        if (col == null) return;

        Gizmos.color = grounded ? Color.green : Color.red;
        Bounds b = col.bounds;
        Gizmos.DrawWireCube(new Vector3(b.center.x, b.min.y - 0.05f, 0), new Vector3(b.size.x * 0.9f, 0.1f, 1));
    }

    public void SetMovementLock(bool locked)
    {
        m_isMovementLocked = locked;
        if (locked)
        {
            velocity = Vector2.zero; // Stoppt sofort die aktuelle Trägheit
            input = Vector2.zero;
        }
    }
}*/

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    // Components
    private Rigidbody2D rb;
    private BoxCollider2D col;

    // Input
    private InputAction moveAction;
    private InputAction jumpAction;

    // Movement
    private Vector2 velocity;
    private Vector2 input;

    // Player stats
    private PlayerStats stats;

    // Ground & Wall check
    public bool grounded { get; private set; }
    public bool onWallLeft { get; private set; }
    public bool onWallRight { get; private set; }
    public LayerMask groundMask;
    
    [Header("Wall Jump Layer")]
    [SerializeField] private LayerMask wallJumpMask; // Separater Layer für Walls

    // Gravity & jump
    private float jumpVelocity;
    private float gravity;

    private bool m_isMovementLocked = false;
    
    // Sound
    [SerializeField] private AudioClip m_jumpSound;

    // Wall Jump Settings
    [Header("Wall Jump Settings")]
    private float wallSlideSpeed = 2f;
    private float wallJumpForceX = 6f;
    private float wallJumpForceY = 8f;
    private float wallJumpDuration = 0.1f;
    
    // Jump Buffer & Coyote Time
    [Header("Jump Feel Settings")]
    private float coyoteTime = 0.15f;
    private float jumpBufferTime = 0.1f;
    
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float wallJumpTimer;
    
    // Collision Detection Settings
    [Header("Collision Settings")]
    private float groundCheckDistance = 0.01f;
    private float wallCheckDistance = 0.01f;
    private float skinWidth = 0.001f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        stats = GetComponent<Player>().m_playerStats;

        // Compute jump physics from desired params
        jumpVelocity = (2 * stats.MaxJumpHeight) / (stats.MaxJumpTime / 2f);
        gravity = (-2 * stats.MaxJumpHeight) / Mathf.Pow(stats.MaxJumpTime / 2f, 2);

        // Setup input
        var inputActions = InputSystem.actions;
        if (inputActions == null) Debug.Log("inputActions is null");
        moveAction = inputActions.FindAction("Move");
        jumpAction = inputActions.FindAction("Jump");

        if (moveAction == null) Debug.Log("moveAction is null");
        if (jumpAction == null) Debug.Log("jumpAction is null");
        
        moveAction.Enable();
        jumpAction.Enable();

        // Rigidbody Settings für bessere Physik
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Wenn wallJumpMask nicht gesetzt, nutze groundMask
        if (wallJumpMask == 0)
            wallJumpMask = groundMask;
    }
    
    private void OnEnable()
    {
        // FIX #1: Reset velocity beim Spawnen/Enable
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

        // Collision Checks
        grounded = CheckGrounded();
        onWallLeft = CheckWall(-1);
        onWallRight = CheckWall(1);

        // Coyote Time
        if (grounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // Jump Buffer
        if (jumpAction.WasPressedThisFrame())
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // Wall Jump Timer
        if (wallJumpTimer > 0)
            wallJumpTimer -= Time.deltaTime;

        HandleMovement();
        HandleJump();
        ApplyGravity();

        // Wende Velocity an
        rb.linearVelocity = velocity;
    }

    // -----------------------------
    // Movement Logic
    // -----------------------------
    private void HandleMovement()
    {
        // Wall Jump Override: Spieler kann kurz nicht selbst steuern
        if (wallJumpTimer > 0)
            return;

        float targetSpeed = input.x * stats.MovementSpeed;
        
        // Smoothere Beschleunigung
        float acceleration = grounded ? stats.MovementSpeed * 8f : stats.MovementSpeed * 5f;
        velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, acceleration * Time.deltaTime);

        // Flip sprite
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
        // Normale Sprünge (Ground + Coyote Time)
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            velocity.y = jumpVelocity;
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
            
            ManagersManager.Get<SFXManager>().PlaySFXClip(m_jumpSound, transform, 1f);
        }
        // Wall Jump
        else if (jumpBufferCounter > 0 && (onWallLeft || onWallRight))
        {
            int wallDirection = onWallLeft ? 1 : -1;
            
            // Springe weg von der Wand
            velocity.x = wallDirection * wallJumpForceX;
            velocity.y = wallJumpForceY;
            
            // Setze Timer, damit Spieler kurz nicht selbst steuern kann
            wallJumpTimer = wallJumpDuration;
            jumpBufferCounter = 0;
            
            // Flip sprite in Sprung-Richtung
            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Abs(newScale.x) * wallDirection;
            transform.localScale = newScale;
            
            ManagersManager.Get<SFXManager>().PlaySFXClip(m_jumpSound, transform, 1f);
        }

        // Variable Jump Height (Sprung abbrechen wenn losgelassen)
        if (jumpAction.WasReleasedThisFrame() && velocity.y > 0)
        {
            velocity.y *= 0.5f;
        }
    }

    private void ApplyGravity()
    {
        // Am Boden: keine Gravität
        if (grounded && velocity.y <= 0f)
        {
            velocity.y = 0f;
            return;
        }

        // FIX #3: Wall Slide nur wenn in Richtung der Wand gedrückt wird
        bool onWall = onWallLeft || onWallRight;
        bool pushingIntoWall = (onWallLeft && input.x < -0.1f) || (onWallRight && input.x > 0.1f);
        
        if (onWall && !grounded && velocity.y < 0 && pushingIntoWall)
        {
            // Langsames Rutschen an der Wand
            velocity.y = Mathf.Max(velocity.y, -wallSlideSpeed);
            return;
        }

        // Normale Gravität (stärker beim Fallen)
        bool falling = velocity.y < 0f;
        float multiplier = falling ? 2f : 1f;

        velocity.y += gravity * multiplier * Time.deltaTime;
    }

    // -----------------------------
    // Collision Checks
    // -----------------------------
    private bool CheckGrounded()
    {
        Bounds b = col.bounds;
        
        // Verkleinere die Box horizontal etwas
        float boxWidth = b.size.x - skinWidth * 2;
        Vector2 boxSize = new Vector2(boxWidth, groundCheckDistance);
        Vector2 origin = new Vector2(b.center.x, b.min.y);

        // Nutze Physics2D.queriesHitTriggers = false
        bool oldQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = false;

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            boxSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundMask
        );

        Physics2D.queriesHitTriggers = oldQueriesHitTriggers;
        return hit.collider != null;
    }

    private bool CheckWall(int direction)
    {
        Bounds b = col.bounds;
        
        // Verkleinere die Box vertikal etwas (nicht die obersten/untersten Pixel prüfen)
        float boxHeight = b.size.y - skinWidth * 4;
        Vector2 boxSize = new Vector2(wallCheckDistance, boxHeight);
        
        float xOffset = direction > 0 ? b.max.x : b.min.x;
        Vector2 origin = new Vector2(xOffset, b.center.y);

        bool oldQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = false;

        // FIX #2: Nutze wallJumpMask statt groundMask für Wall Checks
        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            boxSize,
            0f,
            Vector2.right * direction,
            wallCheckDistance,
            wallJumpMask
        );

        Physics2D.queriesHitTriggers = oldQueriesHitTriggers;
        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        if (col == null) return;

        Bounds b = col.bounds;

        // Ground Check Visualisierung
        Gizmos.color = grounded ? Color.green : Color.red;
        float boxWidth = b.size.x - skinWidth * 2;
        Vector2 groundOrigin = new Vector2(b.center.x, b.min.y);
        Gizmos.DrawWireCube(groundOrigin, new Vector3(boxWidth, groundCheckDistance, 1));

        // Wall Check Visualisierung
        Gizmos.color = onWallLeft ? Color.blue : Color.gray;
        float boxHeight = b.size.y - skinWidth * 4;
        Vector2 leftOrigin = new Vector2(b.min.x, b.center.y);
        Gizmos.DrawWireCube(leftOrigin - Vector2.right * wallCheckDistance / 2, 
            new Vector3(wallCheckDistance, boxHeight, 1));

        Gizmos.color = onWallRight ? Color.blue : Color.gray;
        Vector2 rightOrigin = new Vector2(b.max.x, b.center.y);
        Gizmos.DrawWireCube(rightOrigin + Vector2.right * wallCheckDistance / 2, 
            new Vector3(wallCheckDistance, boxHeight, 1));
    }

    public void SetMovementLock(bool locked)
    {
        m_isMovementLocked = locked;
        if (locked)
        {
            velocity = Vector2.zero;
            input = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Neue Methode: Reset für Teleport/Spawn
    public void ResetMovementState()
    {
        Debug.Log("[PlayerMovement] ResetMovementState called!");
        
        velocity = Vector2.zero;
        input = Vector2.zero;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        // Reset alle Timer
        coyoteTimeCounter = 0;
        jumpBufferCounter = 0;
        wallJumpTimer = 0;
        
        // Force Ground Check Update
        grounded = false;
        onWallLeft = false;
        onWallRight = false;
    }
}