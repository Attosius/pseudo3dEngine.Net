using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine.DrawableObjects;

public class CameraMan : Drawable
{
    public float DistanceView = 200f;

    public double Fov { get; set; } = 3.14 / 3;

    public Vector2f Center { get; set; }
    public World? World { get; set; }

    public void Draw(RenderTarget target, RenderStates states)
    {
        if (World?.Person == null)
        {
            return;
        }
        var person = World.Person;
        Center = person.Center;

        DrawViewSector(target, person);


        var leftViewAngle = person.Direction - Fov / 2;
        var deltaRay = Fov / 10;
        for (int i = 0; i < 10; i++)
        {
            var currentAngle = leftViewAngle + deltaRay * i;
            var point = Helper.GetPointAtAngleAndDistance(Center, currentAngle, DistanceView);
            // center, point
            var lineArr = new Vertex[]
            {
                new Vertex(Center),
                new Vertex(point)
            };
            target.Draw(lineArr, PrimitiveType.LineStrip);
            var segmentRay = (p1: Center, p2: point);
            foreach (var wordObject in World.Objects)
            {
                var points = wordObject.Points;
                var segmentObj = (p1: points[^1], p2: points[0]); // points[points.Count - 1]
                for (int j = 0; j < points.Count; j++)
                {
                    var isSegmentsCrossing = IsSegmentsCrossing(segmentRay, segmentObj, out Vector2f crossPoint);
                }
            }
        }

    }

    private bool IsSegmentsCrossing((Vector2f p1, Vector2f p2) segmentRay, (Vector2f p1, Vector2f p2) segmentObj, out Vector2f vector2F)
    {
        vector2F = default;
        return false;
    }


    private void DrawViewSector(RenderTarget target, Person person)
    {
        var viewSector = new Object2d();
        viewSector.Points.Add(Center);

        var leftViewAngle = person.Direction - Fov / 2;
        var leftPoint = Helper.GetPointAtAngleAndDistance(Center, leftViewAngle, DistanceView);
        viewSector.Points.Add(leftPoint);

        var delta = Fov / 10;
        var currentAngle = leftViewAngle;
        for (int i = 0; i < 10; i++)
        {
            var point = Helper.GetPointAtAngleAndDistance(Center, currentAngle, DistanceView);
            currentAngle += delta;
            viewSector.Points.Add(point);
        }

        var rightViewAngle = person.Direction + Fov / 2;
        var rightPoint = Helper.GetPointAtAngleAndDistance(Center, rightViewAngle, DistanceView);
        viewSector.Points.Add(rightPoint);
        target.Draw(viewSector);
    }
}