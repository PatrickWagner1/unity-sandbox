using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour
{
    float movement = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //Debug.Log("Linke Maustaste gedrückt!");
            //this.mouseEvent();
        }
    }

    private void mouseEvent()
    {

        // can be - and + depending on movement up or down
        this.movement = Input.GetAxis("Mouse Y");
        // get current Position
        Vector3 currentPos = Input.mousePosition;

        if (this.movement != 0)
        {
            Vector3 nearestVertex = getNearestVertexToPoint(currentPos);
            Debug.Log(nearestVertex);


        }
        else
        {
            Debug.Log("movement == 0");
        }
    }

    private Vector3 getNearestVertexToPoint(Vector3 point)
    {
        // convert point to local space
        point = transform.InverseTransformPoint(point);

        // get vertices of mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        // declare and initialize helper variables
        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;

        // scan all vertices to find nearest
        for (var i = 0; i < vertices.Length; i++)
        {
            // get difference between point and each vertex and calculate its quare diffrenece
            Vector3 diff = point - vertices[i];
            float distSqr = diff.sqrMagnitude;

            // distance^2 smaller than current detected smallest distance^2?
            if (distSqr < minDistanceSqr)
            {
                // set new current smallest distance^2 and nearestVertex
                minDistanceSqr = distSqr;
                nearestVertex = vertices[i];
            }
        }
        return nearestVertex;
    }
}