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

    	grounded = CheckGrounded();      // 1. Ground before anything else
    	HorizontalMovement();            // 2. Calc velocity.x

    	if (grounded)
        	HandleJump();                // 3. Jump sets velocity.y

    	ApplyGravity();                  // 4. Only applies when NOT grounded
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
        //if (velocity.x > 0) transform.eulerAngles = Vector3.zero;
        //if (velocity.x < 0) transform.eulerAngles = new Vector3(0, 180, 0);
		Vector3 newScale = transform.localScale;

		if (velocity.x > 0 && newScale.x < 0)
    	{ 
       	 	newScale.x *= -1; // Skalierung von negativ auf positiv setzen (z.B. von -1 auf +1)
        	transform.localScale = newScale;
    	}
    
    	// 2. Bewegung nach LINKS (velocity.x < 0)
    	// Flip nur, wenn der Spieler momentan nach rechts schaut (d.h. Skalierung ist positiv)
    	else if (velocity.x < 0 && newScale.x > 0) 
    	{	
        	newScale.x *= -1; // Skalierung von positiv auf negativ setzen (z.B. von +1 auf -1)
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
        	velocity.y = 0f;     // WICHTIG: NICHT gravity überschreiben
        	return;              // Gravity NICHT anwenden
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
}