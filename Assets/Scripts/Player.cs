using System;
using System.Linq.Expressions;
using Unity.Profiling;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{

    public Animator animator;
    public float moveSpeed;
    public float jumpForce;
    public float jumpTime;
    public int wallJumpMax;
    public float wallJumpForce;
    public float jumpGraceTime;
    public float wallJumpGraceTime;
    public float floorCheckDistance;
    public LayerMask whatIsFloor;
    public LayerMask whatAreWalls;
    public LayerMask whatIsDeath;
    public BoxCollider2D playerCollision;
    public float fallMultiplier;
    public float wallSlideSpeed;

    private Rigidbody2D rb;
    public float moveInput;
    public bool isGrounded;
    private bool leftCollision;
    private bool rightCollision;
    public bool isOnWall;
    private bool graceTimerActive;
    public bool hasJumped;
    private bool isDead;
    private bool isOnDeadlySurface;
    public float jumpTimeCounter;
    public float jumpGraceTimeCounter;
    public float wallJumpGraceTimeCounter;
    public int remainingWallJumps;
    public float height;
    public float width;
    private float currentJumpForce;
    private bool isWallJumping;
    public float velocity;
    public bool ignoreHorizontal;
    public float ignoreHorizontalTimer;
    public float ignoreHorizontalDuration;
    public bool bufferJump;
    public float jumpDecayTime;
    public float jumpDecayTimer;
    public Vector3 respawnPoint;
    public LayerMask whatIsRespawnPoint;
    public LevelManager gameLevelManager;
    


    //Checks direction player is heading in, and aligns the sprite accordingly
    void AlignCharacterFacing()
    {
        if (moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    #region Jumps
    void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpTimeCounter = jumpTime;
        hasJumped = true;
        jumpDecayTimer = jumpDecayTime;

    }

    void ExtendJump()
    {
        rb.velocity = Vector2.up * currentJumpForce;
        if(jumpDecayTimer <= 0)
        {
            currentJumpForce = (float)(currentJumpForce * 0.995);
            jumpDecayTimer = jumpDecayTime;
        } else
        {
            jumpDecayTimer -= Time.deltaTime;
        }
        
        
    }

    void WallJump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isWallJumping = true;
    }

    #endregion

    void HandleInput()
    {
        GraceTimer();
        if (jumpGraceTimeCounter > 0 && Input.GetKeyDown(KeyCode.Space) && !hasJumped || bufferJump == true && isGrounded) 
        {
            Jump();
            Debug.Log("Jumped");
            bufferJump = false;
        }
        else if (!isGrounded && wallJumpGraceTimeCounter > 0 && remainingWallJumps > 0 && Input.GetKeyDown(KeyCode.Space) && hasJumped && (moveInput >= 1 || moveInput <= -1))
        {
            ignoreHorizontal = false;
            WallJump();
            Debug.Log("ENTERED WALL JUMP");
        }
        else if (!isGrounded && jumpTimeCounter > 0 && Input.GetKey(KeyCode.Space) && hasJumped || isWallJumping && hasJumped && jumpTimeCounter > 0)
        {
            ExtendJump();
        }
        else if(!isGrounded && jumpTimeCounter < 0 && Input.GetKeyDown(KeyCode.Space) && hasJumped)
        {
            bufferJump = true;
        }

        jumpTimeCounter -= Time.deltaTime;
    }

    void GraceTimer()
    {
        if(isGrounded)
        {
            jumpGraceTimeCounter = jumpGraceTime;
            currentJumpForce = jumpForce;
        }
        if (isOnWall)
        {
            wallJumpGraceTimeCounter = wallJumpGraceTime;
        } else
        {
            jumpGraceTimeCounter -= Time.deltaTime;
            wallJumpGraceTimeCounter -= Time.deltaTime;
        }
    }

    void CheckOverlaps()
    {

        isGrounded = Physics2D.Raycast(this.transform.position, Vector2.down, floorCheckDistance, whatIsFloor);

        isOnWall = (Physics2D.Raycast(this.transform.position, Vector2.left, floorCheckDistance, whatAreWalls) ||
                Physics2D.Raycast(this.transform.position, Vector2.right, floorCheckDistance, whatAreWalls));


        isOnDeadlySurface = (Physics2D.Raycast(this.transform.position, Vector2.up, floorCheckDistance, whatIsDeath) ||
                Physics2D.Raycast(this.transform.position, Vector2.down, floorCheckDistance, whatIsDeath) ||
                Physics2D.Raycast(this.transform.position, Vector2.left, floorCheckDistance, whatIsDeath) ||
                Physics2D.Raycast(this.transform.position, Vector2.right, floorCheckDistance, whatIsDeath));



        if (isGrounded & !Input.GetKey(KeyCode.Space))
        {
            hasJumped = false;
        } 
    }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollision = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        width = playerCollision.size.x;
        height = playerCollision.size.y;
        ignoreHorizontal = false;
        respawnPoint = transform.position;
        gameLevelManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {

        AlignCharacterFacing();
        CheckOverlaps();
        GraceTimer();
        HandleInput();

        if (isOnDeadlySurface)
        {
            gameLevelManager.Respawn();
        }

        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        if (isGrounded)
        {
            remainingWallJumps = wallJumpMax;
            isWallJumping = false;
            ignoreHorizontal = false;
        }

        if(isOnWall && isWallJumping == true && Input.GetKey(KeyCode.LeftShift))
        {
            ignoreHorizontal = false;
        }
        else if (isOnWall && Input.GetKey(KeyCode.LeftShift))
         {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
            ignoreHorizontal = true; 
         }
        velocity = rb.velocity.y;

    }

    void FixedUpdate()
    {

        moveInput = Input.GetAxisRaw("Horizontal");
        if (!ignoreHorizontal)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            Debug.Log("Respawn hit, respawn point updated to " + other.transform.position);
            respawnPoint = other.transform.position;
            Debug.Log(respawnPoint);
        }
    }


}
