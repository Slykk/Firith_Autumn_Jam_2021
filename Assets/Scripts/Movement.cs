using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Animator animator;
    public Camera mainCamera;
    private Rigidbody2D myRigidbody2D;
    private Collisions collision;              //referencing the script for the collisions

    [Header("Player common Attributes")]
    [SerializeField] float speed = 20f;     //Regular moving Speed
    [SerializeField] float horizontalMove = 0f;         
    [SerializeField] float verticalMove = 0f;
    [SerializeField] float maxVelocity;
    [SerializeField] float slidingSpeed;                 //speed with which character slides off along a wall
    [SerializeField] float dashForce;                   //Speed used in dashing
    [SerializeField] float myWallJumpForce = 50;        // Amount of force added when the player jumps off a wall.


    private Vector3 velocity = Vector3.zero;
    [Range(1, 15)] [SerializeField] float jumpVelocity = 10;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;

    [Header("Player Bools & States")]

    [SerializeField] bool canMove;
    private bool jumping;
    private bool dashing;
    private bool jump;
    private bool dash;
    [SerializeField] bool canDash;
    private bool isOnWall;
    [SerializeField] bool isGrabbingWall;
    [SerializeField] bool isWallSliding;
    [SerializeField] bool walljumped;
    private bool canDoubleJump;

    [SerializeField] bool myAirControl = true;                         // Whether the player can move whilst jumping;

    [Header("World dependancies")]
    public bool facingRight = true;  // Bool for determining which direction the player is currently facing.

    public float xRaw;
    public float yRaw;

    private float fallSpeedLimiter = 25f; // To limit the fall speed
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collision = GetComponent<Collisions>();
        mainCamera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        if (canMove)
        {   horizontalMove = Input.GetAxis("Horizontal") * speed;
            verticalMove   = Input.GetAxis("Vertical");
            xRaw = Input.GetAxisRaw("Horizontal");
            yRaw = Input.GetAxisRaw("Vertical");
            Vector2 direction = new Vector2(horizontalMove, verticalMove);
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));    // Used to trigger the running animation
        }

        isGrabbingWall = isOnWall && Input.GetKey(KeyCode.LeftShift);

              if (collision.isOnWall && Input.GetButton("Fire1") && canMove || collision.isOnWall && Input.GetKey(KeyCode.JoystickButton7) && canMove)
              {
                  isGrabbingWall = true;
                  isWallSliding = false;
              }

              if (Input.GetButtonUp("Fire3") || !collision.isOnWall || !canMove)
              {
                  isGrabbingWall = false;
                  isWallSliding = false;
              }

              if (collision.isOnGround && !dashing)
              {
                  walljumped = false;
              }


        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton1)) // I used using joystick buttons aswell because personally i like testing with a controller

        { jump = true;
          maxVelocity = 0; }



        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.JoystickButton2))

        {  dash = true; }

        if (myRigidbody2D.velocity.y < 0)

        { 
            myRigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;   

        }


        else if (myRigidbody2D.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            myRigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;

        }



        if (isGrabbingWall && !dashing)
        {
            myRigidbody2D.gravityScale = 1;
            float speedModifier = verticalMove > 0 ? .5f : 1;
            myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, verticalMove * (speed * speedModifier));
        }

        else
        {
            myRigidbody2D.gravityScale = 1;
        }

        if (!collision.isOnGround && isOnWall)
        {
            if (horizontalMove != 0 && !isGrabbingWall)
            {
                isWallSliding = true;
                WallSliding();
            }
        }

        if (!collision.isOnWall || collision.isOnGround)
            isWallSliding = false;


        if (isGrabbingWall || isWallSliding || !canMove)
            return;
    }

    private void FixedUpdate()
    {
        Move(horizontalMove * Time.fixedDeltaTime, jump, dash); 

        //The actual move fucntion, I ws going to have every interaction go through a function and maybe even put in a different script
        //so a controller as a script and movement itself in another but havent done that so its still messy

        jump = false;
        dash = false;

        if (isGrabbingWall)
        {
            myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, verticalMove * speed);
        }

    }


    public void Move(float move, bool jump, bool dash)
    {
        if (canMove)
        {
            if (collision.isOnGround || myAirControl)
            {
                if (myRigidbody2D.velocity.y < -fallSpeedLimiter)
                    myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, -fallSpeedLimiter);
                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(move * 10f, myRigidbody2D.velocity.y);
                // And then smoothing it out and applying it to the character
                myRigidbody2D.velocity = Vector3.SmoothDamp(myRigidbody2D.velocity, targetVelocity, ref velocity, movementSmoothing);

                if (move > 0 && !facingRight)
                {
                    // to flip the player sprite according to the direction its facing.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && facingRight)
                {
                    // flip it, flipt it, flip it real good (did you read this?) - just checking slykk rofl
                    Flip();
                }
            }

            if (canDash && dash && !collision.isOnGround) //testing dashing only in midAir
            {
                if (xRaw != 0 || yRaw != 0)
                    Dash(xRaw);
            }

            if (collision.isOnGround && jump)
            {
                collision.isOnGround = false;
                myRigidbody2D.velocity = Vector2.up * jumpVelocity;
                canDoubleJump = true;
            }

            else if (collision.isOnWall && !collision.isOnGround && jump) 
            { 
                WallJump();
            } 


            else if (!collision.isOnGround && jump && canDoubleJump)
            {
                canDoubleJump = false;
                myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, 0);
                myRigidbody2D.velocity = Vector2.up * jumpVelocity;
            }
         }

    }

    public void Flip()
    {   
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;     
    }

    public void WallSliding()
    {
        myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, -slidingSpeed);
    }

    private void Dash(float x)
    {
        mainCamera.GetComponent<CameraController>().ShakeCamera();  //not great, just experimenting with cam Shake

        myRigidbody2D.velocity = new Vector2(transform.localScale.x * dashForce, 0);

        StartCoroutine(DashCooling());
    }

    IEnumerator DashCooling()
    {
        myRigidbody2D.gravityScale = 0;
        walljumped = true;
        dashing = true;
        canDash = false;

        yield return new WaitForSeconds(1f);

        myRigidbody2D.gravityScale = 1;
        walljumped = false;
        dashing = false;
        canDash = true;
    }

        
    private void WallJump()
    { // unsure how to limit wallJump if it ends up being a feature, as it stands you can keep walljumping into the air nonstop in the same place
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = collision.isOnWall? Vector2.left : Vector2.right;

        myRigidbody2D.velocity = new Vector2(myRigidbody2D.velocity.x, 0);
        myRigidbody2D.velocity += (Vector2.up / 1.5f + wallDir / 1.5f) * myWallJumpForce;

        walljumped = true;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
}




