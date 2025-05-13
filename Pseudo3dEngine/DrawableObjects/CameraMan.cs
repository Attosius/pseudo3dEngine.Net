using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine.DrawableObjects;

public class CameraMan : Drawable
{
    public float DistanceView = 200f;

    public double Fov { get; set; } = 3.14 / 3;

    public Vector2f Center { get; set; }
    public World? World { get; set; }
    public Font Font = new Font(Path.Combine(Directory.GetCurrentDirectory(), "cour.ttf"));

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
        var deltaRay = Fov / 2;
        for (int i = 0; i < 2; i++)
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
            var segmentRay = (first: Center, second: point);
            foreach (var wordObject in World.Objects)
            {
                var points = wordObject.Points;
                var segmentObj = (first: points[^1], second: points[0]); // points[points.Count - 1]
                for (int j = 0; j < points.Count-1; j++)
                {
                    var isSegmentsCrossing = IsSegmentsCrossing(segmentRay, segmentObj, out Vector2f crossPoint);
                    if (isSegmentsCrossing)
                    {
                        Console.WriteLine($"For line {i}, cross in {crossPoint.X:0.00}, {crossPoint.Y:0.00}");
                        var crossShape = new CircleShape(5);
                        crossShape.FillColor = Color.Green;
                        crossShape.Position = crossPoint - new Vector2f(5, 5);

                        var text = new Text($"j = {j}", Font, 8);
                        text.Position = crossPoint;
                        text.FillColor = Color.White;
                        target.Draw(text);

                        target.Draw(crossShape);
                    }
                    segmentObj = (first: points[j], second: points[j+1]);
                }
            }
        }

    }

    //class Vector2f
    //{
    //    public Vector2f(float x, float y)
    //    {
    //        this.X = x;
    //        this.Y = y;
    //    }
    //    public float X;
    //    public float Y;
    //    public static Vector2f operator -(Vector2f v1, Vector2f v2) => new Vector2f(v1.X - v2.X, v1.Y - v2.Y);
    //}

    private const double Epsilon = 1e-6;

    private static bool DoubleEquals(double a, double b)
    {
        return Math.Abs(a - b) < Epsilon;
    }

    private static bool IsSegmentsCrossing((Vector2f first, Vector2f second) segment1, (Vector2f first, Vector2f second) segment2, out Vector2f vector2F)
    {
        vector2F = new Vector2f(0, 0);
        var directionVector1 = segment1.second - segment1.first;
        var directionVector2 = segment2.second - segment2.first;
        float denominator = Cross(directionVector1, directionVector2);
        if (Math.Abs(denominator) < Epsilon)
        {
            // Отрезки параллельны или коллинеарны. Проверяем, пересекаются ли проекции.
            if (Math.Max(segment1.first.X, segment1.second.X) < Math.Min(segment2.first.X, segment2.second.X) ||
                Math.Max(segment1.first.Y, segment1.second.Y) < Math.Min(segment2.first.Y, segment2.second.Y) ||
                Math.Max(segment2.first.X, segment2.second.X) < Math.Min(segment1.first.X, segment1.second.X) ||
                Math.Max(segment2.first.Y, segment2.second.Y) < Math.Min(segment1.first.Y, segment1.second.Y))
            {
                // Отрезки не пересекаются
                return false;
            }

            // Отрезки коллинеарны и пересекаются.  Нужно выбрать точку пересечения.
            // (Выбор точки зависит от требований к задаче)
            vector2F = segment1.first; // Пример: возвращаем первую точку первого отрезка
            return true;
        }

        var crossProduct1 = Cross(directionVector1, segment2.first - segment1.first);
        var crossProduct2 = Cross(directionVector1, segment2.second - segment1.first);

        if (Sign(crossProduct1) == Sign(crossProduct2) || DoubleEquals(crossProduct1, 0.0) || DoubleEquals(crossProduct2, 0.0))
        {
            return false;
        }

        var crossProduct3 = Cross(directionVector2, segment1.first - segment2.first);
        var crossProduct4 = Cross(directionVector2, segment1.second - segment2.first);

        if (Sign(crossProduct3) == Sign(crossProduct4) || DoubleEquals(crossProduct3, 0.0)|| DoubleEquals(crossProduct4, 0.0))
        {
            return false;
        }
        if (DoubleEquals(crossProduct1, crossProduct2))
        {
            //// fix divizion by zero
            return false;
        }

        //var t = (crossProduct1) / (crossProduct2 - crossProduct1);
        //vector2F.X = segment1.first.X + directionVector1.X * t;
        //vector2F.Y = segment1.first.Y + directionVector1.Y * t;

        vector2F.X = segment1.first.X + directionVector1.X * Math.Abs(crossProduct3) / Math.Abs(crossProduct4 - crossProduct3);
        vector2F.Y = segment1.first.Y + directionVector1.Y * Math.Abs(crossProduct3) / Math.Abs(crossProduct4 - crossProduct3);

        return true;
    }

    private static bool IsPointOnSegment(Vector2f point, (Vector2f first, Vector2f second) segment)
    {
        var cross = Cross(segment.second - segment.first, point - segment.first);
        if (!DoubleEquals(cross, 0d))
        {
            return false;
        }

        return point.X >= Math.Min(segment.first.X, segment.second.X) - Epsilon &&
                point.X <= Math.Max(segment.first.X, segment.second.X) + Epsilon &&
                point.Y >= Math.Min(segment.first.Y, segment.second.Y) - Epsilon &&
                point.Y <= Math.Max(segment.first.Y, segment.second.Y) + Epsilon;
     }

    private static float Cross(Vector2f a, Vector2f b)
    {
        return a.X * b.Y - a.Y * b.X;
    }

    private static int Sign(float a)
    {
        if (a > 0)
        {
            return 1;
        }

        if (a < 0)
        {
            return -1;
        }
        return 0;

    }
    // Функция для проверки, лежит ли точка на отрезке
    //private static bool is_point_on_segment(Vector2f point, (Vector2f first, Vector2f second) segment)
    //{
    //    // Сначала проверим, что точка коллинеарна отрезку
    //    if (!DoubleEquals(cross(segment.second - segment.first, point - segment.first), 0.0))
    //    {
    //        return false; // Не коллинеарна
    //    }

    //    // Теперь проверим, что точка лежит между концами отрезка (по каждой координате)
    //    return (point.X >= Math.Min(segment.first.X, segment.second.X) - Epsilon &&
    //            point.X <= Math.Max(segment.first.X, segment.second.X) + Epsilon &&
    //            point.Y >= Math.Min(segment.first.Y, segment.second.Y) - Epsilon &&
    //            point.Y <= Math.Max(segment.first.Y, segment.second.Y) + Epsilon);
    //}

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