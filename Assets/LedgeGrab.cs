
using UnityEngine;

public class LedgeGrab : MonoBehaviour
{

    private bool greenBox, redBox;
    public float redXOffset, redYOffset, redXSize, redYSize, greenXOffset, greenYOffset, greenXSize, greenYSize;
    private Rigidbody2D rigidbody2D;
    private float startingGrav;
    public LayerMask ground;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        startingGrav = rigidbody2D.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        greenBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXSize, greenYSize), 0f, ground);
        redBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYOffset), new Vector2(redXSize, redYSize), 0f, ground);

        if (greenBox && !redBox && !PlayerController.isLedgeGrabbing && PlayerController.singleJumping)
        {
            PlayerController.isLedgeGrabbing = true;
            print("grabbedeeem");
        }
        else
        {
            rigidbody2D.gravityScale = startingGrav;
            PlayerController.isLedgeGrabbing = false;
        }

        if (PlayerController.isLedgeGrabbing)
        {
            rigidbody2D.velocity = new Vector2(0f, 0f);
            rigidbody2D.gravityScale = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (redXOffset * transform.localScale.x), transform.position.y + redYOffset), new Vector2(redXSize, redYSize));

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (greenXOffset * transform.localScale.x), transform.position.y + greenYOffset), new Vector2(greenXSize, greenYSize));

    }
}
