using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    public float rotationSpeed = 10;

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
