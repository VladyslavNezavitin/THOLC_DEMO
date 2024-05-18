using UnityEngine;

public static class GameObjectExtensions
{
    public static void SetTag(this GameObject gameObject, string tag)
    {
        gameObject.tag = tag;

        Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);

        foreach (var child in children)
            child.gameObject.tag = tag;
    }

    public static void SetLayer(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;

        Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);

        foreach (var child in children)
            child.gameObject.layer = layer;
    }
}

public static class AxisExtensions
{
    public static Vector3 ToVector(this Axis axis)
    {
        return axis switch
        {
            Axis.X => Vector3.right,
            Axis.Y => Vector3.up,
            _ => Vector3.forward,
        };
    }

    public static Vector3 ToTransformDirection(this Axis axis, Transform transform)
    {
        return axis switch
        {
            Axis.X => transform.right,
            Axis.Y => transform.up,
            _ => transform.forward,
        };
    }

    public static float GetLocalRotationAngle(this Axis axis, Transform transform)
    {
        return axis switch
        {
            Axis.X => transform.localRotation.x,
            Axis.Y => transform.localRotation.y,
            _ => transform.localRotation.z,
        };
    }
}

public static class EPElementIOExtensions
{
    public static EPDirection Offset(this EPDirection dir, int offset)
    {
        if (offset == 0) return dir;

        offset %= 4;

        if (offset < 0)
            offset = 4 + offset;

        int dirValue = (int)dir;
        int shiftedValue = dirValue << offset | dirValue >> (4 - offset);

        return (EPDirection)(shiftedValue & 0xF);
    }
}