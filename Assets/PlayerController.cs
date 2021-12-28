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

    private bool m_FacingRight = false;
    public bool jumping = false;
    public bool singleJumping = false;
    public float groundDistance = 0.01f;
    public float jumpForce = 500;
    public float fallMultiplier = 1.5f;

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] public float rawSpeed;
    [SerializeField] public float rigidBodyVelocityY;
    [SerializeField] public float rigidBodyVelocityX;
    [SerializeField] public float raycastHitDistance;
    [SerializeField] public float forwardMomentum;
    [SerializeField] public bool isJumping;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();    
        rigidbody2D = GetComponent<Rigidbody2D>();    
        boxCollider2D = GetComponent<BoxCollider2D>();    
    }

    private void FixedUpdate() 
    {
        isJumping = jumping;
        rigidBodyVelocityY = rigidbody2D.velocity.y;
        rigidBodyVelocityX = rigidbody2D.velocity.x;
        
        

        if (jumping && !singleJumping)
        {
            forwardMomentum = rawSpeed;
            singleJumping = true;
            animator.applyRootMotion = false;
            rigidbody2D.AddForce(new Vector2(300, jumpForce), ForceMode2D.Force);


            //animator.animatePhysics = true;
            //rigidbody2D.AddForce(Vector3.up * jumpForce);
            //rigidbody2D.AddForce(new Vector3(1, 1, 0));
            //rigidbody2D.velocity += new Vector2(4, 7) * 3;
            //Vector3 targetVelocity = new Vector2(1 * 550f * Time.fixedDeltaTime, rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            //rigidbody2D.velocity = Vector3.SmoothDamp(rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        }

        if (rawSpeed != 0)
        {
            rigidbody2D.AddForce(new Vector2(300 * rawSpeed, 0), ForceMode2D.Force);
        }

        if (singleJumping && forwardMomentum != 0)
        {
            rigidbody2D.AddForce(new Vector2(300 * forwardMomentum, 0), ForceMode2D.Force);
        }

        IsGrounded();
    }

    // Update is called once per fram e
    void Update()
    {

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

        if (Input.GetButtonDown("Jump") && animator.GetBool("grounded"))
        {
            jumping = true;
            animator.SetTrigger("jump");
        }

    }

    void IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, groundDistance, whatIsGround);
        raycastHitDistance = raycastHit.distance;
        if (Physics2D.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, groundDistance, whatIsGround))
        {
            animator.applyRootMotion = true;

            if (!animator.GetBool("grounded")) {
                animator.SetBool("grounded", true);              
                jumping = false;
                singleJumping = false;
                forwardMomentum = 0;
            }
        }
        else
        {
            fasterFall();
            animator.SetBool("grounded", false);
        }
        

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
    }

    void fasterFall()
    {
        if (rigidbody2D.velocity.y < 0)
        {
            rigidbody2D.AddForce(new Vector2(800 * rawSpeed, -100), ForceMode2D.Force);
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

}
