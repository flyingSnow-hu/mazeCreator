using System;
using System.Collections.Generic;
using UnityEngine;

using Vertex = System.Int32;
using Edge = System.Tuple<int, int>;
using Triangle = System.Tuple<int, int, int>;

public class UDelaunayResult
{
    /// <summary>
    /// 三角形列表
    /// </summary>
    public List<Triangle> Triangles;
    /// <summary>
    /// 边列表
    /// </summary>
    public List<Edge> Edges;
}

public class Delaunay
{
    private Vector2[] _vertexes;

    public UDelaunayResult GetResult(Vector2[] vertexList) 
    {
        this._vertexes = new Vector2[vertexList.Length + 4];
        vertexList.CopyTo(this._vertexes, 0);

        UDelaunayResult result = new UDelaunayResult();
        List<Triangle> triangles = new List<Triangle>();
        List<Edge> edges = new List<Edge>();
        //> 找到最小和最大的点
        float minX=0, minY=0, maxX=0, maxY=0; 
        foreach (var v in _vertexes) 
        {
            if (v.x < minX) minX = v.x;
            if (v.y < minY) minY = v.y;
            if (v.x > maxX) maxX = v.x;
            if (v.y > maxY) maxY = v.y;
        } 
        minX -= 10;
        minY -= 10;
        maxX += 10;
        maxY += 10;

        //> 创建超级三角形
        Vector2 leftUp = new Vector2(minX,maxY);
        Vector2 rightUp = new Vector2(maxX, maxY);
        Vector2 rightDown = new Vector2(maxX, minY);
        Vector2 leftDown = new Vector2(minX, minY);
        _vertexes[_vertexes.Length - 4] = leftUp;
        _vertexes[_vertexes.Length - 3] = rightUp;
        _vertexes[_vertexes.Length - 2] = rightDown;
        _vertexes[_vertexes.Length - 1] = leftDown;

        //> 为了确保所有的点都包含在三角形内
        //> 这里使用了两个超级三角形拼成的矩形
        triangles.Add(new Triangle(_vertexes.Length - 4, _vertexes.Length - 3, _vertexes.Length - 2));
        triangles.Add(new Triangle(_vertexes.Length - 4, _vertexes.Length - 2, _vertexes.Length - 1));

        for (Vertex i = 0; i < _vertexes.Length; i++)
        {            
            AddVertex(triangles,i);
        }

        for (int i = triangles.Count - 1; i >= 0 ; i--)
        {
            if (ContainSuperVertex(triangles[i]))
            {
                triangles.RemoveAt(i);
            }
        }
        result.Triangles = triangles;
        result.Edges = GetEdgeFromTriangles(triangles);
        return result;
    }

    private List<Edge> GetEdgeFromTriangles(List<Triangle> _triangles)
    {
        List<Edge> result = new List<Edge>();

        for (int i = 0; i < _triangles.Count; i++) 
        {
            var t0 = _triangles[i];
            AddEdge(t0.Item1, t0.Item2, result);
            AddEdge(t0.Item1, t0.Item3, result);
            AddEdge(t0.Item2, t0.Item3, result);
        }
        return result;
    }

    private void AddEdge(Vertex from, Vertex to, List<Edge> result) 
    {
        for (int i = 0; i < result.Count; i++) 
        {
            var v = result[i];
            if (IsDoubleSide(from, to, v.Item1, v.Item2)) return;  
        }

        result.Add(new Edge(from, to));
    }

    //> 三角形内是否包含超级三角形中的任意一点
    private bool ContainSuperVertex(Triangle _triangle) 
    {
        var minSuperIndex = _vertexes.Length - 4;
        return _triangle.Item1 >= minSuperIndex 
            || _triangle.Item2 >= minSuperIndex 
            || _triangle.Item3 >= minSuperIndex;
    }

