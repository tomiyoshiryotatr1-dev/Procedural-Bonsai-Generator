using UnityEngine;
using System.Collections.Generic;

public class DlaBonsai2 : MonoBehaviour
{
     public int particleCount = 2000;

    public float spawnRadius = 10f;

    public float stepSize = 0.2f;

    public float stickDistance = 0.3f;

    public int maxStep = 300;

    public int nowCount = 0;

    public List<Vector3> leaf_point = new List<Vector3>();

    public Material barkMaterial; //?

    int origin_branch = 0;

    public Vector3 startPosition = new Vector3(1, 10, 1);



    // public GameObject spherePrefab;

    private Mesh sphereMesh; //球

    // private List<Vector3> cluster = new List<Vector3>();
    private List<DLANode> cluster = new List<DLANode>();

    public void Start_brauch()
    {
        // Debug.Log(spherePrefab);
        // Generate();

        //球配置
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        sphereMesh = temp.GetComponent<MeshFilter>().sharedMesh;

        Destroy(temp);

        Generate();
    }

    void Generate()
    {
        cluster.Clear();

        // cluster.Add(Vector3.zero);初期位置の保存
        
        DLANode root = new DLANode(Vector3.zero);
        root.position += startPosition;
        cluster.Add(root);


        // Instantiate(spherePrefab, Vector3.zero, Quaternion.identity);

        for (int i = 0; i < particleCount; i++)
        {
            this.nowCount = i;
            WalkParticle();
            Debug.Log(i);
        }
        Debug.Log(cluster.Count);
        Debug.Log(cluster[1].parent.position);// (0.00, 0.00, 0.00)

        
        //mesh化
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        

        foreach(DLANode node in cluster)
        {
            if(node.parent==null)
                continue;

            CreateCylinder(
                node.parent.position,
                node.position,
                node.parent.radius,
                node.radius,
                vertices,
                triangles,
                uvs
            );
            CreateCylinder_yy(
                node.parent.position,
                node.position,
                node.parent.radius,
                node.radius,
                vertices,
                triangles,
                uvs
            );

            // CreateSphere(
            //     node.parent.position,
            //     node.parent.radius,
            //     vertices,
            //     triangles,
            //     uvs
            // );

            CreateSphere(
                node.position,
                node.radius,
                vertices,
                triangles,
                uvs
            );
        }

        // foreach(DLANode node in cluster){
        //     if(node.parent==null)
        //         continue;

        //     CreateSphere(
        //         node.parent.position,
        //         node.parent.radius,
        //         vertices,
        //         triangles,
        //         uvs
        //     );
        // }

        Mesh mesh=new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices=vertices.ToArray();
        mesh.triangles=triangles.ToArray();
        mesh.uv = uvs.ToArray();


        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        // mesh.RecalculateTangents();

        GetComponent<MeshFilter>().mesh=mesh;
        GetComponent<MeshRenderer>().material = barkMaterial;
        

    }

    // void WalkParticle()
    // {
    //     Vector3 pos = Random.onUnitSphere * spawnRadius;

    //     pos.y = Mathf.Abs(pos.y);

    //     for (int step = 0; step < maxStep; step++)
    //     {
    //         Vector3 dir = Random.onUnitSphere;

    //         pos += dir * stepSize;

    //         foreach (Vector3 p in cluster)
    //         {
    //             if (Vector3.Distance(pos, p) < stickDistance)
    //             {
    //                 cluster.Add(pos);

    //                 Instantiate(
    //                     spherePrefab,
    //                     pos,
    //                     Quaternion.identity
    //                 );

    //                 return;
    //             }
    //         }

