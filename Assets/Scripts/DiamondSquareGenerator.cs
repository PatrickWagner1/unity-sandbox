using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a mesh and random terrain using the Diamond Square algorithm
/// </summary>
public class DiamondSquareGenerator : MonoBehaviour
{

    /// <summary>
    /// Material object of the mesh
    /// </summary>
    private Material material;

    /// <summary>
    /// The total size of the mesh
    /// </summary>
    private int size = 11;

    /// <summary>
    /// Size of the sub meshes
    /// </summary>
    private int subMeshSize = 100;

    /// <summary>
    /// Initial method
    /// </summary>
    void Start()
    {
        this.material = GetComponent<MeshRenderer>().material;

        this.createMesh(this.size);

        gameObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        gameObject.transform.Rotate(Vector3.right, 90);
    }

    /// <summary>
    /// Update method that gets called once per frame
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// Creates a mesh of sub meshes with the given width and height
    /// </summary>
    /// <param name="width">Mesh width</param>
    /// <param name="height">Mesh height</param>
    void createMesh(int width, int height, int subSize)
    {
        int xTotalSubMesh = width / subSize;
        int yTotalSubMesh = height / subSize;

        for (int xSub = 0; xSub < xTotalSubMesh; xSub++)
        {
            for (int ySub = 0; ySub < yTotalSubMesh; ySub++)
            {
                //createSubMesh(xSub, ySub, subSize);
            }
        }
    }

    /// <summary>
    /// Creates a mesh 
    /// </summary>
    /// <param name="size"></param>
    void createMesh(int size)
    {
		int totalSize = (int)Mathf.Pow(2, size) + 1;
		int squareSize = totalSize * totalSize;
        GameObject meshGameObject = new GameObject();
        meshGameObject.transform.SetParent(gameObject.transform);
        meshGameObject.AddComponent<MeshRenderer>().material = this.material;
        meshGameObject.name = "Mesh(" + totalSize + "," + totalSize + ")";
        MeshFilter meshFilter = meshGameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[squareSize];
        Vector2[] uvs = new Vector2[squareSize];
        int[] triangles = new int[6 * ((totalSize - 1) * (totalSize - 1))];

        int triangleIndex = 0;
        for (int y = 0; y < totalSize - 1; y++)
        {
            for (int x = 0; x < totalSize - 1; x++)
            {
                int index = (y * (totalSize - 1)) + x;

                vertices[index] = new Vector3(x, - y, 0);
                uvs[index] = new Vector2(((float)(totalSize + x) / (float)totalSize),
                                         ((float)(totalSize + y) / (float)totalSize));

                int topLeft = index;
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + totalSize - 1;
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
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    // Data container for heights of a terrain
    private TerrainData data;
	// Size of the sides of a terrain
	//private int size;
	// Flag to set random corner heights when terrain is reset
	private bool randomizeCornerValues = false;
	// 2D array of heights
	private float[,] heights;
	// Control variable to determine smoothness of heights
	private float roughness = 0.8f;

	/// <summary>
	/// Getters / Setters for the roughness value.
	/// </summary>
	public float Roughness {
		get { return roughness; }
		set { roughness = Mathf.Clamp(value, 0.001f, 1.999f); }
	}

	/// <summary>
	/// Used for initialization
	/// </summary>
/* 	private void Awake() {
        	data = transform.GetComponent<TerrainCollider>().terrainData;
        	size = data.heightmapWidth;
		
		SetSeed((int)Random.value);
        	Reset();

		return;
	} */
	
	/// <summary>
	/// Sets the seed of the random number generator
	/// </summary>
	/// <param name="seed">A value that influences the random number generator</param>
	public void SetSeed(int seed) {
		Random.InitState(seed);

		return;
	}

	/// <summary>
	/// Flips the value of the randomizeCornerValues flag
	/// </summary>
	public void ToggleRandomizeCornerValues() {
		randomizeCornerValues = !randomizeCornerValues;

		return;
	}

	/// <summary>
	/// Resets the values of the terrain. If randomizeCornerValues is true then the
	/// corner heights will be randomized, else it will be flat.
	/// </summary>
	public void Reset() {
		heights = new float[size, size];

		// If the corners need to be randomized
		if (randomizeCornerValues) {
			heights[0, 0] = Random.value;
			heights[size - 1, 0] = Random.value;
			heights[0, size - 1] = Random.value;
			heights[size - 1, size - 1] = Random.value;
		}

		// Update the terrain data
		data.SetHeights(0, 0, heights);

		return;
	}

	/// <summary>
	/// Executes the DiamondSquare algorithm on the terrain.
	/// </summary>
	public void ExecuteDiamondSquare() {
		heights = new float[size, size];
		float average, range = 0.5f;
		int sideLength, halfSide, x, y;

		// While the side length is greater than 1
		for (sideLength = size - 1; sideLength > 1; sideLength /= 2) {
			halfSide = sideLength / 2;

			// Run Diamond Step
			for (x = 0; x < size - 1; x += sideLength) {
				for (y = 0; y < size - 1; y += sideLength) {
					// Get the average of the corners
					average = heights[x, y];
					average += heights[x + sideLength, y];
					average += heights[x, y + sideLength];
					average += heights[x + sideLength, y + sideLength];
					average /= 4.0f;

					// Offset by a random value
					average += (Random.value * (range * 2.0f)) - range;
					heights[x + halfSide, y + halfSide] = average;
				}
			}

			// Run Square Step
			for (x = 0; x < size - 1; x += halfSide) {
				for (y = (x + halfSide) % sideLength; y < size - 1; y += sideLength) {
					// Get the average of the corners
					average = heights[(x - halfSide + size - 1) % (size - 1), y];
					average += heights[(x + halfSide) % (size - 1), y];
					average += heights[x, (y + halfSide) % (size - 1)];
					average += heights[x, (y - halfSide + size - 1) % (size - 1)];
					average /= 4.0f;

					// Offset by a random value
					average += (Random.value * (range * 2.0f)) - range;

					// Set the height value to be the calculated average
					heights[x, y] = average;

					// Set the height on the opposite edge if this is
					// an edge piece
					if (x == 0) {
						heights[size - 1, y] = average;
					}

					if (y == 0) {
						heights[x, size - 1] = average;
					}
				}
			}

			// Lower the random value range
			range -= range * 0.5f * roughness;
		}

		// Update the terrain heights
		data.SetHeights(0, 0, heights);

		return;
	}

	/// <summary>
	/// Returns the amount of vertices to skip using the given depth.
	/// </summary>
	/// <param name="depth">The vertice detail depth on the height array</param>
	/// <returns>Amount of vertices to skip</returns>
	public int GetStepSize(int depth) {
		// Return an invalid step size if the depth is invalid
		if (!ValidateDepth(depth)) {
			return -1;
		}

		// Return the amount of vertices to skip
		return (int)((size - 1) / Mathf.Pow(2, (depth - 1)));
	}

	/// <summary>
	/// Returns the maximum depth for this terrain's size.
	/// </summary>
	/// <returns>Maximum depth for this terrain</returns>
	public int MaxDepth() {
		// 0.69314718056f = Natural Log of 2
		return (int)((Mathf.Log(size - 1) / 0.69314718056f) + 1);
	}

	/// <summary>
	/// Returns false if the depth is above zero and below maximum depth, true otheriwse
	/// </summary>
	/// <param name="depth">The vertice detail depth on the height array</param>
	/// <returns></returns>
	private bool ValidateDepth(int depth) {
		if (depth > 0 && depth <= MaxDepth()) {
			return true;
		}

		return false;
    }
}