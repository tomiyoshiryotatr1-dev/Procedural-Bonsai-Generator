using UnityEngine;
using System.Collections.Generic;

public class CylinderMesh : MonoBehaviour
{
    public float radius = 0.2f;
    public int segments = 40; //8
    public float SizeScale = 0.03f; //0.1

  
    public Vector3 end_point ;//= new Vector3(0, 0, 0);
    

    int size = 512;

    float[,] img;
    float[,] rootMap;

    public void Start_trunk()
    {

       Debug.Log("trunk");
       ( List<Vector3> path, List<float> r_list) = GenerateMap();

        
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        List<Vector2> uvs = new List<Vector2>();
        end_point = path[path.Count-1];

        int skip_i = 5;
        // 頂点生成　円をポリゴン化
        // リングの向きを保持する
        Vector3 prevRight = Vector3.right;
        for (int i = 0; i < path.Count; i+=skip_i){
            Vector3 center = path[i];

            // end_point = center;

            float radius = r_list[i];
            
            for(int j=0;j<segments;j++)
            {
                // float angle = Mathf.PI*2*j/segments;

                // float x = Mathf.Cos(angle)*radius;
                // float z = Mathf.Sin(angle)*radius;
                // float value = Random.Range(-1.5f, 1.5f);
                // vertices.Add(center + new Vector3(x,value*SizeScale,z));


                // float u = (float)j / segments;
                // float v = (float)i / (path.Count - 1);

                // uvs.Add(new Vector2(u, v));
                Vector3 pathDir;

                if(i == 0)
                    pathDir = (path[i + skip_i] - path[i]).normalized;
                else if(i >= path.Count - skip_i)
                    pathDir = (path[i] - path[i - skip_i]).normalized;
                else
                    pathDir = (path[i + skip_i] - path[i - skip_i]).normalized;

                // 0～10の間で徐々に補間
                float t = Mathf.Clamp01(i / 10f);

                Vector3 dir = Vector3.Slerp(Vector3.up, pathDir, t).normalized;


                // Vector3 right = Vector3.Cross(dir, Vector3.up);

                // if(right.sqrMagnitude < 0.0001f)
                //     right = Vector3.Cross(dir, Vector3.right);

                // right.Normalize();

                // Vector3 forward = Vector3.Cross(right, dir).normalized;//両方に垂直な方向です。

                // 前のリングのrightを利用してねじれを防ぐ
                Vector3 up = Vector3.Cross(prevRight, dir);

                if(up.sqrMagnitude < 0.0001f)
                    up = Vector3.Cross(Vector3.forward, dir);

                up.Normalize();

                Vector3 right = Vector3.Cross(dir, up).normalized;

                // 次のリングのために保存
                prevRight = right;

                Vector3 forward = Vector3.Cross(right, dir).normalized;


                // Vector3.right　x
                // Vector3.up y
                // Vector3.forward z

                float angle = Mathf.PI * 2 * j / segments;

                float noise = Random.Range(-1.5f,1.5f) * SizeScale;
                float right_noise = Random.Range(0.8f,1.0f);
                float forward_noise = Random.Range(0.8f,1.0f);

                Vector3 offset =
                    right   * (Mathf.Cos(angle) * radius*right_noise) +
                    forward * (Mathf.Sin(angle) * radius*forward_noise);

                offset += dir * noise;      // 樹皮の凹凸

                vertices.Add(center + offset);

                uvs.Add(new Vector2(
                    (float)j/segments,
                    (float)i/(path.Count-1)
                ));
            }
        }
        
        // 三角形生成
        Vector3 be_center = new Vector3(0, 0, 0);
        // Vector3 center = new Vector3(0, 0, 0);
        for(int ring=0; ring<(path.Count/skip_i)-1; ring++)
        {
            Vector3 center = path[ring];
            for(int i=0;i<segments;i++)
            {
                int current = ring*segments+i;
                int next = ring*segments+(i+1)%segments;

                int upper = current+segments;
                int upperNext = next+segments;

                Vector3 dir = path[ring + 1] - path[ring];

                triangles.Add(current);
                triangles.Add(upper);
                triangles.Add(next);

                triangles.Add(next);
                triangles.Add(upper);
                triangles.Add(upperNext);

                // if(Vector3.Dot(dir, Vector3.up) >= 0)
                // {
                //     triangles.Add(current);
                //     triangles.Add(upper);
                //     triangles.Add(next);

                //     triangles.Add(next);
                //     triangles.Add(upper);
                //     triangles.Add(upperNext);
                // }
                // else
                // {
                //     triangles.Add(current);
                //     triangles.Add(next);
                //     triangles.Add(upper);

                //     triangles.Add(next);
                //     triangles.Add(upperNext);
                //     triangles.Add(upper);
                // }
            }

            // if ((center.y - be_center.y) > 0){
            //     //正方向
            //     for(int i=0;i<segments;i++)
            //     {
            //         int current = ring*segments+i;
            //         int next = ring*segments+(i+1)%segments;

            //         int upper = current+segments;
            //         int upperNext = next+segments;

            //         triangles.Add(current);
                    
            //         triangles.Add(upper);
            //         triangles.Add(next);


            //         triangles.Add(next);
                    
            //         triangles.Add(upper);
            //         triangles.Add(upperNext);
            //     }
            // }
            // else{
            //     for(int i=0;i<segments;i++)
            //     {
            //         int current = ring*segments+i;
            //         int next = ring*segments+(i+1)%segments;

            //         int upper = current+segments;
            //         int upperNext = next+segments;

                    
                    
            //         triangles.Add(current);
            //         triangles.Add(next);
            //         triangles.Add(upper);

            //         triangles.Add(next);
            //         triangles.Add(upperNext);
            //         triangles.Add(upper);
            //     }
            // }

            // be_center = center; //= new Vector3(1, 10, 1);
                
        }

        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        GetComponent<MeshFilter>().mesh = mesh;

       
        Debug.Log(uvs[0]);
        Debug.Log(uvs[100]);
        Debug.Log(uvs[1000]);
        Debug.Log(mesh.vertexCount);
        Debug.Log(mesh.uv.Length);
        Debug.Log("End Point = " + end_point);
   
    }




