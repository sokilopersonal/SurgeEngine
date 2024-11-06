using UnityEngine;

public static class VectorExtensions
{
    public static float SignedAngleByAxis(this Vector3 v1, Vector3 v2, Vector3 axis) 
    {
        Vector3 right = Vector3.Cross(v2, axis);
        v2 = Vector3.Cross(axis, right);
        return Mathf.Atan2(Vector3.Dot(v1, right), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }
}