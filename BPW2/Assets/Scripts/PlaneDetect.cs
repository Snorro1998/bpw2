using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneDetect : MonoBehaviour
{
    //verrukkelijke spaghetti
    private void OnTriggerEnter(Collider other)
    {
        if (GetComponentInParent<PointingArrow>().target == transform && other.transform.tag == "Player")
        {
            AudioManager.Instance.playSound("honk");

            if (GetComponentInParent<PointingArrow>().activeTarget < GetComponentInParent<PointingArrow>().movePoints.Count - 1)
            {
                GetComponentInParent<PointingArrow>().activeTarget += 1;
            }
            else
            {
                FindObjectOfType<LevelProperties>().levelComplete = true;
                //GetComponentInParent<PointingArrow>().allRings = true;
                //Destroy(gameObject);
            }
        }
    }
}
