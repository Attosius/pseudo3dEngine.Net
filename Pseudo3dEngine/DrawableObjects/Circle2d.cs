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
    public Vector2f Begin => Position + new Vector2f(Radius, 0);

    public Circle2d() : base()
    {

    }
    public Circle2d(float radius) : base()
    {
        Radius = radius;
    }
    public  float CalculateArcLength(float radius, float x1, float y1, float x2, float y2)
    {
        x1 = x1 - Center.X;
        y1 = y1 - Center.Y;
        x2 = x2 - Center.X;
        y2 = y2 - Center.Y;

        // Calculate angles
        double angle1 = Math.Atan2(y1, x1);
        double angle2 = Math.Atan2(y2, x2);

        // Calculate angle difference
        double angleDifference = angle2 - angle1;

        // Adjust the angle difference to be within the range of -PI to PI
        if (angleDifference > Math.PI)
        {
            angleDifference -= 2 * Math.PI;
        }
        else if (angleDifference < -Math.PI)
        {
            angleDifference += 2 * Math.PI;
        }

        // Calculate the absolute angle difference
        double absoluteAngleDifference = Math.Abs(angleDifference);

        // Calculate arc length
        double arcLength = radius * absoluteAngleDifference;

        return (float) arcLength;
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

    public float GetArcLengthFromBegin(Vector2f point)
    {
        var len = CalculateArcLength(Radius, Begin.X, Begin.Y, point.X, point.Y);
        return len;
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