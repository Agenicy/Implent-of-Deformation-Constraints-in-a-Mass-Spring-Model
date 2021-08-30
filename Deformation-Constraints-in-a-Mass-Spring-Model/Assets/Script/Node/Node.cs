using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class Node : MonoBehaviour
{
    public Vector3 v3_Position=> transform.position;

    public void Setup(float x, float y, float z)
    {
        transform.SetPositionAndRotation(new Vector3(x,y,z), Quaternion.Euler(0, 0, 0));
    }
}
