using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointingArrow : MonoBehaviour
{
    public GameObject arrow;

    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public List<Transform> movePoints = new List<Transform>();
    [HideInInspector]
    public int activeTarget = 0;
    int activeTargetOld = 0;
    //[HideInInspector]
    //public bool allRings = false;
    bool started = false;

    private GameObject levelController;

    private void Awake()
    {
        AddTargets();
        levelController = FindObjectOfType<LevelProperties>().gameObject; 
    }

    void Update()
    {
        checkStarted();
        rotateArrow();
        checkTargetChanged();
    }

    void rotateArrow()
    {
        if (movePoints != null)
        {
            target = movePoints[activeTarget];
            if (arrow != null)
            {
                Vector3 targetPostition = new Vector3(target.position.x, target.position.y, target.position.z); //this.transform.position.y
                arrow.transform.LookAt(targetPostition);
            }
        }
    }

    void checkStarted()
    {
        if (levelController.GetComponent<LevelProperties>().levelStarted && !started)
        {
            started = true;
            for (int i = 1; i < movePoints.Count; i++)
            {
                movePoints[i].gameObject.SetActive(false);
            }
        }
    }

    void checkTargetChanged()
    {
        if (activeTargetOld != activeTarget)
        {
            movePoints[activeTargetOld].gameObject.SetActive(false);
            movePoints[activeTarget].gameObject.SetActive(true);
            activeTargetOld = activeTarget;
        }
    }

    void AddTargets()
    {
        foreach (Transform target in transform)
        {
            movePoints.Add(target);
        }
    }
}
