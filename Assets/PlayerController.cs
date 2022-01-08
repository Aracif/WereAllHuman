using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Shortcut cntrl shift m to bring up available functions
    private Animator animator;
    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;
    private Vector3 m_Velocity = Vector3.zero;
    public LayerMask whatIsGround;
    public LayerMask climbableWall;
    public Transform wallGrabPoint;
    public GameObject floatingPoints;
    public TextMesh playersLastActionHUD;

    [SerializeField] public static bool canWallGrab;
    [SerializeField] public static bool isLedgeGrabbing;
    [SerializeField] public static bool isGrabbing;
    [SerializeField] public static bool jumping = false;
    [SerializeField] public static bool singleJumping = false;
    [SerializeField] public static bool wallJumping = false;
    [SerializeField] public static bool rigidBodyWallJumping = false;
    [SerializeField] public static bool leftGround;

    private bool m_FacingRight = false;

    public float groundDistance = 0.01f;
    public float fallMultiplier = 1.5f;
    private float defaultGravity;
    public float wallJumpCounter;
    public float groundedCounter;
    public float walljumpHangtimeCounter;
    

    [SerializeField] public float wallJumpTime = .2f;
    [SerializeField] public float groundedTime = .02f;
    [SerializeField] public float walljumpHangtime = .2f;
    [SerializeField] public float rawSpeed;
    [SerializeField] public float rigidBodyVelocityY;
    [SerializeField] public float rigidBodyVelocityX;
    [SerializeField] public float raycastHitDistance;
    [SerializeField] public float forwardMomentum;
    [SerializeField] public float jumpForce = 700f;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();    
        rigidbody2D = GetComponent<Rigidbody2D>();    
        boxCollider2D = GetComponent<BoxCollider2D>();
        playersLastActionHUD = GameObject.Find("HUDPlayerAction").GetComponent<TextMesh>();
        defaultGravity = rigidbody2D.gravityScale;
    }

    // Update is called once per fram e
    void Update()
    {
        animatorTesting();

        runInput();

        if (rawSpeed < 0.0 && !m_FacingRight)
        {
            Flip();
        }
        else if (rawSpeed > 0.0 && m_FacingRight)
        {
            Flip();
        }

        wallGrabInput();

        jumpInput();
    }

    private void animatorTesting()
    {
        print(animator.GetCurrentAnimatorStateInfo(0).ToString());
        if (isLedgeGrabbing)
        {
            animator.SetTrigger("ledgeClimb");
            //animator.ResetTrigger("ledgeClimb");
        }
        print(animator.GetCurrentAnimatorStateInfo(0).IsName("Ledge Climb"));
        //GetComponent<LedgeGrab>().changePosition();
    }

    private void FixedUpdate() 
    {
        //GameObject points = Instantiate(floatingPoints, transform.position, Quaternion.identity) as GameObject;
        //floatingPoints.transform.GetChild(0).GetComponent<TextMesh>();
        //collision.gameObject.GetComponent<PlayerActionPopupTextHandler>();
        //transform.position += animator.deltaPosition;
        //transform.rotation = animator.deltaRotation * transform.rotation;
        //invalidWalkRotation();

        if (jumping && groundedCounter <= 0)
        {
            if (landed())
            {
                animator.SetBool("grounded", true);
                animator.SetBool("landedAnimator", true);

                singleJumping = false;
                jumping = false;
                leftGround = false;
                rigidBodyWallJumping = false;
            }
        }
        else
        {
            groundedCounter -= Time.deltaTime;
        }


        if (jumping && !singleJumping)
        {
            Instantiate(floatingPoints, transform.position, Quaternion.identity);
            playersLastActionHUD.text = "single jump";
            forwardMomentum = rawSpeed;
            singleJumping = true;
            rigidbody2D.AddForce(new Vector2(100 * rawSpeed, jumpForce), ForceMode2D.Force);
            if (rawSpeed != 0)
            {
                rigidbody2D.AddRelativeForce(new Vector2(200 * rawSpeed, 0), ForceMode2D.Force);
            }

        }
        else if (singleJumping && !rigidBodyWallJumping)
        {


            if (movingUpward(15))
            {
                leftGround = true;
                animator.SetBool("grounded", false);
            }
            else if (landed() && leftGround || landed() && Input.GetButtonDown("Jump"))
            {
                animator.SetBool("grounded", true);
                singleJumping = false;
                jumping = false;
                leftGround = false;
            }
            else
            {
                if (forwardMomentum != 0)
                {
                    rigidbody2D.AddForce(new Vector2(300 * forwardMomentum, 0), ForceMode2D.Force);
                    //print("Forward Momentum");
                }
            }
            //Allow slight horizontal movement in the air

            
            if (!movingUpward(0))
            {
                //If the player is falling from a jump accelerate fall rate slightly
                rigidbody2D.AddRelativeForce(new Vector2(0, -130), ForceMode2D.Force);
            }
        }

        if (rigidBodyWallJumping)
        {
            if (walljumpHangtimeCounter <= 0)
            {
                rigidbody2D.AddRelativeForce(new Vector2(375 * rawSpeed, 0), ForceMode2D.Force);

            }
            else
            {
                walljumpHangtimeCounter -= Time.deltaTime;

                if (rawSpeed > 0 && facingRight() && rigidbody2D.velocity.x < 0 || rawSpeed < 0 && facingLeft() && rigidbody2D.velocity.x > 0)
                {
                    rigidbody2D.AddRelativeForce(-new Vector2(200 * rawSpeed, 0), ForceMode2D.Force);
                }
                else if (rawSpeed > 0 && facingRight() || rawSpeed < 0 && facingLeft())
                {
                    rigidbody2D.AddRelativeForce(new Vector2(350 * rawSpeed, 0), ForceMode2D.Force);
                }
            }

        }
        else if (singleJumping)
        {
            if (rawSpeed != 0)
            {
                rigidbody2D.AddRelativeForce(new Vector2(250 * rawSpeed, 0), ForceMode2D.Force);
            }
        }

         wallGrabPhysics();


    }

    bool movingUpward(float speed)
    {
        return rigidbody2D.velocity.y > speed;
    }

    void jumpInput()
    {
        if (jumping == false)
        {
            if (Input.GetButtonDown("Jump"))
            {

                RaycastHit2D ray = sendGroundRay();
                if (ray.collider != null) //We're on the ground!
                {
                    jumping = true;
                    animator.SetBool("landedAnimator", false);
                    animator.SetTrigger("jump");
                    groundedCounter = groundedTime;
                }
            }
        }
    }

    void runInput()
    {
        if (animator.GetBool("walkingOnly") && Input.GetAxis("Horizontal") == 0)
        {
            animator.SetFloat("speed", Mathf.Abs(1));
        }
        else
        {
            rawSpeed = Input.GetAxis("Horizontal");
            animator.SetBool("walkingOnly", false);
            animator.SetFloat("speed", Mathf.Abs(rawSpeed));
        }
    }

    void wallGrabInput()
    {
        canWallGrab = Physics2D.OverlapCircle(wallGrabPoint.position, .4f, climbableWall);

        isGrabbing = false;

        if (canWallGrab && singleJumping)
        {
            if ((facingRight() && inputRight()) || (facingLeft() && inputLeft()))
            {
                playersLastActionHUD.text = "climbing";
                isGrabbing = true;
                if (Input.GetButtonDown("Jump") )
                {
                    wallJumping = true;
                }
            }
        }
    }

    void wallGrabPhysics()
    {
        if (wallJumpCounter <= 0)
        {
            if (isGrabbing)
            {
                animator.SetBool("wallGrab", true);
                rigidbody2D.gravityScale = 0;
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.angularVelocity = 0;
                rigidbody2D.isKinematic = true;
                rigidBodyWallJumping = false;

                //TODO             
                //Have to figure this out need to pull this key press out of physics engine!
                if (wallJumping)
                {

                    rigidbody2D.gravityScale = defaultGravity;
                    rigidbody2D.isKinematic = false;

                    //rigidbody2D.velocity = new Vector2(-100, 2);
                    float v = rawSpeed * .7f * .5f;
                    float h = v - rigidbody2D.velocity.x;
                    float horizChange = Mathf.Clamp(h, -.2f, .001f);
                    rigidbody2D.AddForce(new Vector2(-Input.GetAxisRaw("Horizontal") * 2700, 800), ForceMode2D.Force);

                    //print("clamped at " + horizChange);
                    //rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x + horizChange, rigidbody2D.velocity.y);
                    //print("wall jump");

                    wallJumpCounter = wallJumpTime;
                    walljumpHangtimeCounter = walljumpHangtime;
                    isGrabbing = false;
                    animator.SetBool("wallGrab", false);
                    rigidBodyWallJumping = true;
                    wallJumping = false;
                }

            }
            else if (!rigidBodyWallJumping)
            {
                rigidbody2D.gravityScale = defaultGravity;
                animator.SetBool("wallGrab", false);
                rigidbody2D.isKinematic = false;
                rigidBodyWallJumping = false;
                wallJumping = false;
            }
        }
        else
        {
            rigidbody2D.AddForce(new Vector2(-Input.GetAxisRaw("Horizontal") * 100, 50), ForceMode2D.Force);
            wallJumpCounter -= Time.fixedDeltaTime;
        }

    }


    public bool facingLeft()
    {
        return transform.localScale.x == -1f;
    }
    
    public bool facingRight()
    {
        return transform.localScale.x == 1f;
    }

    public bool inputRight()
    {
        return Input.GetAxisRaw("Horizontal") > 0;
    }
       
    public bool inputLeft()
    {
        return Input.GetAxisRaw("Horizontal") < 0;
    }

    public void startedWalking()
    {
        playersLastActionHUD.text = "walking";
        animator.speed = 1;
    }


    public void startedRunning()
    {
        playersLastActionHUD.text = "running";
    }


    bool landed()
    {
        RaycastHit2D ray = sendGroundRay();
        if (ray.collider != null)
        {
            return true;
        }
        return false;
    }


    RaycastHit2D sendGroundRay()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, groundDistance, whatIsGround);
        raycastHitDistance = raycastHit.distance;     
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        //print("Hit: " + raycastHit.collider?.gameObject?.name);
        Debug.DrawRay(boxCollider2D.bounds.center, Vector2.down * (boxCollider2D.bounds.extents.y), rayColor);
        return raycastHit;
    }

    void fasterFall()
    {
        if (rigidbody2D.velocity.y < 0.4)
        {
            rigidbody2D.AddForce(new Vector2(500 * rawSpeed, -160), ForceMode2D.Force);
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //print("ouch");
        //print(collision.collider.name);    
        //print(collision.otherCollider.ToString());
        if (collision.otherCollider.ToString().Contains("CircleCollider") && !isGrabbing)
        {
            //print("ouch");
            animator.SetTrigger("jumpedUpAndHitPlatform");
            animator.WriteDefaultValues();
        }
    }

    void invalidWalkRotation()
    {
        float zRotation = rotationZ();
        if (zRotation!= 0)
        {
            if (zRotation > 15 && zRotation < 345)
            {
                //animator.applyRootMotion = false;
                //animator.WriteDefaultValues();
                //animator.enabled = false;
            }
            else
            {
                 //animator.applyRootMotion = true;
                 //animator.enabled = true;
            }
        }

    }

    float rotationZ()
    {
        //Debug.Log(string.Format("Z rotation is {0}", UnwrapAngle(transform.localEulerAngles.z)));
        return UnwrapAngle(transform.localEulerAngles.z);
    }

    private float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }
}
