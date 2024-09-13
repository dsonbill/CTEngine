using System.Linq;
using UnityEngine;

public class InvertedMesh : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        // Reverse triangles
        mesh.triangles = mesh.triangles.Reverse().ToArray();

        // Invert normals
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }
        mesh.normals = normals;

        // You might also need to recalculate the mesh tangents
        mesh.RecalculateTangents();
    }
}
