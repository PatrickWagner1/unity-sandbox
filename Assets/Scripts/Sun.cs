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
        transform.RotateAround(new Vector3(25,20,25), new Vector3(0f,1f,0f),10f*Time.deltaTime);
        transform.LookAt(new Vector3(0,0,0));
    }
}
