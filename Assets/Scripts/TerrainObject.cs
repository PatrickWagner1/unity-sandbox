using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainObject : MonoBehaviour
{
    /// The exponent for the side length of the mesh
    public int size = 10;

    /// The roughness for the diamond square algorithm
    public static float rough = 1.0f;

    /// The seed for the random numbers for the diamond square algorithm
    public static int seed = 212;

    /// True, if contour lines should be displayed, otherwise false
    public static bool showContourLines = true;

    /// Defines the maximum for the range of the color mapping
    public float maxHeight = 100;

    /// The scale of the collider inversely proportional to the mesh
    public int colliderScale = 16;

    /// The mesh for the terrain
    public Mesh mesh;

    /// The mesh collider for the terrain
    private MeshCollider meshCollider;

    /// The game object of the mesh
    private GameObject meshGameObject;

    /// Temp vertices has also stored the negative height values
    /// (needed for realistic terrain moving in water area and for the buoy)
    public Vector3[] tempVertices;

    /// Temp heights to add on the current vertices heights after editing finished
    public float[,] tempDiffHeights;

    /// Gradient as a color map
    public Gradient gradient;

    /// Color for water (shader will overwrites the color)
    public Color waterColor;

    /// Prefab of the buoy
    public GameObject buoyPrefab;

    /// The buoy itself (will be set in water at deepest point)
    public GameObject buoy;

    private Mesh colliderMesh;

    /// <summary>
    /// Set contour lines toggle and creates the terrain.
    /// /// </summary>
    void Awake()
    {
        this.createMesh();

        gameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }

    /// <summary>
    /// Updates the mesh by eliminating the negative heights and recalculating normals and bounds.
    /// </summary>
    public void updateMesh(Vector3[] vertices, Color[] colors)
    {
        this.mesh.vertices = vertices;
        this.mesh.colors = colors;

        this.eliminateNegativeHeights();

        this.mesh.RecalculateNormals();
        this.mesh.RecalculateBounds();
    }

    /// <summary>
    /// Sets the relevant heights of the collider.
    /// </summary>
    /// <param name="heights">The heights of the whole mesh</param>
    public void setColliderHeights(float[,] heights)
    {
        Vector3[] colliderVertices = this.colliderMesh.vertices;
        int totalSize = DiamondSquareGenerator.getTotalSize(this.size);
        int colliderTotalSize = totalSize / this.colliderScale;
        for (int index = 0, z = 0; z < colliderTotalSize; z++)
        {
            for (int x = 0; x < colliderTotalSize; x++)
            {
                colliderVertices[index].y = heights[x * this.colliderScale, z * this.colliderScale];

                index++;
            }
        }
        this.colliderMesh.vertices = colliderVertices;

        this.updateCollider();
    }

    /// <summary>
    /// Updates the collider mesh.
    /// </summary>
    public void updateCollider()
    {
        this.colliderMesh.RecalculateNormals();
        this.colliderMesh.RecalculateBounds();
        this.meshCollider.sharedMesh = this.colliderMesh;
    }

    /// <summary>
    /// Sets the color depending on height (water or soil[with matching color for its height])
    /// </summary>
    /// <param name="height">height to calculate color</param>
    /// <returns>Color for given height</returns>
    public Color getColorFromHeight(float height)
    {
        Color color;
        height = height / this.maxHeight;
        if (height <= 0)
        {
            color = this.waterColor;
        }
        else
        {
            color = this.gradient.Evaluate(height);
        }
        return color;
    }

    /// <summary>
    /// Creates a flat mesh
    /// </summary>
    private void createMesh()
    {
        int totalSize = DiamondSquareGenerator.getTotalSize(this.size);

        // Creates the mesh game object
        this.meshGameObject = new GameObject();
        this.meshGameObject.SetActive(false);
        this.meshGameObject.transform.SetParent(gameObject.transform);
        this.meshGameObject.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
        this.meshGameObject.name = "TerrainMesh";
        this.meshGameObject.layer = 8;

        // Adds a mesh filter to the mesh
        MeshFilter meshFilter = this.meshGameObject.AddComponent<MeshFilter>();
        this.mesh = new Mesh();
        this.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshFilter.sharedMesh = this.mesh;

        // Adds a mesh collider to the mesh
        int colliderTotalSize = totalSize / this.colliderScale;
        this.colliderMesh = new Mesh();
        this.meshCollider = this.meshGameObject.AddComponent<MeshCollider>();

        Vector3[] vertices = new Vector3[totalSize * totalSize];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[6 * ((totalSize - 1) * (totalSize - 1))];

        Vector3[] colliderVertices = new Vector3[colliderTotalSize * colliderTotalSize];
        int[] colliderTriangles = new int[6 * ((colliderTotalSize - 1) * (colliderTotalSize - 1))];

        // Fill vertices and uvs
        for (int index = 0, z = 0; z < totalSize; z++)
        {
            for (int x = 0; x < totalSize; x++)
            {
                vertices[index] = new Vector3(x, 0, z);
                uvs[index] = new Vector2(x, z);

                index++;
            }
        }

        // Fill collider vertices
        for (int index = 0, z = 0; z < colliderTotalSize; z++)
        {
            for (int x = 0; x < colliderTotalSize; x++)
            {
                colliderVertices[index] = new Vector3(x * this.colliderScale, 0, z * this.colliderScale);

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

        // Fill collider triangles
        for (int triangleIndex = 0, index = 0, z = 0; z < colliderTotalSize - 1; z++)
        {
            for (int x = 0; x < colliderTotalSize - 1; x++)
            {
                colliderTriangles[triangleIndex++] = index;
                colliderTriangles[triangleIndex++] = index + colliderTotalSize;
                colliderTriangles[triangleIndex++] = index + 1;
                colliderTriangles[triangleIndex++] = index + 1;
                colliderTriangles[triangleIndex++] = index + colliderTotalSize;
                colliderTriangles[triangleIndex++] = index + colliderTotalSize + 1;
                index++;
            }
            index++;
        }

        // Add vertices, triangles and uvs to mesh
        this.mesh.vertices = vertices;
        this.mesh.triangles = triangles;
        this.mesh.uv = uvs;

        // Add vertices and triangles to collider mesh
        this.colliderMesh.vertices = colliderVertices;
        this.colliderMesh.triangles = colliderTriangles;
    }

    /// <summary>
    /// Sets the vertices heights of the mesh with heights calculated by the diamond square algorithm.
    /// Also sets a buoy on the deepest water point.
    /// </summary>
    public void generateMeshHeights()
    {
        int totalSize = DiamondSquareGenerator.getTotalSize(this.size);
        float[,] heights = DiamondSquareGenerator.diamondSquare(this.size, TerrainObject.rough, TerrainObject.seed);

        Vector3[] vertices = this.mesh.vertices;
        Vector3[] tempVertices = new Vector3[vertices.Length];
        Color[] colors = new Color[vertices.Length];

        float minHeight = Mathf.Infinity;
        int minHeightVertexIndex = 0;

        int colliderTotalSize = totalSize / this.colliderScale;
        Vector3[] colliderVertices = this.colliderMesh.vertices;

        // Add diamond square calculated heights to the vertices
        for (int index = 0, z = 0; z < totalSize; z++)
        {
            for (int x = 0; x < totalSize; x++)
            {
                float height = heights[x, z];
                vertices[index].y = height;

                tempVertices[index] = vertices[index];

                colors[index] = this.getColorFromHeight(height);

                // part for calculating minimum height
                if (height < minHeight)
                {
                    minHeight = height;
                    minHeightVertexIndex = index;
                }

                index++;
            }
        }


        this.setColliderHeights(heights);

        this.tempVertices = tempVertices;
        this.tempDiffHeights = new float[totalSize, totalSize];

        // Set the mesh collider for hitting the terrain with the mouse
        //this.meshCollider.sharedMesh = this.mesh;

        this.updateMesh(vertices, colors);
        this.meshGameObject.SetActive(true);

        // Adds the buoy to the deepest water point (if there is water in the terrain)
        if (minHeight <= 0 && this.buoyPrefab != null)
        {
            this.buoyPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            this.buoy = Instantiate(this.buoyPrefab, this.mesh.vertices[minHeightVertexIndex] * 0.05f, Quaternion.Euler(-90, 0, 0));
        }
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

}
