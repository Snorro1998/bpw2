using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followTarget;

    public int fovZoomedOut = 120;
    public int fovNormal = 60;
    public float smooth = 5;

    private bool isZoomed = false;
    /*
    public float smoothing = 0.25f;
    public Vector3 startOffset = new Vector3(0, 0, 0);*/
    public Vector3 offset = new Vector3(0, 0, 0);

    //bool zoom = false;


    private void Update()
    {
        Vector3 xOffset = followTarget.transform.right * offset.x;
        Vector3 yOffset = followTarget.transform.up * offset.y;
        Vector3 zOffset = followTarget.transform.forward * offset.z;

        Vector3 newOffset = xOffset + yOffset + zOffset;
        Vector3 newPosition = followTarget.transform.position + newOffset;

        transform.position = newPosition;
        transform.eulerAngles = followTarget.eulerAngles;

        if (Input.GetMouseButtonDown(1))
        {
            isZoomed = !isZoomed;
        }

        if (isZoomed)
        {
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, fovZoomedOut, Time.deltaTime * smooth);
        }
        else
        {
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, fovNormal, Time.deltaTime * smooth);
        }

        /*
        Vector3 startXOffset = followTarget.transform.right * startOffset.x;
        Vector3 startYOffset = followTarget.transform.up * startOffset.y;
        Vector3 startZOffset = followTarget.transform.forward * startOffset.z;

        Vector3 newStartOffset = startXOffset + startYOffset + startZOffset;
        Vector3 newStartPosition = followTarget.transform.position + newStartOffset;

        Vector3 xOffset = followTarget.transform.right * offset.x;
        Vector3 yOffset = followTarget.transform.up * offset.y;
        Vector3 zOffset = followTarget.transform.forward * offset.z;

        Vector3 newOffset = xOffset + yOffset + zOffset;
        Vector3 newPosition = followTarget.transform.position + newOffset;

        Vector3 newPositionSmooth = Vector3.Lerp(transform.position, newPosition, smoothing);

        if (transform.position != newPosition)
        {
            if (!zoom)
            {
                print("ZOOOOOM");
                transform.position = newStartPosition;
                zoom = true;
            }
            else
            {
                transform.eulerAngles = followTarget.eulerAngles;
                transform.position = newPositionSmooth;
                //Vector3 newPositionSmooth = Vector3.Lerp(transform.position, newPosition, smoothing);
            }
        }
        else
        {
            transform.position = newPosition;
        }*/


        //Vector3 newPositionSmooth = Vector3.Lerp(transform.position, newPosition, smoothing);
        //Vector3 newRotationSmooth = Vector3.Lerp(transform.eulerAngles, followTarget.transform.eulerAngles, smoothing);


    }
}
