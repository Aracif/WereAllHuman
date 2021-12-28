using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform playerTransform;
    private Camera camera;
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
        Vector3 temp = transform.position;

        temp.x = playerTransform.position.x;

        transform.position = temp;

    }


}
