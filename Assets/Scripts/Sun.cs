using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vector = new Vector3(50,0,50);
        transform.RotateAround(vector, new Vector3(0.2f,0.8f,0f),5f*Time.deltaTime);
        transform.LookAt(vector);
    }
}
