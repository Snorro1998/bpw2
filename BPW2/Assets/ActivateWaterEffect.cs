using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateWaterEffect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Plane>().WaterEffect.gameObject.SetActive(true);
            //other.WaterEffect.gameObject.SetActive(false)
            //print("Player raakt mij!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Plane>().WaterEffect.gameObject.SetActive(false);
            //print("Player laat me los!");
        }
    }
}
