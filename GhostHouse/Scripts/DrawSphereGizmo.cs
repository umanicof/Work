using UnityEngine;

public class DrawSphereGizmo : MonoBehaviour
{
    public Color Color = Color.yellow;
    public float Radius = 0.5f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color;
        Gizmos.DrawSphere(transform.position, Radius);
    }
}
