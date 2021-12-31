using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoubleTap : MonoBehaviour
{
    [SerializeField] public bool firstButtonPressed;
    [SerializeField] public bool reset;
    [SerializeField] public float timeOfFirstButton;
    [SerializeField] private Animator animator;
    [SerializeField] public float rawSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.B) && firstButtonPressed)
        {
            if (Time.time - timeOfFirstButton < 0.5f)
            {
                Debug.Log("Double Tap");
                animator.SetBool("walkingOnly", true);
                //animator.SetFloat("speed", Mathf.Abs(1));
            }
            else
            {
                Debug.Log("Too late");
            }

            reset = true;
        }

        if (Input.GetKeyDown(KeyCode.B) && !firstButtonPressed)
        {
            firstButtonPressed = true;
            timeOfFirstButton = Time.time;
        }

        if (reset)
        {
            firstButtonPressed = false;
            reset = false;
        }
    }
}
