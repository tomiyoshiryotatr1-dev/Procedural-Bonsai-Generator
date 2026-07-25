using UnityEngine;
using System.Collections.Generic;

public class bonsaileaf : MonoBehaviour
{
    public int leafCount = 100;

    public float leafLength = 0.8f;

    public float leafWidth = 0.02f; //xycodinate

    public Material leafMaterial;

    // 葉を生やす中心位置
    public Vector3 center = new Vector3(1, 5, 1); //Vector3.zero;
    public void Start_leaf(List<Vector3> leaf_point)
    {
        foreach (Vector3 point in leaf_point)
        {
            center = point;
            GenerateLeaves();
        }
    }

    public void GenerateLeaves()
    {
        leafCount = Random.Range(50, 70);
        for(int i = 0; i < leafCount; i++)
        {
            CreateLeaf();
        }
    }

    void CreateLeaf()
    {
        GameObject leaf = new GameObject("Leaf");

        MeshFilter mf = leaf.AddComponent<MeshFilter>();
        MeshRenderer mr = leaf.AddComponent<MeshRenderer>();

       
        // GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        // if(leafMaterial != null)
        //     leaf.GetComponent<MeshRenderer>().material = leafMaterial;

        // 半球方向へランダム
        // Vector3 dir = Random.onUnitSphere;

        
        // float theta =
        //     Random.Range(0f,60f) * Mathf.Deg2Rad;

        // float phi =
        //     Random.Range(0f,360f) * Mathf.Deg2Rad;
        // Vector3 dir =new Vector3(
        //     Mathf.Sin(theta)*Mathf.Cos(phi),
        //     Mathf.Cos(theta),
        //     Mathf.Sin(theta)*Mathf.Sin(phi));
        // if(dir.y < 0)
        //     dir.y *= -1;

        // dir.Normalize();

        float theta = Random.Range(0f, 50f) * Mathf.Deg2Rad;
        float phi   = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        Vector3 dir = new Vector3(
            Mathf.Sin(theta) * Mathf.Cos(phi),
            Mathf.Cos(theta),
            Mathf.Sin(theta) * Mathf.Sin(phi)
        ).normalized;

        float length = Random.Range(0.5f,0.7f);

        float width = 0.01f;

        // leafLength = length;
        mf.mesh = CreateCone(length, width, 10);

        mr.material = leafMaterial;

        // 根元
        leaf.transform.position = center;

        // 円錐のY軸をdirへ向ける
        leaf.transform.up = dir;

        // 親にする
        leaf.transform.parent = transform;

        // // 中心から半分だけ進めた位置
        // leaf.transform.position = center + dir * leafLength * 0.5f;

        // // CylinderのY軸を葉の方向に合わせる
        // leaf.transform.up = dir;

        // leaf.transform.localScale =
        //     new Vector3(
        //         leafWidth,
        //         leafLength * 0.5f,
        //         leafWidth
        //     );

        // // 葉をまとめる
        // leaf.transform.parent = transform;
    }


    Mesh CreateCone(float length, float radius, int segments)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // -----------------------------
        // リング位置
        // 0 = 根元
        // 1 = 中間（太さ一定）
        // 2 = 先端
        // -----------------------------

        float midY = length * 0.5f;




        // float bend = Random.Range(0.05f, 0.15f);
        // float cylinderLength = length * 0.5f;
        // float t = y / cylinderLength;

        // float offset = bend * t * t;

        // float bend = Random.Range(0.05f, 0.15f);

        // for(int i=0;i<segments;i++)
        // {
        //     float angle = Mathf.PI * 2 * i / segments;

        //     vertices.Add(
        //         new Vector3(
        //             Mathf.Cos(angle) * radius,
        //             0,
        //             Mathf.Sin(angle) * radius
        //         )
        //     );
        // }
        
        // float offset = bend;

        // for(int i=0;i<segments;i++)
        // {
        //     float angle = Mathf.PI * 2 * i / segments;

        //     vertices.Add(
        //         new Vector3(
        //             Mathf.Cos(angle) * radius,
        //             midY,
        //             Mathf.Sin(angle) * radius - offset
        //         )
        //     );
        // }
        // 根元リング
        for(int i=0;i<segments;i++)
        {
            float angle = Mathf.PI * 2 * i / segments;

            vertices.Add(new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            ));
        }

        // 中間リング（同じ半径）
        for(int i=0;i<segments;i++)
        {
            float angle = Mathf.PI * 2 * i / segments;

            vertices.Add(new Vector3(
                Mathf.Cos(angle) * radius,
                midY,
                Mathf.Sin(angle) * radius
            ));
        }

        // 先端
        vertices.Add(new Vector3(0, length, 0));
        int tip = vertices.Count - 1;

        //-----------------------------------
        // 円柱部分
        //-----------------------------------

        for(int i=0;i<segments;i++)
        {
            int next = (i+1)%segments;

            int b0 = i;
            int b1 = next;

            int t0 = i + segments;
            int t1 = next + segments;

            triangles.Add(b0);
            triangles.Add(t0);
            triangles.Add(b1);

            triangles.Add(b1);
            triangles.Add(t0);
            triangles.Add(t1);
        }

        //-----------------------------------
        // 円錐部分
        //-----------------------------------

        for(int i=0;i<segments;i++)
        {
            int next = (i+1)%segments;

            triangles.Add(tip);
            
            triangles.Add(next + segments);
            triangles.Add(i + segments);
        }

        //-----------------------------------
        // 底面
        //-----------------------------------

        vertices.Add(Vector3.zero);
        int center = vertices.Count - 1;

        for(int i=0;i<segments;i++)
        {
            int next = (i+1)%segments;

            triangles.Add(center);
            triangles.Add(next);
            triangles.Add(i);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    // Mesh CreateCone(float length, float radius, int segments)
    // {
    //     Mesh mesh = new Mesh();

    //     List<Vector3> vertices = new List<Vector3>();
    //     List<int> triangles = new List<int>();

    //     // 先端
    //     vertices.Add(new Vector3(0, length, 0));

    //     // 根元の円
    //     for (int i = 0; i < segments; i++)
    //     {
    //         float angle = i * Mathf.PI * 2 / segments;

    //         vertices.Add(
    //             new Vector3(
    //                 Mathf.Cos(angle) * radius,
    //                 0,
    //                 Mathf.Sin(angle) * radius
    //             )
    //         );
    //     }

    //     // 底面中心
    //     vertices.Add(Vector3.zero);

    //     int center = vertices.Count - 1;
    //     for (int i = 0; i < segments; i++)
    //     {
    //         int next = (i + 1) % segments;

    //         triangles.Add(0);          // 先端
    //         triangles.Add(i + 1);
    //         triangles.Add(next + 1);
    //     }

    //     for (int i = 0; i < segments; i++)
    //     {
    //         int next = (i + 1) % segments;

    //         triangles.Add(center);
    //         triangles.Add(next + 1);
    //         triangles.Add(i + 1);
    //     }

    //     mesh.vertices = vertices.ToArray();
    //     mesh.triangles = triangles.ToArray();

    //     mesh.RecalculateNormals();

    //     return mesh;
    // }
}