using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField] (int, int) int2_MeshSize;
    [SerializeField] GameObject prefab_Node;

    public static List<Node> list_node_AllNode = new List<Node>();
    // Start is called before the first frame update
    void Start()
    {
        (int x, int y) = int2_MeshSize;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject gobj_node = Instantiate(prefab_Node, transform);
                gobj_node.transform.position = new Vector3(i, j, 0);

                Node node = gobj_node.GetComponent<Node>();
                node.Setup(i, j);
                list_node_AllNode.Add(node);
            }
        }
    }

}
