using UnityEngine;

public static class JZVectorExtensions
{
    /// <summary>
    /// Returns a vector where each element is squared
    /// </summary>
    public static Vector2 SquareVector(this Vector2 vector)
    {
        return new Vector2(vector.x * vector.x, vector.y * vector.y);
    }

    //Courtesy of the GMTK Discord
    /// <summary>
    /// Allows you to replace the a value in a vector
    /// </summary>
    public static Vector3 Replace(this Vector3 v, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(
            x ?? v.x,
            y ?? v.y,
            z ?? v.z
        );
    }

    //Courtesy of the GMTK Discord
    /// <summary>
    /// Flatten the Vector along the Y axis.
    /// </summary>
    public static Vector3 Flatten(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }

    //Courtesy of the GMTK Discord
    /// <summary>
    /// Flatten the Vector along the X axis.
    /// </summary>
    public static Vector3 FlattenX(this Vector3 vector)
    {
        return new Vector3(0f, vector.y, vector.z);
    }

    //Courtesy of the GMTK Discord
    /// <summary>
    /// Flatten the Vector along the Z axis.
    /// </summary>
    public static Vector3 FlattenZ(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, 0f);
    }

    //Courtesy of the GMTK Discord
    /// <summary>
    /// Flatten the Vector along the specified vector. Vector is assumed to be normalized
    /// </summary>
    /// <param name="axis">Vector to be flattened along. Must be normalized.</param>
    public static Vector3 Flatten(this Vector3 vector, Vector3 axis)
    {
        return vector - (axis * Vector3.Dot(vector, axis));
    }

    /// <summary>
    /// Returns a vector where each element is squared
    /// </summary>
    public static Vector3 SquareVector(this Vector3 vector)
    {
        return new Vector3(vector.x * vector.x, vector.y * vector.y, vector.z * vector.z);
    }

    /// <summary>
    /// Determines if vector magnitude is within a range using square magnitude
    /// </summary>
    public static bool InRange(this Vector2 vector, Vector2 endPoint, float range)
    {
        return (vector - endPoint).InRange(range);
    }
    public static bool InRange(this Vector2 vector, float range)
    {
        return vector.sqrMagnitude <= range * range;
    }
    public static bool InRange(this Vector3 vector, Vector3 endPoint, float range)
    {
        return (vector - endPoint).InRange(range);
    }
    public static bool InRange(this Vector3 vector, float range)
    {
        return vector.sqrMagnitude <= range * range;
    }

    /// <summary>
    /// Returns true if the unit is almost facing
    /// </summary>
    /// <param name="facing">The direction you are currently facing</param>
    /// <param name="targetDirection">The direction you are comparing against</param>
    /// <param name="limit">Limit for how close is close enough. Default is 90%.</param>
    /// <returns></returns>
    public static bool AlmostFacing(this Vector3 facing, Vector3 targetDirection, float limit = 0.90f)
    {
        return Vector3.Dot(facing, targetDirection) >= limit;
    }

    /// <summary>
    /// Returns a vector where each element of the base vector is squared
    /// </summary>
    public static Vector3 ElementSquare(this Vector3 vec)
    {
        return new Vector3(vec.x * vec.x, vec.y * vec.y, vec.z * vec.z);
    }
    public static Vector2 ElementSquare(this Vector2 vec)
    {
        return new Vector2(vec.x * vec.x, vec.y * vec.y);
    }

    /// <summary>
    /// Returns the square distance between two vectors
    /// </summary>
    public static float SquareDistance(this Vector3 vecA, Vector3 vecB)
    {
        var diff = vecB - vecA;
        return diff.sqrMagnitude;
    }
    public static float SquareDistance(this Vector2 vecA, Vector2 vecB)
    {
        var diff = vecB - vecA;
        return diff.sqrMagnitude;
    }

    /// <summary>
    /// Elementwise clamps a vector
    /// </summary>
    /// <param name="current"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static Vector3 Clamp(this Vector3 current, Vector3 min, Vector3 max)
    {
        Vector3 result;
        result.x = Mathf.Clamp(current.x, min.x, max.x);
        result.y = Mathf.Clamp(current.y, min.y, max.y);
        result.z = Mathf.Clamp(current.z, min.z, max.z);
        return result;
    }
    public static Vector2 Clamp(this Vector2 current, Vector2 min, Vector2 max)
    {
        Vector2 result;
        result.x = Mathf.Clamp(current.x, min.x, max.x);
        result.y = Mathf.Clamp(current.y, min.y, max.y);
        return result;
    }
}