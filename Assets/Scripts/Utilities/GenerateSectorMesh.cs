using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateSectorMesh : MonoBehaviour
{
    public float radius = 5f;        // Radius of the circle
    public float angle = 60f;        // Angle of the sector in degrees
    public int segments = 20;        // Number of segments in the circle
    public bool centerSector = true; // Centers the sector so the middle always points forward

    private void Update()
    {
        if (centerSector)
        {
            transform.localRotation = Quaternion.Euler(0f, angle/2f, 0f);
        }
    }

    public void RenderSector()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        // Center of the circle
        vertices[0] = Vector3.zero;

        // Calculate vertices of the circle sector
        float angleIncrement = angle / segments;
        for (int i = 1; i <= segments + 1; i++)
        {
            float theta = Mathf.Deg2Rad * (i - 1) * angleIncrement;
            vertices[i] = new Vector3(radius * Mathf.Cos(theta), 0f, radius * Mathf.Sin(theta));
        }

        // Create triangles
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2;
            triangles[i * 3 + 2] = i + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Recalculate normals to ensure proper lighting
        mesh.RecalculateNormals();
    }
}
