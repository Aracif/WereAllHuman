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
    public float fallMultiplier = 1.5f;

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] public float rawSpeed;
    [SerializeField] public float rigidBodyVelocityY;
    [SerializeField] public float rigidBodyVelocityX;
    [SerializeField] public float raycastHitDistance;
    [SerializeField] public float forwardMomentum;
    [SerializeField] public float jumpForce = 700f;
    [SerializeField] public bool leftGround;
    [SerializeField] public bool rootMotionEnabled;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();    
        rigidbody2D = GetComponent<Rigidbody2D>();    
        boxCollider2D = GetComponent<BoxCollider2D>();    
        
    }

    private void FixedUpdate() 
    {
        //transform.position += animator.deltaPosition;
        //transform.rotation = animator.deltaRotation * transform.rotation;
        //invalidWalkRotation();

        if (jumping && !singleJumping)
        {

            forwardMomentum = rawSpeed;
            singleJumping = true;
            rigidbody2D.AddForce(new Vector2(100 * rawSpeed, jumpForce), ForceMode2D.Force);
            if (rawSpeed != 0)
            {
                rigidbody2D.AddRelativeForce(new Vector2(200 * rawSpeed, 0), ForceMode2D.Force);
            }

        }
        else if (singleJumping)
        {


            if (movingUpward(15))
            {
                leftGround = true;
                animator.SetBool("grounded", false);
            }
            else if (landed() && leftGround)
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
                }
            }
            //Allow slight horizontal movement in the air
            if (rawSpeed != 0)
            {
                rigidbody2D.AddRelativeForce(new Vector2(150 * rawSpeed, 0), ForceMode2D.Force);
            }
            
            if (!movingUpward(0))
            {
                //If the player is falling from a jump accelerate fall rate slightly
                rigidbody2D.AddRelativeForce(new Vector2(20 * rawSpeed, -100), ForceMode2D.Force);
            }
        }
    }

    bool movingUpward(float speed)
    {
        return rigidbody2D.velocity.y > speed;
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

        if (jumping == false)
        {
            if (Input.GetButtonDown("Jump"))
            {

                RaycastHit2D ray = sendGroundRay();
                if (ray.collider != null) //We're on the ground!
                {
                    jumping = true;
                    animator.SetTrigger("jump");
                }
            }
        }
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
        if (collision.otherCollider.ToString().Contains("CircleCollider"))
        {
            print("ouch");
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
