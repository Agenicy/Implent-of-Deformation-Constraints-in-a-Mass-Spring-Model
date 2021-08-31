using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class Node : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Vector3 v3_Position
    {
        get => transform.position;
        set
        {
            if (!rigidbody.isKinematic)
                transform.SetPositionAndRotation(value, Quaternion.Euler(0, 0, 0));
        }
    }

    Dictionary<Node, Spring> dict_node_force = new Dictionary<Node, Spring>();

    List<Spring> list_spring = new List<Spring>();

    Vector3 v3_TotalForce;

    float f_Mass => rigidbody.mass;

    Vector3 v3_Gravity => f_Mass * 9.8f * Vector3.down;
    Vector3 v3_Viscous => -velocity;

    float f_Tou => 0.1f;

    public static float f_DeltaTime => 0.01f;

    Vector3 accel, velocity;

    public void Setup(float x, float y, float z)
    {
        transform.SetPositionAndRotation(new Vector3(x, y, z), Quaternion.Euler(0, 0, 0));
    }

    public static void Link(Node m, Node n, Spring.ForceType forceType)
    {
        if (m is null || n is null)
            return;

        Spring spring = new Spring(forceType, m, n);
        m.list_spring.Add(spring);
    }

    public void Compute()
    {
        v3_TotalForce = Vector3.zero;

        foreach (var spring in list_spring)
            v3_TotalForce += spring.GetForce(this);

        v3_TotalForce += v3_Gravity;

        v3_TotalForce += v3_Viscous;

        accel = v3_TotalForce / f_Mass;
        velocity = velocity + f_DeltaTime * accel;
        v3_Position = v3_Position + f_DeltaTime * velocity;

        float mag = 1.0f / (1.0f + (float)Mathf.Exp(-v3_TotalForce.magnitude / 30f));
        Debug.DrawLine(v3_Position, v3_Position + v3_TotalForce.normalized * 2, Color.Lerp(Color.green, Color.yellow, mag), Node.f_DeltaTime);
    }
}

public class Spring
{
    float f_OriginialLength;
    float f_K = 1;

    Node m, n;

    Vector3 v3_ForceAtM;

    public static List<Spring> list_spring_AllSpring = new List<Spring>();

    public enum ForceType
    {
        Structural,
        Shear,
        Flexion
    }
    public readonly ForceType forceType;

    public Spring(ForceType forceType, Node m, Node n)
    {
        list_spring_AllSpring.Add(this);

        this.forceType = forceType;

        this.m = m;
        this.n = n;

        f_OriginialLength = Vector3.Distance(m.v3_Position, n.v3_Position);
    }

    public void Compute()
    {
        float nowLength = Vector3.Distance(m.v3_Position, n.v3_Position);
        float deltaX = Mathf.Max(nowLength - f_OriginialLength, 0);

        v3_ForceAtM = f_K * (n.v3_Position - m.v3_Position) * deltaX;

        float mag = 1.0f / (1.0f + (float)Mathf.Exp(-v3_ForceAtM.magnitude / 30f));
        Debug.DrawLine(m.v3_Position, n.v3_Position, Color.Lerp(Color.blue, Color.red, mag), Node.f_DeltaTime);
    }


    public Vector3 GetForce(Node self)
    {
        return m == self ? v3_ForceAtM : -v3_ForceAtM;

    }
}