    //         if (pos.magnitude > spawnRadius * 2)
    //         {
    //             pos = Random.onUnitSphere * spawnRadius;
    //             pos.y = Mathf.Abs(pos.y);
    //         }
    //     }
    // }
    void WalkParticle()//ひとつのパーティクルが移動
    {
        Vector3 pos = Random.onUnitSphere * spawnRadius; //初期位置　半径1の球の表面上のランダムな点
        
        if (this.nowCount < this.particleCount/2){
            pos.y = Mathf.Abs(pos.y);
            
        }
        else{
            pos.y = - Mathf.Abs(pos.y);
        }
        // pos.y = Mathf.Abs(pos.y);
        pos += startPosition;
        for (int step = 0; step < maxStep; step++)
        {
            // 中心方向
            Vector3 toCenter = (startPosition - pos).normalized;

            // 少しランダム性を加える
            Vector3 random = Random.onUnitSphere * 0.3f;

            Vector3 dir = (toCenter + random).normalized;

            

            float radius_p = 0.12f;

            pos += dir * stepSize;

            // foreach (Vector3 p in cluster)
            foreach (DLANode p in cluster)
            {
                if (this.nowCount < this.particleCount/2 ){
                    if (pos.y > startPosition.y){
                        if (Vector3.Distance(pos, p.position) < stickDistance)
                        {
                            //Debug.Log(cluster[1].parent.position);// (0.00, 0.00, 0.00)
                            if (p.position == startPosition)
                            {
                                if (origin_branch < 5){
                                    
                                    MakeChild(pos,p,radius_p);
                                }
                                // Debug.Log("pは原点です");
                                origin_branch += 1;


                            }
                            else if(p.children.Count <= 0){
                                if (Vector3.Distance(pos, startPosition) > 1)
                                    radius_p = 0.05f;
                            
                                MakeChild(pos,p,radius_p);
                                // int r = Random.Range(0,2);
                                if (Random.Range(0,3) != 0){
                                    leaf_point.Add(pos);
                                }
                                    
                            }
                            
                            // DLANode child = new DLANode(pos);

                            // child.parent = p;
                            // p.children.Add(child);

                            // cluster.Add(child);

                            // Instantiate(
                            //     spherePrefab,
                            //     pos,
                            //     Quaternion.identity);

                            // // 線を作成
                            // GameObject lineObj = new GameObject("Branch");
                            // LineRenderer lr = lineObj.AddComponent<LineRenderer>();

                            // lr.positionCount = 2;
                            // // lr.SetPosition(0, p);      // 親
                            // // lr.SetPosition(1, pos);    // 子
                            // lr.SetPosition(0, p.position);
                            // lr.SetPosition(1, child.position);

                            // lr.startWidth = 0.05f;
                            // lr.endWidth = 0.05f;

                            // lr.material = new Material(Shader.Find("Sprites/Default"));
                            // lr.startColor = Color.black;
                            // lr.endColor = Color.black;
                            return;
                        }
                    }

                }
                else{
                    if (Vector3.Distance(pos, p.position) < stickDistance && Vector3.Distance(pos, startPosition) > 1 && p.children.Count <= 1)
                    {
                        radius_p = 0.02f;
                        // if (Vector3.Distance(pos, Vector3.zero) > 1)
                        //     radius_p = 0.1f;

                        MakeChild(pos,p,radius_p);
                        leaf_point.Add(pos);
                        
                        return;
                    }
                }
                
            }
        }
    }


