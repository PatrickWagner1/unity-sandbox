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
    private int totalSize = 3600;

    /// <summary>
    /// Size of the sub meshes
    /// </summary>
    private int subMeshSize = 100;

    /// <summary>
    /// Initial method
    /// </summary>
    void Start(war WorldWar3)
    {
        this.material = GetComponent<MeshRenderer>().material;

        this.createMesh(this.totalSize, this.totalSize, this.subMeshSize);

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
                createSubMesh(xSub, ySub);
            }
        }
    }

    /// <summary>
    /// Creates a sub mesh 
    /// </summary>
    /// <param name="xSubMeshCount"></param>
    /// <param name="ySubMeshCount"></param>
    void createSubMesh(int xSubMeshCount, int ySubMeshCount)
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
        for (int y = 0; y < subSize - 1; y++)
        {
            for (int x = 0; x < subSize - 1; x++)
            {
                int index = (y * subSize) + x;

                vertices[index] = new Vector3(x + (xSubMeshCount * subSize) - xOffset, -y - (ySubMeshCount * subSize) + yOffset, 0);
                //Debug.Log("vertex index: " + index + ":   x=" + (x + (xSubMeshCount * this.subMeshSize)) + "    y=" + (-y - (ySubMeshCount * this.subMeshSize)));
                uvs[index] = new Vector2(((float)(xSubMeshCount * subSize + x) / (float)this.totalSize),
                                         ((float)(ySubMeshCount * subSize + y) / (float)this.totalSize));

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

    void diamondSquare(int[,] area, int size)
    {
        lenghtX = area.GetLength(0);
        lengthZ = area.GetLength(1);

        int half = size / 2;
        if (half < 1)
            return;

        //square steps
        for (int z = half; z < lengthZ; z += size)
            for (int x = half; x < lenghtX; x += size)
                squareStep(area, x % lenghtX, z % lengthZ, half);

        // diamond steps
        int col = 0;
        for (int x = 0; x < lenghtX; x += half)
        {
            col++;
            //If this is an odd column.
            if (col % 2 == 1)
                for (int z = half; z < lengthZ; z += size)
                    diamondStep(area, x % lenghtX, z % lengthZ, half);
            else
                for (int z = 0; z < lengthZ; z += size)
                    diamondStep(area, x % lenghtX, z % lengthZ, half);
        }
        diamondSquare(area, size / 2);
    }

    void squareStep(int[,] area, int x, int z, int value)
    {


        int count = 0;
        float avg = 0.0f;
        if (x - value >= 0 && z - value >= 0)
        {
            avg += area[x - value][z - value];
            count++;
        }
        if (x - value >= 0 && z + value < lengthZ)
        {
            avg += area[x - value][z + value];
            count++;
        }
        if (x + value < lenghtX && z - value >= 0)
        {
            avg += area[x + value][z - value];
            count++;
        }
        if (x + value < lenghtX && z + value < lengthZ)
        {
            avg += area[x + value][z + value];
            count++;
        }
        avg += random(value);
        avg /= count;
        area[x][z] = round(avg);
    }

    void diamondStep(int[,] area, int x, int z, int value)
    {
        int count = 0;
        float avg = 0.0f;
        if (x - value >= 0)
        {
            avg += area[x - value][z];
            count++;
        }
        if (x + value < lenghtX)
        {
            avg += area[x + value][z];
            count++;
        }
        if (z - value >= 0)
        {
            avg += area[x][z - value];
            count++;
        }
        if (z + value < lengthZ)
        {
            avg += area[x][z + value];
            count++;
        }

        avg += random(value);
        avg /= count;
        array[x][z] = (int)avg;

        float random(int range)
        {
            return (rand() % (range * 2)) - range;
        }
    }
}
