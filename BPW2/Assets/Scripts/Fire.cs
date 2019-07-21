﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public float health = 100f;
    public float damage = 2f;
    public float extinquishSpeed = 2f;
    private float healthPrev;

    List<Transform> objList = new List<Transform>();
    List<float> objScaleList = new List<float>();

    private void Awake()
    {
        transform.parent.GetComponent<FireManager>().nFires += 1;
        //werkte spontaan niet meer
        //FireManager.Instance.nFires += 1;

        foreach (Transform target in transform)
        {
            objList.Add(target);
            objScaleList.Add(target.localScale.x);
        }
    }

    private void Update()
    {
        if (healthPrev != health)
        {
            float scale = health / 100;
            healthPrev = health;

            for (int i = 0; i < objList.Count; i++)
            {
                objList[i].localScale = new Vector3(objScaleList[i] * scale, objScaleList[i] * scale, objScaleList[i] * scale);
            }
        }

        if (health < 25 && health > 0)
        {
            health -= extinquishSpeed * Time.deltaTime;
        }
        else if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        health = Mathf.Max(0f, health - damage);
    }

    private void OnDestroy()
    {
        FireManager.Instance.nFires -= 1;
    }
}
