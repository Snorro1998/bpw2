using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : MonoBehaviour
{
    public ParticleSystem part;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        print(other);
        if (other.tag == "fire")
        {
            print("weblussen");
            if (other.transform.localScale.x > 0)
            {
                float scale = other.transform.localScale.x - 0.05f;
                Vector3 temp = new Vector3(scale, other.transform.localScale.y, scale);
                other.transform.localScale = temp;
            }
        }
    }
}
