using UnityEngine;
using UnityEngine.UI;

public class WaterLevelUI : MonoBehaviour
{
    private GameObject plane;

    void Start()
    {
        plane = FindObjectOfType<Plane>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Slider>().value = plane.GetComponent<Plane>().waterLevel;
    }
}