    void MakeChild(Vector3 pos,DLANode p,float radius_p){
        DLANode child = new DLANode(pos);
        child.radius = radius_p;
        child.parent = p; //自分自身の親を登録
        p.children.Add(child); //親の子を自分として登録

        cluster.Add(child);

        // Instantiate(
        //     spherePrefab,
        //     pos,
        //     Quaternion.identity
        // );

        // 線を作成
        // GameObject lineObj = new GameObject("Branch");
        // LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        // lr.positionCount = 2;
        // // lr.SetPosition(0, p);      // 親
        // // lr.SetPosition(1, pos);    // 子
        // lr.SetPosition(0, p.position);
        // lr.SetPosition(1, child.position);

        // lr.startWidth = 0.05f;
        // lr.endWidth = 0.05f;

        // lr.material = new Material(Shader.Find("Sprites/Default"));
        // lr.startColor = Color.black;
        // lr.endColor = Color.black;
    }
    //mesh化
    void CreateCylinder(Vector3 start,Vector3 end,float r1,float r2,List<Vector3> vertices,List<int> triangles,List<Vector2> uvs)
    {
        int startIndex = vertices.Count;
        int segments = 10;

        Vector3 axis = (end - start).normalized;

        Vector3 right = Vector3.Cross(axis, Vector3.up);

        if (right.magnitude < 0.001f)
            right = Vector3.Cross(axis, Vector3.forward);

        right.Normalize();

        if (start.y > end.y)
        {
            (start, end) = (end, start);
            (r1, r2) = (r2, r1);
        }

        Vector3 forward = Vector3.Cross(right, axis);
        //水平方向
        for(int ring=0; ring<2; ring++)
        {
            Vector3 center = (ring==0)?start:end;
            float radius = (ring==0)?r1:r2;

            for(int i=0;i<segments;i++)
            {
                // float angle = Mathf.PI*2*i/segments;

                // Vector3 offset =
                //     Mathf.Cos(angle)*right*radius +
                //     Mathf.Sin(angle)*forward*radius;

                // vertices.Add(center+offset);

                float angle = Mathf.PI*2*i/segments;

                float x = Mathf.Cos(angle)*radius;
                float z = Mathf.Sin(angle)*radius;
                float value = Random.Range(-0.8f, 0.8f);
                float SizeScale = 0.1f;
                vertices.Add(center + new Vector3(x,value*SizeScale,z));


                float u = (float)i / segments;
                float v = (float)ring; // (path.Count - 1);

                uvs.Add(new Vector2(u, v));

                // float u = (float)j / segments;
                // float v = (float)i / (path.Count - 1);

                // vertices.Add(new Vector2(u, v));
            }
        }

        for(int i=0;i<segments;i++)
        {
            int current=startIndex+i;
            int next=startIndex+(i+1)%segments;

            int upper=current+segments;
            int upperNext=next+segments;

            triangles.Add(current);
            triangles.Add(upper);
            triangles.Add(next);
            



            triangles.Add(next);
            triangles.Add(upper);
            triangles.Add(upperNext);
            
            
        }

        // ---------------------------
        // 下側のフタ
        // ---------------------------

        int bottomCenter = vertices.Count;
        vertices.Add(start);
        uvs.Add(new Vector2(0.5f, 0.5f));

        for(int i=0;i<segments;i++)
        {
            int next = (i + 1) % segments;

            triangles.Add(bottomCenter);
            triangles.Add(startIndex + next);
            triangles.Add(startIndex + i);
        }

        // ---------------------------
        // 上側のフタ
        // ---------------------------

        int topCenter = vertices.Count;
        vertices.Add(end);
        uvs.Add(new Vector2(0.5f, 0.5f));

        int topStart = startIndex + segments;

        for(int i=0;i<segments;i++)
        {
            int next = (i + 1) % segments;

            triangles.Add(topCenter);
            triangles.Add(topStart + i);
            triangles.Add(topStart + next);
        }

        //xy or yz
        for(int ring=0; ring<2; ring++)
        {
            Vector3 center = (ring==0)?start:end;
            float radius = (ring==0)?r1:r2;

            for(int i=0;i<segments;i++)
            {
                // float angle = Mathf.PI*2*i/segments;

                // Vector3 offset =
                //     Mathf.Cos(angle)*right*radius +
                //     Mathf.Sin(angle)*forward*radius;

                // vertices.Add(center+offset);

                float angle = Mathf.PI*2*i/segments;

                float x = Mathf.Cos(angle)*radius;
                float z = Mathf.Sin(angle)*radius;
                float value = Random.Range(-0.8f, 0.8f);
                float SizeScale = 0.1f;


                vertices.Add(center + new Vector3(x,value*SizeScale,z));


                float u = (float)i / segments;
                float v = (float)ring; // (path.Count - 1);

                uvs.Add(new Vector2(u, v));

                // float u = (float)j / segments;
                // float v = (float)i / (path.Count - 1);

                // vertices.Add(new Vector2(u, v));
            }
        }

        for(int i=0;i<segments;i++)
        {
            int current=startIndex+i;
            int next=startIndex+(i+1)%segments;

            int upper=current+segments;
            int upperNext=next+segments;

            triangles.Add(current);
            triangles.Add(upper);
            triangles.Add(next);
            



            triangles.Add(next);
            triangles.Add(upper);
            triangles.Add(upperNext);
            
            
        }


        // //垂直
        // for(int ring=0; ring<2; ring++)
        // {
        //     Vector3 center = (ring==0)?start:end;
        //     float radius = (ring==0)?r1:r2;

        //     for(int i=0;i<segments;i++)
        //     {
        //         float angle = Mathf.PI*2*i/segments;

        //         Vector3 offset =
        //             Mathf.Cos(angle)*right*radius +
        //             Mathf.Sin(angle)*forward*radius;

        //         vertices.Add(center+offset);



        //         // float u = (float)j / segments;
        //         // float v = (float)i / (path.Count - 1);

        //         // vertices.Add(new Vector2(u, v));
        //         float u = (float)i / segments;
        //         float v = (float)ring; // (path.Count - 1);

        //         uvs.Add(new Vector2(u, v));

                
        //     }
        // }

        // for(int i=0;i<segments;i++)
        // {
        //     int current=startIndex+i;
        //     int next=startIndex+(i+1)%segments;

        //     int upper=current+segments;
        //     int upperNext=next+segments;

        //     triangles.Add(current);
        //     triangles.Add(upper);
        //     triangles.Add(next);

        //     triangles.Add(next);
        //     triangles.Add(upper);
        //     triangles.Add(upperNext);
        // }
    }


