using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour
{
    private bool isEditMode;

    private int hitVertexIndex;

    private TerrainObject terrain;

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
            // can be - and + depending on movement up or down
            float mouseSliderMovement = Input.GetAxis("Mouse ScrollWheel") * 5; // for the mouse scroll wheel
            if (mouseSliderMovement == 0 && !Input.GetKey(KeyCode.LeftControl))
            {
                mouseSliderMovement = Input.GetAxis("Mouse Y"); //for the mouse/touchpad movement up/down
            }

            float hitHeight = terrain.tempVertices[this.hitVertexIndex].y;
            if (!this.isEditMode && hitHeight < 0 && mouseSliderMovement > 0)
            {
                this.useGaussianBell(this.hitVertexIndex, -hitHeight);
            }

            // get current Position
            Vector3 currentPos = Input.mousePosition;
            if (mouseSliderMovement != 0)
            {
                this.useGaussianBell(this.hitVertexIndex, mouseSliderMovement * 10);
            }
        }
    }

    private void setTempVertices()
    {
        Vector3[] tempVertices = terrain.tempVertices;

        for (int index = 0; index < tempVertices.Length; index++)
        {
            tempVertices[index].y += terrain.tempDiffHeights[index];
        }
        terrain.tempVertices = tempVertices;
        terrain.tempDiffHeights = new float[tempVertices.Length];
        this.recalculateBuoyPosition();
        this.isEditMode = false;
    }

    /// <summary>
    /// Returns the nearest Vertex from a given Vector3 point
    /// /// </summary>
    /// <param name="point">The point to calculate the nearest Vertex from</param>
    /// <returns>The nearest Vertex of the mesh</returns>
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
    /// <param name="vertexToChange">vertex to change</param>
    /// <param name="heightFactor">factor to change the height (will be multiplized by 10)</param>
    private void useGaussianBell(int indexOfVertexToChange, float heightFactor)
    {
        float widthFactor = 0.02f;
        Vector3[] tempVertices = terrain.tempVertices;
        Vector3[] vertices = new Vector3[tempVertices.Length];
        float[] diffHeights = terrain.tempDiffHeights;
        Color[] colors = new Color[vertices.Length];

        Vector3 vertexToChange = tempVertices[indexOfVertexToChange];

        for (int index = 0; index < vertices.Length; index++)
        {
            Vector3 vertex = tempVertices[index];
            Vector3 diff = vertex - vertexToChange;

            // calculates the distance in x and z direction to between the two vertices
            float dist = Mathf.Sqrt(Mathf.Pow(diff.x, 2) + Mathf.Pow(diff.z, 2));

            // calculate the height for the current vertex with the gaussian bell algorithm
            float diffHeight = Mathf.Exp(-Mathf.Pow(widthFactor * dist, 2)) * heightFactor;
            diffHeights[index] += diffHeight;

            float height = vertex.y + diffHeights[index];
            vertex.y = height;
            vertices[index] = vertex;
            colors[index] = terrain.getColorFromHeight(height);
        }

        terrain.tempDiffHeights = diffHeights;
        terrain.updateMesh(vertices, colors);
    }

    public void recalculateBuoyPosition()
    {
        float minHeight = Mathf.Infinity;
        int minHeightVertexIndex = 0;
        Vector3[] tempVertices = terrain.tempVertices;

        for (int index = 0; index < tempVertices.Length; index++)
        {
            float height = tempVertices[index].y;

            if (height < minHeight)
            {
                minHeight = height;
                minHeightVertexIndex = index;
            }
        }

        if (terrain.buoyPrefab != null)
        {
            if (minHeight <= 0)
            {
                if (terrain.buoy == null)
                {
                    terrain.buoy = Instantiate(terrain.buoyPrefab, terrain.mesh.vertices[minHeightVertexIndex] * 0.05f, Quaternion.Euler(-90, 0, 0));
                }
                terrain.buoy.transform.position = terrain.mesh.vertices[minHeightVertexIndex] * 0.05f;
            } else if (terrain.buoy != null)
            {
                Destroy(terrain.buoy);
            }
        }
    }
}