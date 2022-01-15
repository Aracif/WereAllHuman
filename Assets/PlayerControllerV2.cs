
using UnityEngine;

public class PlayerControllerV2 : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rigidbody2D;
    //private BoxCollider2D boxCollider2D;
    private CapsuleCollider2D boxCollider2D;
    private CircleCollider2D headCollider;
    public LayerMask whatIsGround;
    public LayerMask whatIsCeiling;

    private bool m_FacingRight = false;
    public bool isKinematic = false;
    public bool grounded = true;
    public bool hitHead = false;
    public bool playerGroundRayHit = true;
    public float groundedCounter;
    public float groundedCheckWaitTime = 0.2f;
    public float rawSpeed;

    [SerializeField] public float raycastHitDistance;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponentInParent<Rigidbody2D>();
        boxCollider2D = GetComponentInParent<CapsuleCollider2D>();
        headCollider = transform.Find("Root Bone").GetChild(0).GetChild(0).GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        if (isFalling())
        {
            animator.SetBool("falling", true);
            rigidbody2D.AddRelativeForce(new Vector2(0, -30), ForceMode2D.Force);
            //rigidbody2D.rotation += 5.0f;

        }
        else
        {
            animator.SetBool("falling", false);
        }


        if (rawSpeed != 0 && !grounded && !rigidbody2D.isKinematic)
        {
            rigidbody2D.AddRelativeForce(new Vector2(20 * rawSpeed, 0), ForceMode2D.Force);
        }
        else if (rawSpeed != 0 && !grounded && rigidbody2D.isKinematic)
        {
            rigidbody2D.velocity = new Vector2(9, 0) * rawSpeed;
        }




    }
    // Update is called once per frame
    void Update()
    {
        //Check if you hit something while moving upwards
        bool hitHead = this.hitHead; ;
        this.hitHead = sendHeadRay().collider != null;
        if (this.hitHead)
        {
            rigidbody2D.isKinematic = false;
        }

        //Check if youre grounded
        bool wasGrounded = grounded;
        grounded = Physics2D.OverlapCircle(boxCollider2D.gameObject.transform.position, .1f, whatIsGround);

        if (!wasGrounded && grounded)
        {
            rigidbody2D.isKinematic = false;
            animator.SetBool("jump", false);
            animator.SetBool("grounded", true);
        }
        else if (!grounded)
        {
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

    }

    //Used as animation event for standard single jump
    public void setKinematic(string val)
    {
        isKinematic = bool.Parse(val);
        rigidbody2D.isKinematic = isKinematic;
        animator.SetBool("falling", true);
    }

    void jumpInput() 
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            rigidbody2D.isKinematic = true;
            animator.SetBool("jump", true);
            animator.SetTrigger("jumpTrigger");
        }             
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
        RaycastHit2D raycastHit = Physics2D.Raycast(headCollider.transform.position + (Vector3.up * 1.1f), Vector3.up, .3f, whatIsCeiling);
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

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
