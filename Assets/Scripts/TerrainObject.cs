using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainObject : MonoBehaviour
{
    /// <summary>
    /// The exponent for the side length of the mesh
    /// </summary>
    public int size = 10;

    /// <summary>
    /// The roughness for the diamond square algorithm
    /// </summary>
    public static float rough = 1.0f;

    /// <summary>
    /// The seed for the random numbers for the diamond square algorithm
    /// </summary>
    public static int seed = 200;

    public static bool showContourLines = true;

    public float maxHeight = 100;

    /// <summary>
    /// The mesh for the terrain
    /// </summary>
    public Mesh mesh;

    private MeshCollider meshCollider;

    public Vector3[] tempVertices;

    public float[] tempDiffHeights;

    public Gradient gradient;

    public Color waterColor;

    public GameObject buoyPrefab;

    public GameObject buoy;

    /// <summary>
    /// Initial method
    /// /// </summary>
    void Awake()
    {
        Debug.Log("Awake");
        Toggle showContourLinesToggle = GameObject.Find("ShowContourLinesToggle").GetComponent<Toggle>();
        showContourLinesToggle.isOn = TerrainObject.showContourLines;
        SceneInteraction.changeContourLines(TerrainObject.showContourLines);

        this.createMesh();

        gameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }

    /// <summary>
    /// Updates the mesh by eliminating the negatives and recalculating normals and bounds
    /// </summary>
    public void updateMesh(Vector3[] vertices, Color[] colors)
    {
        this.mesh.vertices = vertices;
        this.mesh.colors = colors;

        this.eliminateNegativeHeights();

        this.mesh.RecalculateNormals();
        this.mesh.RecalculateBounds();
    }

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
    /// Creates a flat mesh.
    /// </summary>
    void createMesh()
    {
        int totalSize = DiamondSquareGenerator.getTotalSize(this.size);

        GameObject meshGameObject = new GameObject();
        meshGameObject.transform.SetParent(gameObject.transform);
        meshGameObject.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
        meshGameObject.name = "TerrainMesh";
        meshGameObject.layer = 8;

        MeshFilter meshFilter = meshGameObject.AddComponent<MeshFilter>();
        this.mesh = new Mesh();
        this.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshFilter.sharedMesh = this.mesh;
        this.meshCollider = meshGameObject.AddComponent<MeshCollider>();

        Vector3[] vertices = new Vector3[totalSize * totalSize];
        int[] triangles = new int[6 * ((totalSize - 1) * (totalSize - 1))];

        // Fill vertices and uvs
        for (int index = 0, z = 0; z < totalSize; z++)
        {
            for (int x = 0; x < totalSize; x++)
            {
                vertices[index] = new Vector3(x, 0, z);

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

        this.generateMeshHeights();
    }

    /// <summary>
    /// Sets the vertices heights of the mesh with heights calculated by the diamond square algorithm.
    /// </summary>
    void generateMeshHeights()
    {
        int totalSize = DiamondSquareGenerator.getTotalSize(this.size);
        float[,] heights = DiamondSquareGenerator.diamondSquare(this.size, TerrainObject.rough, TerrainObject.seed);

        Vector3[] vertices = this.mesh.vertices;
        Vector3[] tempVertices = new Vector3[vertices.Length];
        Color[] colors = new Color[vertices.Length];

        float minHeight = Mathf.Infinity;
        int minHeightVertexIndex = 0;

        for (int index = 0, z = 0; z < totalSize; z++)
        {
            for (int x = 0; x < totalSize; x++)
            {
                float height = heights[x, z];
                vertices[index].y = height;

                tempVertices[index] = vertices[index];

                colors[index] = this.getColorFromHeight(height);

                if (height < minHeight)
                {
                    minHeight = height;
                    minHeightVertexIndex = index;
                }

                index++;
            }
        }

        this.tempVertices = tempVertices;
        this.tempDiffHeights = new float[tempVertices.Length];

        this.meshCollider.sharedMesh = this.mesh;

        this.updateMesh(vertices, colors);
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
