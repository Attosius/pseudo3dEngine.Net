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

    public override bool IsRayCrossingObject((Vector2f first, Vector2f second) segmentRay, out Vector2f crossPoint, out (Vector2f first, Vector2f second)? segmentCrossingObj)
    {
        crossPoint = default;
        var isCross = false;
        segmentCrossingObj = null;
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

        var distance = float.MaxValue;

        if (t1 is >= 0 and <= 1)
        {
            crossPoint = start + direction.ScalarMult(t1);
            distance = segmentRay.first.DecartDistance(crossPoint);
            isCross = true;
        }
        if (t2 is >= 0 and <= 1)
        {
            var cross2 = start + direction.ScalarMult(t2);
            if (segmentRay.first.DecartDistance(cross2) < distance)
            {
                crossPoint = cross2;
            }
            isCross = true;
        }


        segmentCrossingObj = new(crossPoint, crossPoint);
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
        //crossShape.Scale = Scale;
        target.Draw(crossShape);

        crossShape = new CircleShape(2);
        crossShape.Position = Center - new Vector2f(2, 2);
        crossShape.FillColor = Color.White;
        //crossShape.Scale = Scale;
        target.Draw(crossShape);

        if (Type == Object2dTypes.Wall)
        {
            var text = new Text($"{Name}", Resources.FontCourerNew, 12);
            text.Position = Center;
            text.CharacterSize = 14;
            text.FillColor = Color.White;
            target.Draw(text);
        }
    }

}