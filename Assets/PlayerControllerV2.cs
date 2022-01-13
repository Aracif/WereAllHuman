
using UnityEngine;

public class PlayerControllerV2 : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;
    public LayerMask whatIsGround;

    private bool m_FacingRight = false;
    public bool isKinematic = false;
    public bool grounded = true;
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
        boxCollider2D = GetComponentInParent<BoxCollider2D>();
        
    }

    private void FixedUpdate()
    {
        if (isFalling())
        {
            animator.SetBool("falling", true);
        }
        else
        {
            animator.SetBool("falling", false);
        }

        //if (playerGroundRayHit && rigidbody2D.isKinematic)
        //{
        //    rigidbody2D.isKinematic = false;
        //}

        //if (isKinematic)
        //{
        //    rigidbody2D.isKinematic = true;
        //}
        //else
        //{
        //    rigidbody2D.isKinematic = false;
        //}

        if (rawSpeed != 0 && !grounded && !rigidbody2D.isKinematic)
        {
            rigidbody2D.AddRelativeForce(new Vector2(10 * rawSpeed, 0), ForceMode2D.Force);
        }
        else if (rawSpeed != 0 && !grounded && rigidbody2D.isKinematic)
        {
            rigidbody2D.velocity = new Vector2(9, 0) * rawSpeed;
        }




    }
    // Update is called once per frame
    void Update()
    {
        rawSpeed = Input.GetAxis("Horizontal");
        animator.SetFloat("speed", Mathf.Abs(rawSpeed));
        playerGroundRayHit = sendGroundRay().collider != null;

        if (rawSpeed < 0.0 && !m_FacingRight)
        {
            Flip();
        }
        else if (rawSpeed > 0.0 && m_FacingRight)
        {
            Flip();
        }
        jumpInput();

        //if (playerGroundRayHit && rigidbody2D.isKinematic)
        //{
        //    rigidbody2D.isKinematic = false;
        //    isKinematic = false;
        //}
    }

    public void setKinematic(string val)
    {
        isKinematic = bool.Parse(val);
        rigidbody2D.isKinematic = isKinematic;
        //if (!isKinematic)
        //{
        //    animator.SetBool("jump", false);
        //}
    }
    void jumpInput() 
    {
        bool wasGrounded = grounded;
        grounded = Physics2D.OverlapCircle(boxCollider2D.gameObject.transform.position, .1f, whatIsGround);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            //isKinematic = true;
            rigidbody2D.isKinematic = true;
            animator.SetBool("jump", true);
        }
        
        if (!wasGrounded && grounded)
        {
            animator.SetBool("grounded", true);
            
            if (animator.GetBool("jump"))
            {
                animator.SetBool("jump", false);
            }
        }
        else if (wasGrounded && !grounded)
        {
            animator.SetBool("grounded", false);

        }

        if (groundedCounter <= 0)
        {
            //bool wasGrounded = grounded;
            //grounded = Physics2D.OverlapCircle(boxCollider2D.gameObject.transform.position, .1f, whatIsGround);

            //if (!wasGrounded && grounded)
            //{
            //    animator.SetBool("grounded", true);
            //    isKinematic = false;
            //    if (animator.GetBool("jump"))
            //    {
            //        animator.SetBool("jump", false);
            //    }
            //}
            //else if (wasGrounded && !grounded)
            //{
            //    animator.SetBool("grounded", false);
            //}
            
            //if (Input.GetButtonDown("Jump") && grounded)
            //{
            //    animator.SetBool("jump", true);
            //    //animator.SetBool("grounded", false);
            //    groundedCounter = groundedCheckWaitTime;
            //}
            //else if (!playerGroundRayHit)
            //{
            //    animator.SetBool("grounded", false);
            //}
        }
        else
        {
            groundedCounter -= Time.deltaTime;
        }
        
    }

    bool isFalling()
    {
        return rigidbody2D.velocity.y < -1f;
    }

    void OnAnimatorMove()
    {
        Animator animator = GetComponent<Animator>();
        //print("animator move");
        if (animator)
        {
            //Vector3 newPosition = transform.position;
            //newPosition.x += animator.GetFloat("Runspeed") * Time.deltaTime;
            //transform.position = newPosition;
            transform.parent.position += animator.deltaPosition;
        }
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
        //print("Hit: " + raycastHit.collider?.gameObject?.name);
        Debug.DrawRay(boxCollider2D.bounds.center, Vector2.down * (boxCollider2D.bounds.extents.y), rayColor);
        return raycastHit;
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
}
