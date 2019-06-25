using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        print(rb.velocity);
        rb.drag = 0.5f * 1.93f * rb.velocity.z * 130 * 0.03f;
        if (Input.GetKey("w"))
        {
            rb.AddForce(transform.forward * 1789679f);

            
            //rb.drag = 10;
            //print("madden john!");
        }
    }
}
