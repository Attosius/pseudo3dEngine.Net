using System.Diagnostics;
using System.Runtime.ExceptionServices;
using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine.DrawableObjects;

[DebuggerDisplay("{Name}")]
public class Circle2d : Object2d
{
    public float Radius { get; set; }
    public Vector2f Center => Position + new Vector2f(Radius, Radius);

    public Circle2d() : base()
    {

    }
    public Circle2d(float radius) : base()
    {
        Radius = radius;
    }

    public override bool IsRayCrossingObject((Vector2f first, Vector2f second) segmentRay, out Vector2f crossPoint)
    {
        crossPoint = default;
        var isCross = false; 
        var direction = segmentRay.second - segmentRay.first;
        var start = segmentRay.first;
        double kA = direction.X * direction.X + direction.Y * direction.Y;
        double kB = 2 * (direction.X * (start.X - Center.X) + direction.Y * (start.Y - Center.Y));
        double kC = (start.X - Center.X) * (start.X - Center.X) + (start.Y - Center.Y) * (start.Y - Center.Y) - Radius * Radius;

        var discriminant = kB * kB - 4 * kA * kC;
        if (discriminant < 0)
        {
            return false;
        }

        double t1 = (-kB + Math.Sqrt(discriminant)) / (2 * kA);
        double t2 = (-kB - Math.Sqrt(discriminant)) / (2 * kA);

        //Vector2f cross1 = default;

        if (t1 is >= 0 and <= 1)
        {
            crossPoint = start + direction.ScalarMult(t1);
            return true;
        }
        if (t2 is >= 0 and <= 1)
        {
            crossPoint = start + direction.ScalarMult(t2);
            return true;
        }


        return isCross;
    }

    public void SetCenter(Vector2f center)
    {
        Position = center - new Vector2f(Radius, Radius);
    }


    public override void Draw(RenderTarget target, RenderStates states)
    {

        var crossShape = new CircleShape(Radius);
        crossShape.Position = Position;
        crossShape.FillColor = FillColor;
        crossShape.OutlineThickness = OutlineThickness;
        crossShape.OutlineColor = OutlineColor;
        target.Draw(crossShape);

        crossShape = new CircleShape(4);
        crossShape.Position = Center - new Vector2f(4, 4);
        crossShape.FillColor = Color.White;
        target.Draw(crossShape);
    }

}