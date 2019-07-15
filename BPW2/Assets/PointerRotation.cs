using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerRotation : MonoBehaviour
{
    GameObject plane;
    float rotZ = 0;

    // Start is called before the first frame update
    void Start()
    {
        plane = FindObjectOfType<Plane>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        rotZ = -1.25f * plane.GetComponent<Plane>().speed;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
}
