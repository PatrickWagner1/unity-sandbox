using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour
{
    /// True, if the terrain is currently moving up or down, otherwise false
    private bool isEditMode;

    /// The index of the vertex, which was hitted with the mouse
    private int hitVertexIndex;

    /// An object of the terrain.
    private TerrainObject terrain;

    /// <summary>
    /// Sets the object for the terrain.
    /// </summary>
    void Start()
    {
        this.terrain = GetComponent<TerrainObject>();
    }

    /// <summary>
    /// Update method that gets called once per frame
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.setHitVertexIndex();
        }

        if (Input.GetMouseButton(0))
        {
            this.onMovement();
        }
        if (Input.GetMouseButtonUp(0))
        {
            this.setTempVertices();
        }
    }

    /// <summary>
    /// Sets the hitVertexIndex to the nearest Vertex of the mouse position if a hit is found,
    /// otherwise the hitVertexIndex is set to -1.
    /// </summary>
    private void setHitVertexIndex()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        LayerMask mask = LayerMask.GetMask("Terrain");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            this.hitVertexIndex = this.getNearestVertexIndexToPoint(hit.point);
            this.isEditMode = true;
        }
        else
        {
            this.hitVertexIndex = -1;
        }
    }

    /// <summary>
    /// Moves the terrain up or down on the current hitVertexIndex and its neighbours.
    /// The deflection depending on the mouse scroll wheel delta
    /// </summary>
    private void onMovement()
    {
        if (this.hitVertexIndex > -1)
        {
            // mouseSliderMovement can be - and + depending on movement up or down

            // for the mouse scoll wheel move up/down
            float mouseSliderMovement = Input.GetAxis("Mouse ScrollWheel") * 5;
            if (mouseSliderMovement == 0 && !Input.GetKey(KeyCode.LeftControl))
            {
                // for the mouse/touchpad movement up/down
                mouseSliderMovement = Input.GetAxis("Mouse Y");
            }

            float hitHeight = terrain.tempVertices[this.hitVertexIndex].y;
            if (!this.isEditMode && hitHeight < 0 && mouseSliderMovement > 0)
            {
                // Only called, on first movement after the mouse was clicked.
                // If the hitted point has a negative value and the movement goes up,
                // the terrain will be moved up, until hitted point height is zero.
                this.useGaussianBell(this.hitVertexIndex, -hitHeight);
            }

            if (mouseSliderMovement != 0)
            {
                this.useGaussianBell(this.hitVertexIndex, mouseSliderMovement * 10);
            }
        }
    }

    /// <summary>
    /// Overrides the old heights of the temp vertices with the new heights of the temp vertices.
    /// Calls recalculateBuoyPosition() to update buoy position.
    /// </summary>
    private void setTempVertices()
    {
        Vector3[] tempVertices = terrain.tempVertices;
        int totalSize = DiamondSquareGenerator.getTotalSize(terrain.size);
        float[,] heights = new float[totalSize, totalSize];
        if (tempVertices.Length > 0)
        {
            for (int index = 0, z = 0; z < totalSize; z++)
            {
                for (int x = 0; x < totalSize; x++)
                {
                    tempVertices[index].y += terrain.tempDiffHeights[x, z];
                    heights[x, z] = tempVertices[index].y;

                    index++;
                }
            }
        }


        terrain.tempVertices = tempVertices;
        terrain.tempDiffHeights = new float[totalSize, totalSize];
        terrain.setColliderHeights(heights);
        terrain.recalculateBuoyPosition();
        this.isEditMode = false;
    }

    /// <summary>
    /// Returns the nearest vertex for a given Vector3 point.
    /// </summary>
    /// <param name="point">The point to calculate the nearest Vertex from</param>
    /// <returns>The nearest vertex of the mesh</returns>
    private int getNearestVertexIndexToPoint(Vector3 point)
    {
        // convert point to local space
        point = transform.InverseTransformPoint(point);

        // get vertices of mesh
        Mesh mesh = terrain.mesh;
        Vector3[] vertices = mesh.vertices;

        // declare and initialize helper variables
        float minDistance = Mathf.Infinity;
        int nearestVertexIndex = 0;

        // scan all vertices to find nearest
        for (int index = 0; index < vertices.Length; index++)
        {
            // get difference between point and each vertex and calculate its quare diffrenece
            Vector3 diff = point - vertices[index];
            float dist = diff.sqrMagnitude;

            // distance smaller than current detected smallest distance?
            if (dist < minDistance)
            {
                // set new current smallest distance and nearestVertex
                minDistance = dist;
                nearestVertexIndex = index;
            }
        }
        return nearestVertexIndex;
    }

    /// <summary>
    /// Change the height of the given vertex and its neighbors with the gaussian bell algroithm
    /// </summary>
    /// <param name="vertexToChange">index of the vertex to change</param>
    /// <param name="heightFactor">factor to change the height</param>
    private void useGaussianBell(int indexOfVertexToChange, float heightFactor)
    {
        int totalSize = DiamondSquareGenerator.getTotalSize(terrain.size);

        float widthFactor = 0.02f;
        Vector3[] tempVertices = terrain.tempVertices;
        Vector3[] vertices = new Vector3[tempVertices.Length];
        float[,] diffHeights = terrain.tempDiffHeights;
        Color[] colors = new Color[vertices.Length];

        Vector3 vertexToChange = tempVertices[indexOfVertexToChange];

        for (int index = 0, z = 0; z < totalSize; z++)
        {
            for (int x = 0; x < totalSize; x++)
            {
                Vector3 vertex = tempVertices[index];
                Vector3 diff = vertex - vertexToChange;

                // calculates the distance in x and z direction to between the two vertices
                float dist = Mathf.Sqrt(Mathf.Pow(diff.x, 2) + Mathf.Pow(diff.z, 2));

                // calculate the height for the current vertex with the gaussian bell algorithm
                float diffHeight = Mathf.Exp(-Mathf.Pow(widthFactor * dist, 2)) * heightFactor;
                diffHeights[x, z] += diffHeight;

                float height = vertex.y + diffHeights[x, z];
                vertex.y = height;
                vertices[index] = vertex;
                colors[index] = terrain.getColorFromHeight(height);

                index++;
            }
        }

        terrain.tempDiffHeights = diffHeights;
        terrain.updateMesh(vertices, colors);
    }
}