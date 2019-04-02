using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GridMesh : MonoBehaviour
{
    void Awake()
    {
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        var mesh = new Mesh();
        var verticies = new List<Vector3>();
        var indicies = new List<int>();

        var dist_x = 1.0f / (BoardCore.MAX_X + 1);
        var dist_y = 1.0f / (BoardCore.MAX_Y + 1);

        int index = 0;
        float y = 0.49f;
        for (int i = 0; i <= BoardCore.MAX_X + 1; i++)
        {
            verticies.Add(new Vector3(-0.5f + dist_x * i, y, -0.5f));
            verticies.Add(new Vector3(-0.5f + dist_x * i, y, 0.5f));
            indicies.Add(index++);
            indicies.Add(index++);
        }
        for (int i = 0; i <= BoardCore.MAX_Y + 1; i++)
        {
            verticies.Add(new Vector3(-0.5f, y, -0.5f + dist_y * i));
            verticies.Add(new Vector3(0.5f, y, -0.5f + dist_y * i));
            indicies.Add(index++);
            indicies.Add(index++);
        }

        mesh.vertices = verticies.ToArray();
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Lines, 0);
        filter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = Color.white;
    }
}