    (List<Vector3>,List<float>) GenerateMap()
    {
        img = new float[size, size];
        rootMap = new float[size, size];

        //--------------------------------------------------
        // グラデーション生成
        //--------------------------------------------------

        for (int yy = 0; yy < size; yy++)
        {
            float grad = (float)yy / size;

            for (int xx = 0; xx < size; xx++)
            {
                img[yy, xx] = grad * 255f;
            }
        }

        //--------------------------------------------------
        // 円生成
        //--------------------------------------------------

        int nCircles = Random.Range(5, 10);

        int[] centersX = new int[nCircles];
        int[] centersY = new int[nCircles];
        int[] radii = new int[nCircles];

        for (int i = 0; i < nCircles; i++)
        {
            centersX[i] = Random.Range(
                10 * (size / 256),
                (size - 10) * size / 256);

            centersY[i] = Random.Range(0, size);

            radii[i] = Random.Range(
                10 * size / 256,
                200 * size / 256);
        }

        //--------------------------------------------------
        // 少し位置をずらす
        //--------------------------------------------------

        for (int i = 0; i < nCircles; i++)
        {
            centersY[i] += Random.Range(-5, 5);
            centersX[i] -= 5;
        }

        //--------------------------------------------------
        // グラデーション円
        //--------------------------------------------------

        for (int i = 0; i < nCircles; i++)
        {
            int cx = centersX[i];
            int cy = centersY[i];
            int radius = radii[i];

            for (int r = radius; r >= 1; r--)
            {
                float alpha = 255f * (1f - (float)r / radius);

                DrawCircle(
                    img,
                    cx,
                    cy,
                    r,
                    alpha);
            }
        }

        //--------------------------------------------------
        // ポット位置
        //--------------------------------------------------

        int wPot = 20 * size / 256;
        int potUnder = 2 * size / 256;

        int under =
            size
            - (27 * size / 256)
            - potUnder
            + (int)(wPot * 0.08f);

        int x = under;
        int subY = size / 2;
        int y = size / 2;

        //--------------------------------------------------
        // 幹開始位置
        //--------------------------------------------------

        for (int yy = x; yy < size; yy++)
        {
            img[yy, y] = 255;
        }

        //--------------------------------------------------
        // root_img2 = (1-img/255)
        //--------------------------------------------------

        for (int yy = 0; yy < size; yy++)
        {
            for (int xx = 0; xx < size; xx++)
            {
                rootMap[yy, xx] = 1f - img[yy, xx] / 255f;
            }
        }

        // Debug.Log("Map Generated");
        // Debug.Log(rootMap[10, 10]);

        //--------------------------------------------------
        // 幹生成
        //--------------------------------------------------

        int length = Random.Range(
            100 * size / 256,
            350 * size / 256);

        int branchStraight = Random.Range(36, 80);

        Vector2Int[] nextGrid =
        {
            new Vector2Int(-1,-1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int( 0,-1),
            new Vector2Int( 0, 1),
            new Vector2Int( 1,-1),
            new Vector2Int( 1, 0),
            new Vector2Int( 1, 1)
        };

        int underPos = x;
        List<Vector3> trunkPoints = new List<Vector3>();
        // List<int> x3 = new List<int>();
        List<float> r_list = new List<float>();
        // srand((unsigned int)time(nullptr));

        // int w = rand() % (35 * size / 256 - 15 * size / 256 + 1) + (15 * size / 256);
        int w = Random.Range(15 * size / 256,35 * size / 256 + 1);

        float trunkZ = 0.0f;
        for (int i = 0; i < length; i += 1) // i++
        {
            // x3.Add((size - x) - (size - underPos));
            // y3.Add(y - subY);
            float trunkX = y - subY;
            float trunkY = (size - x) - (size - underPos);
            

            // float scale = 0.1f;
            float scale = this.SizeScale;
            //zを変える
            int z_grad = Random.Range(0, 5);
            
            if (z_grad == 0){
                trunkZ += 1;
            }

            trunkPoints.Add(
                new Vector3(
                    trunkX * scale,
                    trunkY * scale,
                    trunkZ * scale
                )
            );

            // Debug.Log($"x={x}, y={y}");
            // Debug.Log($"size={size}");
            // if (x <= 0 || x >= size - 1 || y <= 0 || y >= size - 1)
            //     {
            //         Debug.Log("Out of range");
            //         break;
            //     }
            float[] nowGrid;
            float[] nowGrid2;

            if (x < 1)
            {
                nowGrid = new float[]
                {
                    rootMap[x,y-1],
                    rootMap[x,y+1],
                    rootMap[x+1,y-1],
                    rootMap[x+1,y],
                    rootMap[x+1,y+1]
                };

                nowGrid2 = new float[]
                {
                    rootMap[x,y-1],
                    rootMap[x,y+1]
                };
            }
            else
            {
                nowGrid = new float[]
                {
                    rootMap[x-1,y-1],
                    rootMap[x-1,y],
                    rootMap[x-1,y+1],
                    rootMap[x,y-1],
                    rootMap[x,y+1],
                    rootMap[x+1,y-1],
                    rootMap[x+1,y],
                    rootMap[x+1,y+1]
                };

                nowGrid2 = new float[]
                {
                    rootMap[x-1,y-1],
                    rootMap[x-1,y],
                    rootMap[x-1,y+1]
                };
                // float[] nowGrid2 =
                // {
                //     rootMap[x-1,y-1],
                //     rootMap[x-1,y],
                //     rootMap[x-1,y+1],
                //     rootMap[x,y-1],
                //     rootMap[x,y+1]
                // };
            }

            

            int argMin = ArgMin(nowGrid);
            float maxValue = Max(nowGrid);

            nowGrid[argMin] = maxValue;

            float[] norm = Normalize(nowGrid);
            float[] norm2 = Normalize(nowGrid2);

            norm[argMin] = 0f;
            norm[1] += 0.000001f;

            Vector2Int select;

            int r = Random.Range(0,2);

            if(i < branchStraight)
            {
                int argMax = ArgMax(norm2);
                select = nextGrid[argMax];
            }
            else if(r==0)
            {
                select = WeightedRandom(nextGrid,norm);
            }
            else
            {
                int argMax = ArgMax(norm);
                select = nextGrid[argMax];
            }

            x += select.x;
            y += select.y;

            // int r_w0 = rand() % 3;   // 0～2
            // int r_w  = rand() % 7;   // 0～6
            // int r_w2 = rand() % 9;   // 0～8
            int r_w0 = Random.Range(0, 3);
            int r_w = Random.Range(0, 7);
            int r_w2 = Random.Range(0, 9);

            if (w > 30 * size / 256)
            {
                if (r_w0 == 0)
                {
                    w -= 1;
                }
            }
            else if (w > 9 * size / 256)
            {
                if (r_w == 0)
                {
                    w += 1;
                }
                else if (r_w >= 1 && r_w <= 4)
                {
                    // w += 0;
                }
                else
                {
                    w -= 1;
                }
            }
            else if (w > 4 * size / 256)
            {
                if (r_w2 == 0)
                {
                    w -= 1;
                }
            }//w == 4 * size / 256 の時breik
            // else{
            //     break;
            // }
            r_list.Add((float)(w* scale));
        }
        return (trunkPoints,r_list);
        // List<int> xUnity = y3;
        // List<int> yUnity = x3;

        // return (xUnity, yUnity);

        // return rootMap;
    }


    void DrawCircle(
        float[,] image,
        int cx,
        int cy,
        int radius,
        float value)
    {
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    int px = cx + x;
                    int py = cy + y;

                    if (px >= 0 &&
                        px < size &&
                        py >= 0 &&
                        py < size)
                    {
                        image[py, px] = value;
                    }
                }
            }
        }
    }
    

    float Max(float[] array)
    {
        float max = array[0];

        foreach(float v in array)
        {
            if(v > max)
                max = v;
        }

        return max;
    }

    float Min(float[] array)
    {
        float min = array[0];

        foreach(float v in array)
        {
            if(v < min)
                min = v;
        }

        return min;
    }


    int ArgMax(float[] array)
    {
        int index = 0;

        for(int i=1;i<array.Length;i++)
        {
            if(array[i] > array[index])
                index = i;
        }

        return index;
    }


    int ArgMin(float[] array)
    {
        int index = 0;

        for(int i=1;i<array.Length;i++)
        {
            if(array[i] < array[index])
                index = i;
        }

        return index;
    }

    float[] Normalize(float[] array)
    {
        float min = Min(array);
        float max = Max(array);

        float[] result = new float[array.Length];

        for(int i=0;i<array.Length;i++)
        {
            result[i] =
                (array[i]-min)/
                (max-min+0.00001f);
        }

        return result;
    }

    Vector2Int WeightedRandom(Vector2Int[] dirs,float[] weights)
    {
        float total = 0;

        foreach(float w in weights)
            total += w;

        float r = Random.value * total;

        float sum = 0;

        for(int i=0;i<weights.Length;i++)
        {
            sum += weights[i];

            if(r <= sum)
                return dirs[i];
        }

        return dirs[0];
    }
        


    
}
