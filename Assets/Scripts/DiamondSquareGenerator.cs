using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a mesh and random terrain using the Diamond Square algorithm
/// </summary>
public class DiamondSquareGenerator : MonoBehaviour {

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
    void Start () {
        this.material = GetComponent<MeshRenderer>().material;

        this.createMesh(this.totalSize, this.totalSize);

        gameObject.transform.localScale = new Vector3(0.001f,0.001f,0.001f);
        gameObject.transform.Rotate(Vector3.right, 90);
    }

    /// <summary>
    /// Update method that gets called once per frame
    /// </summary>
    void Update () {
		
	}

    /// <summary>
    /// Creates a mesh of sub meshes with the given width and height
    /// </summary>
    /// <param name="width">Mesh width</param>
    /// <param name="height">Mesh height</param>
    void createMesh(int width, int height)
    {
        int xTotalSubMesh = width / this.subMeshSize;
        int yTotalSubMesh = height / this.subMeshSize;
        
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
        meshGameObject.name = "SubMesh("+xSubMeshCount+","+ySubMeshCount+")";
        MeshFilter meshFilter = meshGameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[this.subMeshSize * this.subMeshSize];
        Vector2[] uvs = new Vector2[this.subMeshSize * this.subMeshSize];
        int[] triangles = new int[6 * ((this.subMeshSize - 1) * (this.subMeshSize - 1))];
        
        int xOffset = xSubMeshCount;
        int yOffset = ySubMeshCount;

        int triangleIndex = 0;
        for (int y = 0; y < this.subMeshSize - 1; y++)
        {
            for (int x = 0; x < this.subMeshSize - 1; x++)
            {
                int index = (y * this.subMeshSize) + x;

                vertices[index] = new Vector3(x + (xSubMeshCount * this.subMeshSize) - xOffset, - y - (ySubMeshCount * this.subMeshSize) + yOffset, 0);
                //Debug.Log("vertex index: " + index + ":   x=" + (x + (xSubMeshCount * this.subMeshSize)) + "    y=" + (-y - (ySubMeshCount * this.subMeshSize)));
                uvs[index] = new Vector2(((float)(xSubMeshCount * this.subMeshSize + x) / (float)this.totalSize),
                                         ((float)(ySubMeshCount * this.subMeshSize + y) / (float)this.totalSize));
                
                int topLeft = index;
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + this.subMeshSize;
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
}
