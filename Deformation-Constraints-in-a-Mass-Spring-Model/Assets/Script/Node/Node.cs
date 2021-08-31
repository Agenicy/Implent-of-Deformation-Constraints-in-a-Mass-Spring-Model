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

    Vector3 v3_Norm
    {
        get
        {
            Vector3 center = Vector3.zero;
            foreach (var spring in list_spring)
            {
                center += spring.GetForce(this);
            }

            return Vector3.Cross(list_spring[0].GetForce(this) - center, list_spring[1].GetForce(this) - center).normalized;
        }
    }

    Vector3 v3_WindEffect => Vector3.forward * f_Mass * 10;
    float f_DampingCoefficient = 1.0f;
    Vector3 v3_Viscous_Wind => f_DampingCoefficient * (Vector3.Dot(v3_Norm, v3_WindEffect - velocity) * v3_Norm);
    Vector3 v3_Viscous_Air => -velocity;

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
        n.list_spring.Add(spring);
    }

    public void Compute()
    {
        v3_TotalForce = Vector3.zero;

        foreach (var spring in list_spring)
            v3_TotalForce += spring.GetForce(this);

        v3_TotalForce += v3_Gravity;

        v3_TotalForce += v3_Viscous_Wind + v3_Viscous_Air;

        accel = v3_TotalForce / f_Mass;
        velocity = velocity + f_DeltaTime * accel;
        v3_Position = v3_Position + f_DeltaTime * velocity;
    }

}

public class Spring
{
    float f_OriginialLength;
    float f_K => 30;

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
        /*switch (forceType)
        {
            case ForceType.Structural:
                Debug.DrawLine(m.v3_Position, n.v3_Position, Color.red, Node.f_DeltaTime);
                break;
            case ForceType.Shear:
                Debug.DrawLine(m.v3_Position, n.v3_Position, Color.blue, Node.f_DeltaTime);
                break;
            case ForceType.Flexion:
                Debug.DrawLine(m.v3_Position, n.v3_Position, Color.green, Node.f_DeltaTime);
                break;
            default:
                break;
        }*/
    }


    public Vector3 GetForce(Node self)
    {
        return m == self ? v3_ForceAtM : -v3_ForceAtM;

    }
}
