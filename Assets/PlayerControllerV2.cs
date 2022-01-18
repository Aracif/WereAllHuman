
using UnityEngine;

public class PlayerControllerV2 : MonoBehaviour
{
    public TextMesh playersLastActionHUD;
    public TextMesh fallDistanceHUD;
    public TextMesh nearTextHUD;
    public TextMesh velocityHUD;

    public Collider2D nearSomething;

    private Animator animator;
    private Rigidbody2D rigidbody2D;
    private CapsuleCollider2D boxCollider2D;
    private CircleCollider2D headCollider;
    public LayerMask whatIsGround;
    public LayerMask whatIsCeiling;
    public LayerMask everything;

    public Vector2 fallPoint;
    public float fallDistance;

    private bool m_FacingRight = false;
    public bool isKinematic = false;
    public bool grounded = true;
    public bool hitHead = false;
    public bool wasGrounded = true;
    public bool playerGroundRayHit = true;
    public bool doubleJump = false;
    public float groundedCounter;
    public float groundedCheckWaitTime = 0.2f;
    public float rawSpeed;

    [SerializeField] public float raycastHitDistance;


    //Ledge Grab
    private bool greenBox, redBox;
    public float redXOffset = 0.8f, redYOffset = 2f, redXSize = 0.25f, redYSize = 0.15f, greenXOffset = 0.8f, greenYOffset = 1.3f, greenXSize = 0.2f, greenYSize = 0.15f;
    private float startingGrav;
    public bool isGrabbingLedge;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponentInParent<Rigidbody2D>();
        boxCollider2D = GetComponentInParent<CapsuleCollider2D>();
        headCollider = transform.Find("Root Bone").GetChild(0).GetChild(0).GetComponent<CircleCollider2D>();
        playersLastActionHUD = GameObject.Find("HUDPlayerAction").GetComponent<TextMesh>();
        nearTextHUD = GameObject.Find("NearText").GetComponent<TextMesh>();
        fallDistanceHUD = GameObject.Find("FallDistance").GetComponent<TextMesh>();
        velocityHUD = GameObject.Find("VelocityInfo").GetComponent<TextMesh>();

