using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform playerTransform;
    private Camera camera;
    public float smoothing = 10f;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        camera = GetComponent<Camera>();
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

    private void LateUpdate()
    {
        //smoothFollow();
        basicFollow();
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