    void CreateCylinder_yy(Vector3 start,Vector3 end,float r1,float r2,List<Vector3> vertices,List<int> triangles,List<Vector2> uvs)
    {
        int startIndex = vertices.Count;
        int segments = 10;

        Vector3 axis = (end - start).normalized;

        Vector3 right = Vector3.Cross(axis, Vector3.up);

        if (right.magnitude < 0.001f)
            right = Vector3.Cross(axis, Vector3.forward);

        right.Normalize();

        if (Mathf.Abs((start.x-end.x)) > Mathf.Abs((start.z-end.z))){
        //z-y
            if (start.x > end.x)
            {
                (start, end) = (end, start);
                (r1, r2) = (r2, r1);
            }

            Vector3 forward = Vector3.Cross(right, axis);
            //水平方向
            for(int ring=0; ring<2; ring++)
            {
                Vector3 center = (ring==0)?start:end;
                float radius = (ring==0)?r1:r2;

                for(int i=0;i<segments;i++)
                {
                    

                    float angle = Mathf.PI*2*i/segments;

                    float z = Mathf.Cos(angle)*radius;
                    float y = Mathf.Sin(angle)*radius;
                    float value = Random.Range(-0.5f, 0.5f);
                    float SizeScale = 0.0f;
                    vertices.Add(center + new Vector3(value*SizeScale,y,z));


                    float u = (float)i / segments;
                    float v = (float)ring; // (path.Count - 1);

                    uvs.Add(new Vector2(u, v));

                }
            }

            for(int i=0;i<segments;i++)
            {
                int current=startIndex+i;
                int next=startIndex+(i+1)%segments;

                int upper=current+segments;
                int upperNext=next+segments;

                triangles.Add(current);
                triangles.Add(upper);
                triangles.Add(next);
                



                triangles.Add(next);
                triangles.Add(upper);
                triangles.Add(upperNext);
                
                
            }
        }
        else {
            if (start.z > end.z)
            {
                (start, end) = (end, start);
                (r1, r2) = (r2, r1);
            }

            Vector3 forward = Vector3.Cross(right, axis);
            //水平方向
            for(int ring=0; ring<2; ring++)
            {
                Vector3 center = (ring==0)?start:end;
                float radius = (ring==0)?r1:r2;

                for(int i=0;i<segments;i++)
                {
                    

                    float angle = Mathf.PI*2*i/segments;

                    float y = Mathf.Cos(angle)*radius;
                    float x = Mathf.Sin(angle)*radius;
                    float value = Random.Range(-0.5f, 0.5f);
                    float SizeScale = 0.0f;
                    vertices.Add(center + new Vector3(x,y,value*SizeScale));


                    float u = (float)i / segments;
                    float v = (float)ring; // (path.Count - 1);

                    uvs.Add(new Vector2(u, v));

                }
            }

            for(int i=0;i<segments;i++)
            {
                int current=startIndex+i;
                int next=startIndex+(i+1)%segments;

                int upper=current+segments;
                int upperNext=next+segments;

                triangles.Add(current);
                triangles.Add(upper);
                triangles.Add(next);
                
                triangles.Add(next);
                triangles.Add(upper);
                triangles.Add(upperNext);
                
                
            }
        }
        

            
        


    }

    void CreateSphere(Vector3 center,float radius,List<Vector3> vertices,List<int> triangles,List<Vector2> uvs)
    {
        int startIndex = vertices.Count;

        // 頂点
        foreach (Vector3 v in sphereMesh.vertices)
        {
            vertices.Add(center + v * radius*(1.8f));
        }

        // UV
        foreach (Vector2 uv in sphereMesh.uv)
        {
            uvs.Add(uv);
        }

        // 三角形
        foreach (int t in sphereMesh.triangles)
        {
            triangles.Add(startIndex + t);
        }
    }
}


public class DLANode
{
    public Vector3 position;
    public DLANode parent;
    public float radius = 0.2f;
    
    public List<DLANode> children = new List<DLANode>();

    public DLANode(Vector3 pos)
    {
        position = pos;
    }
}