        startingGrav = rigidbody2D.gravityScale;
    }

    private void FixedUpdate()
    {
        velocityHUD.text = string.Format("Velocity: x{0} y{1}", rigidbody2D.velocity.x, rigidbody2D.velocity.y);

        //Logic related to falling
        if (isFalling())
        {
            if (fallPoint.y == 0)
            {
                fallPoint = new Vector2(0,rigidbody2D.transform.position.y);
            }
            fallDistance = Vector2.Distance(new Vector2(0,rigidbody2D.transform.position.y), fallPoint);
            fallDistanceHUD.text = "Fall Distance: " + fallDistance;
            animator.SetBool("falling", true);
            rigidbody2D.isKinematic = false;
            rigidbody2D.AddRelativeForce(new Vector2(0, -30), ForceMode2D.Force);
        }
        else
        {
            animator.SetBool("falling", false);
            fallPoint = new Vector2(0, 0);
        }


        if (rawSpeed != 0 && !grounded && !rigidbody2D.isKinematic)
        {
            rigidbody2D.AddRelativeForce(new Vector2(20 * rawSpeed, 0), ForceMode2D.Force);
        }

        if (rawSpeed != 0 && !grounded && rigidbody2D.isKinematic && !wasGrounded && !hitHead && nearSomething==null)
        {
            rigidbody2D.velocity = new Vector2(9, 0) * rawSpeed;
        }
        else if (nearSomething != null)
        {
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
        }




    }
    // Update is called once per frame
    void Update()
    {
        velocityHUD.text = string.Format("Velocity: x{0} y{1}", rigidbody2D.velocity.x, rigidbody2D.velocity.y);

        //Respawn player for testing purposes
        respawn();

        //Are we close to another gameobject?
        nearSomething = nearAGameobject();
        if (nearSomething != null)
        {
            int numColliders = 10;
            Collider2D[] colliders = new Collider2D[numColliders];
            ContactFilter2D contactFilter = new ContactFilter2D();
            nearSomething.OverlapCollider(contactFilter, colliders); //Other colliders that overlap the collision
            nearTextHUD.text = "Near: " + nearSomething.name;
        }


        //Check if you hit something while moving upwards
        bool hitHead = this.hitHead;
        Collider2D col = sendHeadRay().collider;
        this.hitHead = col != null;
        if (this.hitHead)
        {
             if (col.sharedMaterial != null && !col.sharedMaterial.name.Equals("wall")) {
                rigidbody2D.isKinematic = false;
            }
        }

        //Check if youre grounded 
        wasGrounded = grounded;
        grounded = Physics2D.OverlapCircle(boxCollider2D.gameObject.transform.position, .1f, whatIsGround);

        if (!wasGrounded && grounded )
        {
            playersLastActionHUD.text = "Grounded";
            rigidbody2D.isKinematic = false;
            animator.SetBool("jump", false);
            animator.SetBool("grounded", true);
            doubleJump = false;

        }
        else if (!grounded)
        {
            playersLastActionHUD.text = "In Air";
            animator.SetBool("grounded", false);
        }

        //Locomotion
        rawSpeed = Input.GetAxis("Horizontal");
        animator.SetFloat("speed", Mathf.Abs(rawSpeed));

        if (rawSpeed < 0.0 && !m_FacingRight)
        {
            Flip();
        }
        else if (rawSpeed > 0.0 && m_FacingRight)
        {
            Flip();
        }

        jumpInput();
        ledgeGrab();
    }

    //Used as animation event for standard single jump
    public void setKinematic(string val)
    {
        isKinematic = bool.Parse(val);
        rigidbody2D.isKinematic = isKinematic;
        //animator.SetBool("falling", true);
        //doubleJump = false;
    }

    void ledgeGrab()
    {
        greenBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXSize, greenYSize), 0f, whatIsGround);
        redBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYOffset), new Vector2(redXSize, redYSize), 0f, whatIsGround);

        if (greenBox && !redBox && !isGrabbingLedge)
        {
            isGrabbingLedge = true;
            print("grabbedeeem");
        }
        else
        {
            isGrabbingLedge = false;
        }
    }

    void jumpInput() 
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            rigidbody2D.isKinematic = true;
            animator.SetBool("jump", true);
            animator.SetTrigger("jumpTrigger");
        }
        else if (Input.GetButtonDown("Jump") && animator.GetBool("jump") && !doubleJump){
            animator.SetBool("falling", false);
            animator.SetTrigger("doubleJump");
            doubleJump = true;
            rigidbody2D.velocity = new Vector2(9, 0) * rawSpeed;
            if (!rigidbody2D.isKinematic)
            {
                rigidbody2D.isKinematic = true;
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYOffset), new Vector2(redXSize, redYSize));

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXSize, greenYSize));

    }

    //Recall that when this method is implemented you will receive "Root motion handled by script"
    //in unity IDE.
    void OnAnimatorMove()
    {
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            transform.parent.position += animator.deltaPosition;
            transform.parent.rotation = animator.deltaRotation;
        }
    }

    bool isFalling()
    {
        return rigidbody2D.velocity.y < -1f;
    }

    RaycastHit2D sendGroundRay()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, .3f, whatIsGround);
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
        Debug.DrawRay(boxCollider2D.bounds.center, Vector2.down * (boxCollider2D.bounds.extents.y), rayColor);
        return raycastHit;
    }    
    
    RaycastHit2D sendHeadRay()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(headCollider.transform.position + (Vector3.up * 1.1f), Vector3.up, .6f, whatIsCeiling);
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
        Debug.DrawRay(headCollider.bounds.center, Vector2.up * (headCollider.bounds.extents.y), rayColor);
        return raycastHit;
    }

    Collider2D nearAGameobject()
    {
        return Physics2D.OverlapCircle(boxCollider2D.transform.position, .8f, everything);
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void respawn()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.parent.transform.position = new Vector3(50, 0, 0);
            rigidbody2D.isKinematic = false;
            animator.SetBool("jump", false);
            animator.SetBool("grounded", true);
        }
    }
}
