using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMovement : MonoBehaviour
{/*
    private float xRotation = 0;
    public float xRotateSpeed = 10;

    public float angleAttack, liftCoefficient, dragCoefficient = 0;

    private float dragSineAmp = 1.0f;
    private float dragSineOffset = 1.0f;

    //const float

    private void Update()
    {

        calcAngleAttack();
        calcDragCoef();
        calcLiftCoef();

        //float lift = 0.5f * Mathf.Pow(speed, 2) * 130 * liftCoefficient;

        
    }

    //baggerfunctie maar het werkt
    void calcAngleAttack()
    {
        float xRot = xRotateSpeed * Time.deltaTime;
        xRotation = (xRotation + xRot) % 360;

        angleAttack = xRotation;
        if (angleAttack < 0)
        {
            angleAttack = 360 + angleAttack;
        }
        angleAttack = 360 - angleAttack;
        transform.Rotate(xRot, 0, 0, Space.Self);
    }

    void calcDragCoef()
    {
        if (angleAttack >= 0 && angleAttack <= 180)
        {
            dragSineAmp = 1.0f;
        }
        else
        {
            dragSineAmp = 0.75f;
        }

        dragSineOffset = dragSineAmp + 0.05f;
        dragCoefficient = dragSineOffset + dragSineAmp * Mathf.Sin((2.0f * Mathf.PI / 180) * (angleAttack - 45.0f));
    }

    void calcLiftCoef()
    {
        if (angleAttack >= 0 && angleAttack <= 15)
        {
            liftCoefficient = 0.054f * angleAttack + 0.886f;
        }
        else if (angleAttack > 15 && angleAttack <= 20)
        {
            liftCoefficient = -0.1f * angleAttack + 3.2f;
        }
    }
    */
    
    public float speed = 90f;
    public bool frozen = false;
    public Transform dirTransform;

    void Start()
    {
        Debug.Log("Script added to: " + gameObject.name);
    }

    void Update()
    {
        dirTransform.position = transform.position;
        dirTransform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);//transform.rotation.y;

        Vector3 moveCamTo = transform.position - transform.forward * 10.0f + Vector3.up * 5.0f;
        Camera.main.transform.position = moveCamTo;
        Camera.main.transform.LookAt(transform.position + transform.forward * 30.0f);

        if (!frozen) transform.position += transform.forward * Time.deltaTime * speed;

        speed -= transform.forward.y * Time.deltaTime * 50.0f;

        if (speed < 35.0f)
        {
            speed = 35.0f;
        }

        transform.Rotate(Input.GetAxis("Vertical"), 0.0f, -Input.GetAxis("Horizontal"));

        float terrainHeightWhereWeAre = Terrain.activeTerrain.SampleHeight(transform.position);

        if (terrainHeightWhereWeAre > transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, terrainHeightWhereWeAre, transform.position.z);
        }

        Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
        Debug.DrawRay(transform.position, Vector3.forward * 5, Color.green);
    }
}
