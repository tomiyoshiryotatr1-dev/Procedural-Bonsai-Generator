// using UnityEngine;

// public class BonsaiAll : MonoBehaviour
// {
//     void Start()
//     {
//         // 幹
//         GameObject trunkObj = new GameObject("Trunk");
//         trunkObj.AddComponent<MeshFilter>();
//         trunkObj.AddComponent<MeshRenderer>();

//         CylinderMesh trunk = trunkObj.AddComponent<CylinderMesh>();
//         trunk.Start_trunk();


//         // 枝
//         GameObject branchObj = new GameObject("Branch");
//         branchObj.AddComponent<MeshFilter>();
//         branchObj.AddComponent<MeshRenderer>();

//         DlaBonsai2 branch = branchObj.AddComponent<DlaBonsai2>();

//         // 必要ならパラメータを変更
//         branch.startPosition = new Vector3(0, 10, 0);
//         branch.particleCount = 2000;

//         branch.Start_brauch();
//     }
// }

using UnityEngine;

public class BonsaiAll : MonoBehaviour
{
    GameObject obj;
    CylinderMesh cylinder;

    public Material barkMaterial;
    public Material barkMaterial2;
    public Material leafMaterial;

    void Start()
    {
        obj = new GameObject("Cylinder");

        // ←追加
        obj.AddComponent<MeshFilter>();

        MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
        // CylinderMesh trunk = obj.AddComponent<CylinderMesh>();
        renderer.material = barkMaterial;

        CylinderMesh trunk = obj.AddComponent<CylinderMesh>();

        trunk.Start_trunk();

        // obj.end_point



        // 枝
        GameObject branchObj = new GameObject("Branch");
        branchObj.AddComponent<MeshFilter>();
        MeshRenderer renderer2 = branchObj.AddComponent<MeshRenderer>();
        // renderer2.material = barkMaterial2;

      


        DlaBonsai2 branch = branchObj.AddComponent<DlaBonsai2>();

        branch.barkMaterial = barkMaterial2;
        branch.startPosition = trunk.end_point;
        branch.particleCount = 2000;
        // Debug.Log("a");
        // Debug.Log(trunk.end_point);
        
        branch.Start_brauch();


        //========================
        // 葉
        //========================
        GameObject leafObj = new GameObject("Leaf");

        bonsaileaf leaf = leafObj.AddComponent<bonsaileaf>();

        leaf.leafMaterial = leafMaterial;   // 葉用Material
        leaf.center = trunk.end_point;      // 幹の先端から生やす
        leaf.leafCount = 100;

        leaf.Start_leaf(branch.leaf_point);
    }
}