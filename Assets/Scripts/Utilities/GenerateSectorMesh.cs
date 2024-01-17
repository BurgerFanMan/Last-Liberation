using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateSectorMesh : MonoBehaviour
{
    public float innerRadius = 1f;
    public float radius = 5f;        // Radius of the circle
    public float angle = 60f;        // Angle of the sector in degrees
    public float centerAngle = 0f;
    public int segments = 20;        // Resolution basically, the number of sides of the polygon. Higher means more circular but more vertices to generate.
    public bool centerSector = true; // Centers the sector so the middle always points forward

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

        if (centerSector)
        {
            transform.localRotation = Quaternion.Euler(0f, (angle / 2f) + centerAngle, 0f);
        }
    }
}