    private void AddVertex(List<Triangle> triangles, Vertex vertex) 
    {
        Vector2 pos = _vertexes[vertex];
        List<Edge> edges = new List<Edge>();

        for (int i = 0; i < triangles.Count; i++) 
        {
            var t0 = triangles[i];
            if (Inside(t0, pos))
            {
                triangles.RemoveAt(i--);
                edges.Add(new Edge(t0.Item1, t0.Item2));
                edges.Add(new Edge(t0.Item1, t0.Item3));
                edges.Add(new Edge(t0.Item2, t0.Item3));
            }
        }

        for (int i = 0; i < edges.Count; i++) 
        {
            var ei = edges[i];
            for (int n = i+1; n < edges.Count; n++) 
            {
                var en = edges[n];
                if (IsDoubleSide(ei.Item1, ei.Item2, en.Item1, en.Item2)) 
                {
                    edges.RemoveAt(n);
                    edges.RemoveAt(i--);
                    break;
                }
            }
        }

        foreach (var v in edges) 
        {
            triangles.Add(new Triangle(v.Item1, v.Item2, vertex));
        }
    }

    //> 是否是双边
    private bool IsDoubleSide(Vertex a0,Vertex a1,Vertex b0,Vertex b1)
    {
        var _a0 = _vertexes[a0];
        var _a1 = _vertexes[a1];
        var _b0 = _vertexes[b0];
        var _b1 = _vertexes[b1];
        float x = 0, y = 0;
        x += _a0.x - _b0.x;
        y += _a0.y - _b0.y;
        x += _a1.x - _b1.x;
        y += _a1.y - _b1.y;
        return x == 0 && y == 0;
    }

    private bool Inside(Triangle triangle, Vector2 _newVertex) 
    {
        //> 求三角形任意两边垂直平分线
        Vector2[] t01 = GetBisector(_vertexes[triangle.Item1], _vertexes[triangle.Item2]);
        Vector2[] t02 = GetBisector(_vertexes[triangle.Item1], _vertexes[triangle.Item3]);

        //> 求圆心
        Vector2 circelPoint = LineIntersection(t01[0],t01[1],t02[0],t02[1]);

        //> 求圆心半径
        float r = Vector2.Distance(_vertexes[triangle.Item1],circelPoint);
        float r2 = Vector2.Distance(_newVertex, circelPoint);

        //> 检测是否在圆内 
        return r2 <= r;
    }

    //> 获取任意线段的交点
    private static Vector2 LineIntersection(Vector3 p0, Vector3 p1, Vector3 e0, Vector3 e1)
    { 
        return get_line_intersection(p0.x, p0.y, p1.x, p1.y, e0.x, e0.y, e1.x, e1.y);         
    }

    private static Vector2 get_line_intersection(float p0_x, float p0_y, float p1_x, float p1_y,
                                                 float p2_x, float p2_y, float p3_x, float p3_y)
    { 
        float s02_x, s02_y, s10_x, s10_y, s32_x, s32_y, t_numer, denom, t;
        s10_x = p1_x - p0_x;
        s10_y = p1_y - p0_y;
        s32_x = p3_x - p2_x;
        s32_y = p3_y - p2_y;

        denom = s10_x * s32_y - s32_x * s10_y;  
        s02_x = p0_x - p2_x;
        s02_y = p0_y - p2_y;
        t_numer = s32_x * s02_y - s32_y * s02_x; 
        t = t_numer / denom;
        Vector2 v;
        v.x = p0_x + (t * s10_x);
        v.y = p0_y + (t * s10_y); 
        return v;
    }

    //> 获取任意线段的垂直平分线
    private static Vector2[] GetBisector(Vector2 _a, Vector2 _b)
    {
        float rotate = Mathf.PI / 2;
        Vector2 a = _b - _a;
        _a -= a;
        _b += a;
        Vector2[] r = new Vector2[2];
        Vector2 half = _a + (_b - _a) / 2;
        r[0].x = _a.x * Mathf.Cos(rotate) - _a.y * Mathf.Sin(rotate);
        r[0].y = _a.x * Mathf.Sin(rotate) + _a.y * Mathf.Cos(rotate);
        r[1].x = _b.x * Mathf.Cos(rotate) - _b.y * Mathf.Sin(rotate);
        r[1].y = _b.x * Mathf.Sin(rotate) + _b.y * Mathf.Cos(rotate);
        r[0].x += half.x * (1 - Mathf.Cos(rotate)) + half.y * Mathf.Sin(rotate);
        r[0].y += half.y * (1 - Mathf.Cos(rotate)) - half.x * Mathf.Sin(rotate);
        r[1].x += half.x * (1 - Mathf.Cos(rotate)) + half.y * Mathf.Sin(rotate);
        r[1].y += half.y * (1 - Mathf.Cos(rotate)) - half.x * Mathf.Sin(rotate);
        return r;
    }
}