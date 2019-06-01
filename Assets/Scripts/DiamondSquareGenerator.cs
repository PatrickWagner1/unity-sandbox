using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generates a mesh and random terrain using the Diamond Square algorithm
/// </summary>
public class DiamondSquareGenerator : MonoBehaviour {

    /// <summary>
    /// The total size of the mesh
    /// </summary>
    private int size = 10;

    /// <summary>
    /// Initial method
    /// /// </summary>
    void Awake () {

        this.createMesh (this.size, 1.0f, 15);

        gameObject.transform.localScale = new Vector3 (0.05f, 0.05f, 0.05f);
    }

    /// <summary>
    /// Update method that gets called once per frame
    /// </summary>
    void Update () {

    }

    /// <summary>
    /// Creates a mesh with the given size. The heights will be calculated with the diamond square algorithm with the given rough and seed.
    /// </summary>
    /// <param name="size"></param>
    /// <param name="rough"></param>
    /// <param name="seed"></param>
    void createMesh (int size, float rough, int seed) {
        int totalSize = (int) Mathf.Pow (2, size) + 1;
        float[, ] heights = this.diamondSquare (size, rough, seed);
        this.eliminateNegativeValues (heights);

        GameObject meshGameObject = new GameObject ();
        meshGameObject.transform.SetParent (gameObject.transform);
        meshGameObject.AddComponent<MeshRenderer> ().material = GetComponent<MeshRenderer>().material;
        meshGameObject.name = "SubMesh(" + totalSize + "x" + totalSize + ")";
        MeshFilter meshFilter = meshGameObject.AddComponent<MeshFilter> ();
        Mesh mesh = new Mesh ();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[totalSize * totalSize];
        Vector2[] uvs = new Vector2[totalSize * totalSize];
        int[] triangles = new int[6 * ((totalSize - 1) * (totalSize - 1))];

        int triangleIndex = 0;

        // Fill vertices and uvs
        for (int z = 0; z < totalSize; z++) {
            for (int x = 0; x < totalSize; x++) {
                int index = (z * totalSize) + x;

                float y = heights[x, z];
                vertices[index] = new Vector3 (x, y, z);
                uvs[index] = new Vector2 (((float) x),
                    ((float) z));
            }
        }

        // Fill triangles
        for (int z = 0; z < totalSize - 1; z++) {
            for (int x = 0; x < totalSize - 1; x++) {
                int index = (z * totalSize) + x;

                int topLeft = index;
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + totalSize;
                int bottomRight = bottomLeft + 1;

                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = topRight;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = topRight;
                triangles[triangleIndex++] = bottomRight;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals ();
        mesh.RecalculateBounds ();
    }

    private float[, ] diamondSquare (int size, float rough, int seed) {
        Random.InitState ((int) (Random.value * 250.0f));
        int depth = (int) Mathf.Pow (2, size);
        int totalSize = depth + 1;
        float[, ] map = new float[totalSize, totalSize];
        map[0, 0] = Random.value;
        map[0, depth] = Random.value;
        map[depth, 0] = Random.value;
        map[depth, depth] = Random.value;

        float average;
        float range = 500.0f;
        int halfSide;

        for (int sideLength = totalSize - 1; sideLength > 1; sideLength /= 2) {
            halfSide = sideLength / 2;

            // Diamond step
            for (int x = 0; x < depth; x += sideLength) {
                for (int z = 0; z < depth; z += sideLength) {
                    average = (map[x, z] + map[x + sideLength, z] +
                            map[x, z + sideLength] + map[x + sideLength, z + sideLength]) / 4.0f +
                        (Random.value * (range * 2.0f)) - range;
                    map[x + halfSide, z + halfSide] = average;
                }
            }

            // Square step
            for (int x = 0; x < depth; x += halfSide) {
                for (int z = (x + halfSide) % sideLength; z < depth; z += sideLength) {
                    average = (map[(x - halfSide + depth) % depth, z] +
                            map[(x + halfSide) % depth, z] +
                            map[x, (z + halfSide) % depth] +
                            map[x, (z - halfSide + depth) % depth]) / 4.0f +
                        (Random.value * (range * 2.0f)) - range;

                    map[x, z] = average;

                    if (x == 0) {
                        map[depth, z] = average;
                    }

                    if (z == 0) {
                        map[x, depth] = average;
                    }
                }
            }

            range -= range * 0.5f * rough;
        }

        return map;
    }

    private void eliminateNegativeValues (float[, ] map) {
        int xSize = map.GetLength (0);
        int zSize = map.GetLength (1);
        for (int x = 0; x < xSize; x++) {
            for (int z = 0; z < zSize; z++) {
                if (map[x, z] < 0) {
                    map[x, z] = 0;
                }
            }
        }
    }

    public void OnValueChanged (float value) {
        this.createMesh (this.size, 1.0f, (int) value);
    }
}