using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform playerTransform;
    private Camera camera;
    public float smoothing = 10f;
    public Vector3 offset;

    Vector2 velocity;
    public float smoothTimeY;
    public float smoothTimeX;

    [SerializeField] private GameObject floatingPoints;


    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        camera = GetComponent<Camera>();
        Instantiate(floatingPoints, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis ("Mouse ScrollWheel") > 0)
        {
            camera.orthographicSize--;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            camera.orthographicSize++;
        }
    }

    private void FixedUpdate()
    {
        float posX = Mathf.SmoothDamp(transform.position.x, playerTransform.position.x, ref velocity.x, smoothTimeX);
        float posY = Mathf.SmoothDamp(transform.position.y, playerTransform.position.y, ref velocity.y, smoothTimeY);

        transform.position = new Vector3(posX, posY, transform.position.z);
    }
    private void LateUpdate()
    {
        //smoothFollow();
        //basicFollow();
    }

    //private void FixedUpdate()
    //{
    //    smoothFollow();
    //}

    private void basicFollow()
    {
        Vector3 temp = transform.position;

        temp.x = playerTransform.position.x;
        temp.y = playerTransform.position.y;

        transform.position = temp;

    }

    private void smoothFollow()
    {
        Vector3 desiredPosition = playerTransform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothing * Time.deltaTime);
        transform.position = smoothedPosition;

        //transform.LookAt(playerTransform);
    }
}
