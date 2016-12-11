using System.Windows.Media.Media3D;

/// <summary>
/// Extension for Point3D class
/// </summary>
public static class Point3DExtension
{
    /// <summary>
    /// Calculate distance between points
    /// </summary>
    /// <param name="pointFrom">Start point</param>
    /// <param name="pointTo">End point</param>
    /// <returns>Distance</returns>
    public static double DistanceTo(this Point3D pointFrom, Point3D pointTo)
    {
        return Point3D.Subtract(pointFrom, pointTo).Length;
    }
}
