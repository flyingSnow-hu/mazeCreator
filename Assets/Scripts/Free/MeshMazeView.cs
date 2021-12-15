using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshMazeView : MonoBehaviour
{

    public void Draw(IMazeModel mazeModel)
    {
        Vector2[] points = mazeModel.GetPoints();

        var result = new Delaunay().GetResult(points);
        var mesh = new Mesh();
        mesh.vertices = points.Select(v2=>new Vector3(v2.x, v2.y, 0)).ToArray();

        var triangles = new int[result.Triangles.Count * 3];
        for (int i = 0; i < result.Triangles.Count; i++)
        {
            var triangle = result.Triangles[i];
            triangles[i*3] = triangle.Item1;
            var p1 = mesh.vertices[triangle.Item1];
            var p2 = mesh.vertices[triangle.Item2];
            var p3 = mesh.vertices[triangle.Item3];
            var edge1 = p2 - p1;
            var edge2 = p3 - p1;
            // 把所有三角形调整到正面
            if (Vector3.Cross(edge1, edge2).z < 0)
            {
                triangles[i*3 + 1] = triangle.Item2;
                triangles[i*3 + 2] = triangle.Item3;
            }else
            {
                triangles[i*3 + 2] = triangle.Item2;
                triangles[i*3 + 1] = triangle.Item3;
            }
        }
        mesh.triangles = triangles;
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
