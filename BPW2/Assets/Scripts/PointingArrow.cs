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
    int activeTargetOld = 1;
    bool started = false;

    private void Awake()
    {
        AddTargets();
    }

    void Update()
    {
        checkStarted();
        checkTargetChanged();
    }

    void checkStarted()
    {
        if (LevelProperties.Instance.levelStarted && !started)
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
            target = movePoints[activeTarget];
            arrow.GetComponent<ArrowColor>().target = movePoints[activeTarget];
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
