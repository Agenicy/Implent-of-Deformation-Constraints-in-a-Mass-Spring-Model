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

    public static Node[,] node_arr;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        node_arr = new Node[xSize + 1, ySize + 1];

        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                GameObject gobj_node = Instantiate(prefab_Node, transform);
                gobj_node.transform.position = new Vector3(x * f_GridSize, y * f_GridSize, 0);

                Node node = gobj_node.GetComponent<Node>();
                node.Setup(x * f_GridSize, y * f_GridSize, 0);
                node_arr[x, y] = node;
            }
        }
        node_arr[0, ySize].rigidbody.isKinematic = true;
        node_arr[xSize, ySize].rigidbody.isKinematic = true;

        Node GetNode(int i, int j)
        {
            return (i >= 0 && i < node_arr.GetLength(0) && j >= 0 && j < node_arr.GetLength(1)) ? node_arr[i, j] : null;
        };

        for (int x = 0; x <= xSize; x++)
            for (int y = 0; y <= ySize; y++)
            {
                Node.Link(GetNode(x, y), GetNode(x + 1, y), Spring.ForceType.Structural);
                Node.Link(GetNode(x, y), GetNode(x, y + 1), Spring.ForceType.Structural);

                Node.Link(GetNode(x, y), GetNode(x + 1, y + 1), Spring.ForceType.Shear);
                Node.Link(GetNode(x + 1, y), GetNode(x, y + 1), Spring.ForceType.Shear);

                Node.Link(GetNode(x, y), GetNode(x + 2, y), Spring.ForceType.Flexion);
                Node.Link(GetNode(x, y), GetNode(x, y + 2), Spring.ForceType.Flexion);
            }

        StartCoroutine(IUpdate());
    }

    IEnumerator IUpdate()
    {
        while (true)
        {
            List<Vector3> vertices = new List<Vector3>();
            for (int y = 0; y <= ySize; y++)
                for (int x = 0; x <= xSize; x++)
                    vertices.Add(node_arr[x, y].v3_Position);

            mesh.vertices = vertices.ToArray();

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

            yield return new WaitForSeconds(Node.f_DeltaTime);

            foreach (var sp in Spring.list_spring_AllSpring)
                sp.Compute();

            yield return new WaitForSeconds(Node.f_DeltaTime);

            for (int x = 0; x <= xSize; x++)
                for (int y = 0; y <= ySize; y++)
                    node_arr[x, y].Compute();
            Debug.Log("Tick");

            yield return new WaitForSeconds(Node.f_DeltaTime);
        }
    }

}
