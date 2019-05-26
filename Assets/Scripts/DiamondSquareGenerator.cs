using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a mesh and random terrain using the Diamond Square algorithm
/// </summary>
public class DiamondSquareGenerator : MonoBehaviour
{

	private const int MaxMeshSize = 254;

    /// <summary>
    /// Material object of the mesh
    /// /// </summary>
    private Material material;

    /// <summary>
    /// The total size of the mesh
    /// </summary>
    private int size = 9;

    /// <summary>
    /// Size of the sub meshes
    /// </summary>
    private int subMeshSize = 100;

    /// <summary>
    /// Initial method
    /// /// </summary>
    void Awake()
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
    void createMesh(int size)
    {
		float[,] heights = this.diamondSquare(size, 5.0f, 20);

		int totalSize = (int)Mathf.Pow(2, size) + 1;
		int subSize = totalSize / MaxMeshSize;
        int totalSubMesh = totalSize / subSize;

        for (int xSub = 0; xSub < totalSubMesh; xSub++)
        {
            for (int ySub = 0; ySub < totalSubMesh; ySub++)
            {
                this.createSubMesh(xSub, ySub, subSize, totalSize, heights);
            }
        }
    }

    /// <summary>
    /// Creates a sub mesh 
    /// </summary>
    /// <param name="xSubMeshCount"></param>
    /// <param name="ySubMeshCount"></param>
    void createSubMesh(int xSubMeshCount, int ySubMeshCount, int subSize, int totalSize, float[,] heights)
    {
        GameObject meshGameObject = new GameObject();
        meshGameObject.transform.SetParent(gameObject.transform);
        meshGameObject.AddComponent<MeshRenderer>().material = this.material;
        meshGameObject.name = "SubMesh(" + xSubMeshCount + "," + ySubMeshCount + ")";
        MeshFilter meshFilter = meshGameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[subSize * subSize];
        Vector2[] uvs = new Vector2[subSize * subSize];
        int[] triangles = new int[6 * ((subSize - 1) * (subSize - 1))];

        int xOffset = xSubMeshCount;
        int yOffset = ySubMeshCount;

        int triangleIndex = 0;

		int testX;
		int testY;
		// Fill vertices and uvs
		for (int y = 0; y < subSize; y++)
        {
            for (int x = 0; x < subSize; x++)
            {
                int index = (y * subSize) + x;

				int globalX = x + (xSubMeshCount * subSize) - xOffset;
				int globalY = - y - (ySubMeshCount * subSize) + yOffset;
				testX = globalX;
				testY = globalY;

                vertices[index] = new Vector3(globalX, globalY, heights[globalX, -globalY]);
                uvs[index] = new Vector2(((float)(xSubMeshCount * subSize + x) / (float)totalSize),
                                         ((float)(ySubMeshCount * subSize + y) / (float)totalSize));
            }
        }

        // Fill triangles
		for (int y = 0; y < subSize - 1; y++)
        {
            for (int x = 0; x < subSize - 1; x++)
            {
                int index = (y * subSize) + x;

                int topLeft = index;
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + subSize;
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

	private float[,] diamondSquare(int size, float rough, int seed)
	{
		Random.InitState(seed);
		int depth = (int) Mathf.Pow(2, size);
		int totalSize = depth + 1;
		float[,] map = new float[totalSize, totalSize];
		map[0, 0] = Random.value;
		map[0, depth] = Random.value;
		map[depth, 0] = Random.value;
		map[depth, depth] = Random.value;

		float average;
		float range = 0.5f;
		int halfSide;
		
		for (int sideLength = totalSize - 1; sideLength > 1; sideLength /= 2)
		{
			halfSide = sideLength / 2;

			// Diamond step
			for (int x = 0; x < depth; x += sideLength)
			{
				for (int z = 0; z < depth; z += sideLength)
				{
					average = (map[x, z] + map[x + sideLength, z]
					+ map[x, z + sideLength] + map[x + sideLength, z + sideLength]) / 4.0f
					+ (Random.value * (range * 2.0f)) - range;
					map[x + halfSide, z + halfSide] = average;
				}
			}

			// Square step
			for (int x = 0; x < depth; x += halfSide)
			{
				for (int z = (x + halfSide) % sideLength; z < depth; z += sideLength)
				{
					average = (map[(x - halfSide + depth) % depth, z]
					+ map[(x + halfSide) % depth, z]
					+ map[x, (z + halfSide) % depth]
					+ map[x, (z - halfSide + depth) % depth]) / 4.0f
					+ (Random.value * (range * 2.0f)) - range;

					map[x, z] = average;

					if (x == 0)
					{
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
	
	//private void add
}