using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField] int xSize;
    [SerializeField] int ySize;
    [SerializeField] GameObject prefab_Node;
    Mesh mesh;

    float f_GridSize = 3.0f;

    public static List<Node> list_node_AllNode = new List<Node>();

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                GameObject gobj_node = Instantiate(prefab_Node, transform);
                gobj_node.transform.position = new Vector3(x* f_GridSize, y* f_GridSize, 0);

                Node node = gobj_node.GetComponent<Node>();
                node.Setup(x * f_GridSize, y * f_GridSize, 0);
                list_node_AllNode.Add(node);

            }
        }
    }

    private void Update()
    {
        mesh.vertices = list_node_AllNode.Select((n)=>n.v3_Position).ToArray();

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }


        /*
    private void OnDrawGizmos()
    {
        if (list_node_AllNode.Count == 0)
        {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < list_node_AllNode.Count; i++)
        {
            Gizmos.DrawSphere(list_node_AllNode[i].v3_Position, 0.1f);
        }
    }*/
}
