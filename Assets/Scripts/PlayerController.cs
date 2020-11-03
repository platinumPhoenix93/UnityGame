using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    #region Variables
    public Animator animator;

    private Rigidbody2D rb;
    public float speed;
    public float jumpForce;
    private float moveInput;
    public int numberOfWallJumps;
    private int wallJumpsRemaining;
    private bool isOnGround;
    private bool isOnWall;
    private bool hasWallJumped;
    public Transform feetPos;
    public Transform headPos;
    public Transform leftPos;
    public Transform rightPos;
    public float checkRadius;
    public LayerMask whatIsGround;
    public LayerMask whatIsDeath;
    public LayerMask whatAreWalls;
    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;
    public float jumpGraceTime;
    private float jumpGraceTimeCounter;
    public float dashSpeed;
    public float wallJumpRad;
    public float wallJumpPushForce;
    private bool jumpGraceTimerStarted;
    public BoxCollider2D trigger;

    #endregion

    #region Jumping

    private void GraceTimer()
    {
        if (!isOnGround && !jumpGraceTimerStarted)
        {
            jumpGraceTimeCounter = jumpGraceTime;
            jumpGraceTimerStarted = true;
        }
        else
        {
            jumpGraceTimeCounter -= Time.deltaTime;
        }
        
    }





    private void Jump()
    {
        if (isOnWall  && wallJumpsRemaining > 0)
        {
            rb.velocity = Vector2.up * jumpForce;
            wallJumpsRemaining--;
            hasWallJumped = true;
        }

        if (isOnGround || jumpGraceTimeCounter > 0)
        {
            rb.velocity = Vector2.up * jumpForce;
        }
    }

    #endregion


    void OnTriggerEnter2D (Collider2D other)
    {
 
    }

    void OnTriggerExit2D(Collider2D other)
    {
    }

    void Start()

    {
        rb = GetComponent<Rigidbody2D>();

    }

    private void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

 /*       if (moveInput != 0)
        {
            animator.SetBool("isMoving", true);
        } else
        {
            animator.SetBool("isMoving", false);
        }*/

    }

    //Checks if the player is overlapping with any deadly surfaces
    bool SurfaceIsDeadly()
    {
        return Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsDeath);
    }
    
    // Update is called once per frame
    void Update()
    {
        isOnGround = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        isOnWall = Physics2D.OverlapCircle(leftPos.position, wallJumpRad, whatAreWalls) || Physics2D.OverlapCircle(rightPos.position, wallJumpRad, whatAreWalls);

        if(moveInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        } else if(moveInput < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (isOnGround)
        {
            wallJumpsRemaining = numberOfWallJumps;
        }

        if (isOnGround == true && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = Vector2.up * jumpForce;
            isJumping = true;
            jumpTimeCounter = jumpTime;
        }

        if (isOnWall && Input.GetKeyDown(KeyCode.Space) && wallJumpsRemaining > 0)
        {
            rb.velocity = Vector2.up * jumpForce;
            isJumping = true;
            jumpTimeCounter = jumpTime;
            wallJumpsRemaining--;
            wallJumpsRemaining = 100;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping == true){
            if(jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            } else
            {
                isJumping = false;
            }
            
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }
}
