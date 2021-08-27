using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2 v2_Position;

    public void Setup(int x, int y)
    {
        v2_Position = new Vector2(x, y);
    }

}
