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
    private float rough;

    /// <summary>
    /// The seed for the random numbers for the diamond square algorithm
    /// </summary>
    private int seed;

    private float minHeight;
    
    private float maxHeight;

    /// <summary>
    /// The mesh for the terrain
    /// </summary>
    private Mesh mesh;

    public Gradient gradient;

    /// <summary>
    /// Initial method
    /// /// </summary>
    void Start()
    {
        this.rough = 1.0f;
        this.seed = 100;
        this.createMesh();

        gameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }

    /// <summary>
    /// Update method that gets called once per frame
    /// </summary>
    void Update()
    {

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
        meshFilter.mesh = this.mesh;

        Vector3[] vertices = new Vector3[totalSize * totalSize];
        Color[] colors = new Color[vertices.Length];
        int[] triangles = new int[6 * ((totalSize - 1) * (totalSize - 1))];

        // Fill vertices and uvs
        for (int index = 0, z = 0; z < totalSize; z++)
        {
            for (int x = 0; x < totalSize; x++)
            {
                vertices[index] = new Vector3(x, 0, z);
                colors[index] = gradient.Evaluate(0);
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
        this.mesh.colors = colors;
        this.mesh.RecalculateNormals();
        this.mesh.RecalculateBounds();

        this.generateMeshHeights();
    }

    /// <summary>
    /// Sets the vertices heights of the mesh with heights calculated by the diamond square algorithm.
    /// </summary>
    void generateMeshHeights()
    {
        int totalSize = this.getTotalSize();
        float[,] heights = this.diamondSquare();
        this.eliminateNegativeValues(heights);

        Vector3[] vertices = this.mesh.vertices;
        Color[] colors = new Color[vertices.Length];
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

        for (int index = 0, z = 0; z < totalSize; z++)
        {
            for (int x = 0; x < totalSize; x++)
            {
                float height = Mathf.InverseLerp(this.minHeight, this.maxHeight, heights[x, z]);
                colors[index] = gradient.Evaluate(height);
                index++;
            }
        }

        this.mesh.vertices = vertices;
        this.mesh.colors = colors;
        this.mesh.RecalculateNormals();
        this.mesh.RecalculateBounds();
    }

    /// <summary>
    /// Returns a map of heights calculated with the diamond square algorithm.
    /// </summary>
    /// <returns>Map of heights</returns>
    private float[,] diamondSquare()
    {
        Random.InitState(this.seed);
        int totalSize = this.getTotalSize();
        int depth = totalSize - 1;
        float[,] map = new float[totalSize, totalSize];
        map[0, 0] = Random.value;
        map[0, depth] = Random.value;
        map[depth, 0] = Random.value;
        map[depth, depth] = Random.value;

        float average;
        float range = this.seed;
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

            range -= range * 0.5f * this.rough;
        }

        return map;
    }

    /// <summary>
    /// Sets all negative values in the given map to zero.
    /// </summary>
    /// <param name="map">map for eliminate negative values</param>
    private void eliminateNegativeValues(float[,] map)
    {
        int xSize = map.GetLength(0);
        int zSize = map.GetLength(1);
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                if (map[x, z] < 0)
                {
                    map[x, z] = 0;
                }
            }
        }
    }

    /// <summary>
    /// Sets the roughness and recalculate the mesh heights.
    /// </summary>
    /// <param name="rough">roughness</param>
    public void OnRoughChanged(float rough)
    {
        this.rough = rough;
        this.generateMeshHeights();
    }
    /// <summary>
    /// Sets the seed and recalculate the mesh heights.
    /// </summary>
    /// <param name="seed">seed</param>
    public void OnSeedChanged(float seed)
    {
        this.seed = (int)seed;
        this.generateMeshHeights();
    }
}