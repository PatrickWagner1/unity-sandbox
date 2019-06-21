using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    float scrollSpeed = 0.005f;
    Renderer rend;

    // Set the render object
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Set the offset for the normal maps of the water
    void Update()
    {
        float offset = (Time.time * 0.05f) % 1;
        rend.material.SetTextureOffset("_NormalMap", new Vector2(offset, 0));
        rend.material.SetTextureOffset("_NormalMap2", new Vector2(0, offset));
    }

}
