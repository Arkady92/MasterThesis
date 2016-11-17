using System.Windows.Media.Media3D;

public static class Point3DExtension
{
    public static double DistanceTo(this Point3D pointFrom, Point3D pointTo)
    {
        return Point3D.Subtract(pointFrom, pointTo).Length;
    }
}
