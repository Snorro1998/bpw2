using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoWater : MonoBehaviour
{
    public string info = "";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            LevelProperties.Instance.infoPanel.GetComponent<Blinking>().setInfo(info);
            Destroy(gameObject);
        }
    }
}
