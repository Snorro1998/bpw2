using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    public float xSpeed = .1f;
    public float ySpeed = .1f;
    public float x = 0;

    public float sizze = 100;
    

    public float minValue = 1;
    public float maxValue = 2;
    public float period = 2;

    float amplitude, evenStand = 0;

    public float size = 0;

    public Vector2 waterStartSize = new Vector2(0, 0);

    Terrain terrain;
    TerrainLayer[] terrainLayers;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainLayers = terrain.terrainData.terrainLayers;
        waterStartSize = terrainLayers[1].tileSize;
    }

    void Update()
    {
        //om tijdens t testen aan te passen, kan later in de startfunctie
        amplitude = (maxValue - minValue) / 2.0f;
        evenStand = minValue + amplitude;

        size = amplitude * Mathf.Sin((2 * Mathf.PI / period) * x) + evenStand;
        x = (x + Time.deltaTime) % Mathf.Max(0.001f, period);
        terrainLayers[1].tileOffset += new Vector2(xSpeed,ySpeed);
        terrainLayers[1].tileSize = new Vector2(sizze, sizze);
        //terrainLayers[1].tileSize = size * waterStartSize;
    }
}


