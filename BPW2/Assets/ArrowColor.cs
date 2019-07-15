using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowColor : MonoBehaviour
{
    Vector3 parentPos = new Vector3(0,0,0);
    Vector3 parentRot = new Vector3(0, 0, 0);

    public Color colorFront;
    public Color colorBack;

    private float rot = 0;
    private float colorFactor = 0;

    private Color arrowColor;

    // Update is called once per frame
    void Update()
    {
        float planeRotY = transform.parent.rotation.eulerAngles.y;
        float rotY = transform.rotation.eulerAngles.y; 

        rot = rotY < planeRotY ? planeRotY - rotY : rotY - planeRotY;
        colorFactor = (rot >= 180 && rot < 360) ? 1.0f - ((rot - 180.0f) / 180.0f) : rot / 180.0f;

        //print("Plane rotation: " + gameObject.transform.parent.rotation.eulerAngles.y + "\nOwn rotation: " + gameObject.transform.rotation.eulerAngles.y);

        arrowColor = Color.Lerp(colorFront, colorBack, colorFactor);

        GetComponent<Renderer>().material.color = arrowColor;
        GetComponent<Renderer>().material.SetColor("_EmissionColor", arrowColor);
    }
}
