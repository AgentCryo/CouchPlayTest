using System.Numerics;

namespace CouchPlayTest.Utilities;

public class Utility
{
    public static Vector2 ClampMagnitude(Vector2 v, float maxLength)
    {
        float length = v.Length();
        return length <= maxLength ? v : v / length * maxLength;
    }
    
    public static float Lerp(float a, float b, float t)
        => a + (b - a) * Math.Clamp(t, 0, 1);
}