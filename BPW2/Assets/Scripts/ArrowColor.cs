using UnityEngine;

public class ArrowColor : MonoBehaviour
{
    public Color colorFront;
    public Color colorBack;

    public Transform target;

    void Update()
    {
        updateColor();
        rotateArrow();
    }

    void rotateArrow()
    {
        if(target != null)
        {
            Vector3 targetPostition = new Vector3(target.position.x, target.position.y, target.position.z);
            transform.LookAt(targetPostition);
        }
    }

    void updateColor()
    {
        float planeRotY = transform.parent.rotation.eulerAngles.y;
        float rotY = transform.rotation.eulerAngles.y;

        float rot = rotY < planeRotY ? planeRotY - rotY : rotY - planeRotY;
        float colorFactor = (rot >= 180 && rot < 360) ? 1.0f - ((rot - 180.0f) / 180.0f) : rot / 180.0f;

        Color arrowColor = Color.Lerp(colorFront, colorBack, colorFactor);

        GetComponent<Renderer>().material.color = arrowColor;
        GetComponent<Renderer>().material.SetColor("_EmissionColor", arrowColor);
    }
}
