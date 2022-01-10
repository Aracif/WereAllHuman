using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Animator anim = this.GetComponentInChildren<Animator>();
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
        if (collision.gameObject.tag.Equals("ground"))
        {
            print("collide ground");
            anim.SetBool("jump", false);

        }
        //if (collision.relativeVelocity.magnitude > 2)
        //audioSource.Play();
    }
}
