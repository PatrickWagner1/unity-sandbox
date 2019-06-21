using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    float scrollSpeed = 0.01f;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        rend.material.SetTextureOffset("_NormalMap", new Vector2(offset, offset/2.0f));
        rend.material.SetTextureOffset("_NormalMap2", new Vector2(-offset, offset));
    }

}
