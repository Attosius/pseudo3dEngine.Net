using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine.DrawableObjects;

public class Person : Object2d
{
    public Vector2f PersonPosition { get; set; } = new Vector2f(333, 606);
    //public double Fov { get; set; } = 3.14 / 3;
    public float Radius { get; set; } = 10;
    public Vector2f Center { get; set; }
    public float Direction { get; set; } = -125.54f*(float)Math.PI / 180;
    public float Speed = 10f;
    public float SpeedTurn = 0.1f;

    public float SpeedStrafe = 5f;
    //public float DistanceView = 200f;

    public float DirectionDegree => Direction * 180 / (float)Math.PI;

    public override void Draw(RenderTarget target, RenderStates states)
    {
        DrawPerson(target);

        Center = new Vector2f(PersonPosition.X + Radius, PersonPosition.Y + Radius);

        //var viewSector = new Object2d();
        //viewSector.Points.Add(Center);

        //var leftViewAngle = Direction - Fov / 2;
        //var leftPoint = Helper.GetPointAtAngleAndDistance(Center, leftViewAngle, DistanceView);
        //viewSector.Points.Add(leftPoint);

        //var delta = Fov / 10;
        //var currentAngle = leftViewAngle;
        //for (int i = 0; i < 10; i++)
        //{
        //    var point = Helper.GetPointAtAngleAndDistance(Center, currentAngle, DistanceView);
        //    currentAngle += delta;
        //    viewSector.Points.Add(point);
        //}

        //var rightViewAngle = Direction + Fov / 2;
        //var rightPoint = Helper.GetPointAtAngleAndDistance(Center, rightViewAngle, DistanceView);
        //viewSector.Points.Add(rightPoint);
        //target.Draw(viewSector);

        DrawDirection(target, states, Center);

        //
        //var deltaRay = Fov / 100;
        //for (int i = 0; i < 100; i++)
        //{
        //    currentAngle = leftViewAngle + deltaRay * i;
        //    var point = Helper.GetPointAtAngleAndDistance(Center, currentAngle, DistanceView);
        //    // center, point
        //}
    }


    private void DrawDirection(RenderTarget target, RenderStates states, Vector2f center)
    {
        var xDir = center.X + (float)Math.Sin(Direction) * 150f;
        var yDir = center.Y + (float)Math.Cos(Direction) * 150f;
        var lineArr = new Vertex[]
        {
            new(center),
            new(new Vector2f(xDir, yDir)),
        };
        //target.Draw(lineArr, PrimitiveType.LineStrip, states);
    }

    private void DrawPerson(RenderTarget target)
    {
        var person = new CircleShape(Radius);
        person.FillColor = Color.Red;
        person.Position = PersonPosition;
        target.Draw(person);
    }

}