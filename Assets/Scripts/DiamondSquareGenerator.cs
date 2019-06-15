using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generates a mesh and random terrain using the Diamond Square algorithm
/// </summary>
public class DiamondSquareGenerator : MonoBehaviour
{

    /// <summary>
    /// The exponent for the side length of the mesh
    /// </summary>
    private int size = 10;

    /// <summary>
    /// The roughness for the diamond square algorithm
    /// </summary>
    public static float rough = 1.0f;

    /// <summary>
    /// The seed for the random numbers for the diamond square algorithm
    /// </summary>
    public static int seed = 0;

    private float minHeight;

    private float maxHeight;

    /// <summary>
    /// The mesh for the terrain
    /// </summary>
    public Mesh mesh;

    public MeshCollider meshCollider;

    private Vector3 hitPosition;

    public Gradient gradient;

    /// <summary>
    /// Initial method
    /// /// </summary>
    void Start()
    {
        this.createMesh();

        gameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }

    /// <summary>
    /// Update method that gets called once per frame
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.setHitPosition();
        }

        if (Input.GetMouseButton(0))
        {
            this.onMovement();
        }
    }

    /// <summary>
    /// Calculates the total side length of the mesh.
    /// </summary>
    /// <returns>The total side length</returns>
    private int getTotalSize()
    {
        return (int)Mathf.Pow(2, this.size) + 1;
    }

    /// <summary>
    /// Updates the mesh by eliminating the negatives and recalculating normals and bounds
    /// </summary>
    private void updateMesh()
    {
        this.eliminateNegativeHeights();
        this.mesh.RecalculateNormals();
        this.mesh.RecalculateBounds();
    }

    /// <summary>
    /// Creates a flat mesh.
    /// </summary>
    void createMesh()
    {
        int totalSize = this.getTotalSize();

        GameObject meshGameObject = new GameObject();
        meshGameObject.transform.SetParent(gameObject.transform);
        meshGameObject.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
        meshGameObject.name = "SubMesh(" + totalSize + "x" + totalSize + ")";
        MeshFilter meshFilter = meshGameObject.AddComponent<MeshFilter>();
        this.mesh = new Mesh();
        this.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshFilter.sharedMesh = this.mesh;
        this.meshCollider = meshGameObject.AddComponent<MeshCollider>();
        this.meshCollider.sharedMesh = this.mesh;

        Vector3[] vertices = new Vector3[totalSize * totalSize];
        Color[] colors = new Color[vertices.Length];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[6 * ((totalSize - 1) * (totalSize - 1))];

        // Fill vertices and uvs
        for (int index = 0, z = 0; z < totalSize; z++)
        {
            for (int x = 0; x < totalSize; x++)
            {
                vertices[index] = new Vector3(x, 0, z);
                uvs[index] = new Vector2(x, z);
                colors[index] = Color.Lerp(Color.green, Color.red, uvs[index].y);

                index++;
            }
        }

        // Fill triangles
        for (int triangleIndex = 0, index = 0, z = 0; z < totalSize - 1; z++)
        {
            for (int x = 0; x < totalSize - 1; x++)
            {
                triangles[triangleIndex++] = index;
                triangles[triangleIndex++] = index + totalSize;
                triangles[triangleIndex++] = index + 1;
                triangles[triangleIndex++] = index + 1;
                triangles[triangleIndex++] = index + totalSize;
                triangles[triangleIndex++] = index + totalSize + 1;
                index++;
            }
            index++;
        }

        this.mesh.vertices = vertices;
        this.mesh.triangles = triangles;
        //this.mesh.colors = colors;
        //this.mesh.uv = uvs;
        this.updateMesh();

        this.generateMeshHeights();
    }

    /// <summary>
    /// Sets the vertices heights of the mesh with heights calculated by the diamond square algorithm.
    /// </summary>
    void generateMeshHeights()
    {
        int totalSize = this.getTotalSize();
        float[,] heights = this.diamondSquare();

        Vector3[] vertices = this.mesh.vertices;
        Color[] colors = new Color[vertices.Length];
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int index = 0, z = 0; z < totalSize; z++)
        {
            for (int x = 0; x < totalSize; x++)
            {
                float height = heights[x, z];
                vertices[index].y = height;

                if (height > this.maxHeight)
                {
                    this.maxHeight = height;
                }
                if (height < this.minHeight)
                {
                    this.minHeight = height;
                }
                index++;
            }
        }

        Debug.Log(this.maxHeight);
        minHeight = 0;
        this.maxHeight = 100;
        for (int index = 0, z = 0; z < totalSize; z++)
        {
            for (int x = 0; x < totalSize; x++)
            {
                uvs[index] = new Vector2(x, z);
                //colors[index] = Color.Lerp(Color.green, Color.red, uvs[index].y);
                //float height = Mathf.InverseLerp(minHeight, maxHeight, vertices[index].y);
                float height = vertices[index].y / this.maxHeight;
                if (height <= 0)
                {
                    colors[index] = new Color(0.118f, 0.51f, 0.902f);
                }
                else
                {
                    colors[index] = this.gradient.Evaluate(height);
                }
                index++;
            }
        }

        this.mesh.vertices = vertices;
        this.mesh.colors = colors;
        //this.mesh.uv = uvs;
        this.updateMesh();
    }

    /// <summary>
    /// Returns a map of heights calculated with the diamond square algorithm.
    /// </summary>
    /// <returns>Map of heights</returns>
    private float[,] diamondSquare()
    {
        Random.InitState(DiamondSquareGenerator.seed);
        int totalSize = this.getTotalSize();
        int depth = totalSize - 1;
        float[,] map = new float[totalSize, totalSize];
        map[0, 0] = Random.value;
        map[0, depth] = Random.value;
        map[depth, 0] = Random.value;
        map[depth, depth] = Random.value;

        float average;
        float range = DiamondSquareGenerator.seed;
        int halfSide;

        for (int sideLength = totalSize - 1; sideLength > 1; sideLength /= 2)
        {
            halfSide = sideLength / 2;

            // Diamond step
            for (int x = 0; x < depth; x += sideLength)
            {
                for (int z = 0; z < depth; z += sideLength)
                {
                    average = (map[x, z] + map[x + sideLength, z] +
                            map[x, z + sideLength] + map[x + sideLength, z + sideLength]) / 4.0f +
                        (Random.value * (range * 2.0f)) - range;
                    map[x + halfSide, z + halfSide] = average;
                }
            }

            // Square step
            for (int x = 0; x < depth; x += halfSide)
            {
                for (int z = (x + halfSide) % sideLength; z < depth; z += sideLength)
                {
                    average = (map[(x - halfSide + depth) % depth, z] +
                            map[(x + halfSide) % depth, z] +
                            map[x, (z + halfSide) % depth] +
                            map[x, (z - halfSide + depth) % depth]) / 4.0f +
                        (Random.value * (range * 2.0f)) - range;

                    map[x, z] = average;

                    if (x == 0)
                    {
                        map[depth, z] = average;
                    }

                    if (z == 0)
                    {
                        map[x, depth] = average;
                    }
                }
            }

            range -= range * 0.5f * DiamondSquareGenerator.rough;
        }

        return map;
    }

    /// <summary>
    /// Sets all negative vertex heights to 0
    /// </summary>
    private void eliminateNegativeHeights()
    {
        Vector3[] vertices = this.mesh.vertices;
        for (int index = 0; index < vertices.Length; index++)
        {
            vertices[index].y = Mathf.Max(0, vertices[index].y);
        }

        this.mesh.vertices = vertices;
    }

    /// <summary>
    /// Moves the terrain up or down on the current hitPosition and its neighbours.
    /// The deflection depending on the mouse scroll wheel delta
    /// </summary>
    private void onMovement()
    {
        // can be - and + depending on movement up or down
        //float MouseSliderMovement = Input.GetAxis("Mouse ScrollWheel"); // for the mouse scroll wheel
        float MouseSliderMovement = Input.GetAxis("Mouse Y"); //for the mouse/touchpad movement up/down
        // get current Position
        Vector3 currentPos = Input.mousePosition;
        if (MouseSliderMovement != 0 && this.hitPosition != -Vector3.one)
        {
            this.useGaussianBell(this.hitPosition, MouseSliderMovement);
        }
    }

    /// <summary>
    /// Sets the hitPosition to the nearest Vertex of the mouse position if a hit is found,
    /// otherwise the hitPosition is set to (-1,-1,-1).
    /// </summary>
    private void setHitPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            this.hitPosition = this.getNearestVertexToPoint(hit.point);
        }
        else
        {
            this.hitPosition = -Vector3.one;
        }
    }

    /// <summary>
    /// Returns the nearest Vertex from a given Vector3 point
    /// /// </summary>
    /// <param name="point">The point to calculate the nearest Vertex from</param>
    /// <returns>The nearest Vertex of the mesh</returns>
    private Vector3 getNearestVertexToPoint(Vector3 point)
    {
        // convert point to local space
        point = transform.InverseTransformPoint(point);

        // get vertices of mesh
        Mesh mesh = this.mesh;
        Vector3[] vertices = mesh.vertices;

        // declare and initialize helper variables
        float minDistance = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;

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
                nearestVertex = vertices[index];
            }
        }
        return nearestVertex;
    }

    /// <summary>
    /// Change the height of the given vertex and its neighbors with the gaussian bell algroithm
    /// </summary>
    /// <param name="vertexToChange">vertex to change</param>
    /// <param name="heightFactor">factor to change the height (will be multiplized by 10)</param>
    private void useGaussianBell(Vector3 VertexToChange, float heightFactor)
    {
        heightFactor *= 10;
        float widthFactor = 0.01f;
        Vector3[] vertices = this.mesh.vertices;
        Color[] colors = new Color[vertices.Length];
        for (int index = 0; index < vertices.Length; index++)
        {
            Vector3 vertex = vertices[index];
            Vector3 diff = vertex - VertexToChange;

            // calculates the distance in x and z direction to between the two vertices
            float dist = Mathf.Sqrt(Mathf.Pow(diff.x, 2) + Mathf.Pow(diff.z, 2));

            // calculate the height for the current vertex with the gaussian bell algorithm
            float diffHeight = Mathf.Exp(-Mathf.Pow(widthFactor * dist, 2)) * heightFactor;
            float height = vertex.y;
            if (diffHeight > 1)
            {
                height += diffHeight;
            }

            vertices[index].y = height;
            height = height / this.maxHeight;
            if (height <= 0)
            {
                colors[index] = new Color(0.118f, 0.51f, 0.902f);
            }
            else
            {
                colors[index] = this.gradient.Evaluate(height);
            }
        }
        this.mesh.vertices = vertices;
        this.mesh.colors = colors;
        this.updateMesh();
    }
}