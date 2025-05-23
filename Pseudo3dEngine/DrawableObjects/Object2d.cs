using System.Diagnostics;
using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine.DrawableObjects;

[DebuggerDisplay("{Name}")]
public class Object2d : Drawable
{
    public static int Counter = 0;
    
    public List<Vector2f> Points = new();
    public string Name = $"NewObj_{++Counter}";
    public Vector2f Position = new(0, 0);
    public Color FillColor = new Color(255, 175, 174, 100);
    public Color OutlineColor = new Color(255, 255, 255);
    public float OutlineThickness = 2;

    public virtual bool IsRayCrossingObject((Vector2f first, Vector2f second) segmentRay, out Vector2f crossPoint)
    {
        crossPoint = default;
        var isCross = false;
        var distance = float.MaxValue;
        if (Name == "Five")
        {

        }
        var segmentObj = (first: Points[^1], second: Points[0]); // points[points.Count - 1]
        for (var j = 0; j < Points.Count; j++)
        {
            var isCurrentCross = IsSegmentsCrossing(segmentRay, segmentObj, out var currentCrossPoint);

            if (isCurrentCross)
            {
                isCross = true;
                //var currentDistance = Math.Abs(currentCrossPoint.X - segmentRay.first.X) + Math.Abs(currentCrossPoint.Y - segmentRay.first.Y);
                var currentDistance = segmentRay.first.ManhattanDistance(currentCrossPoint);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    crossPoint = currentCrossPoint;
                }
            }
            
            if (j + 1 == Points.Count)
            {
                break;
            }
            segmentObj = (first: Points[j], second: Points[j + 1]);
        }
        
        return isCross;
    }

    public virtual void Draw(RenderTarget target, RenderStates states)
    {
        var convexShape = new ConvexShape();
        convexShape.SetPointCount((uint)Points.Count);
        for (var i = 0; i < Points.Count; i++)
        {
            var vector2F = Points[i];
            convexShape.SetPoint((uint)i, vector2F);
        }
        convexShape.FillColor = FillColor;
        convexShape.OutlineThickness = OutlineThickness;
        convexShape.OutlineColor = OutlineColor;
        convexShape.Position = Position;
        target.Draw(convexShape);
    }

    public const double Epsilon = 1e-6;

    public static bool DoubleEquals(double a, double b)
    {
        return Math.Abs(a - b) < Epsilon;
    }

    public static bool IsSegmentsCrossing((Vector2f first, Vector2f second) segment1, (Vector2f first, Vector2f second) segment2, out Vector2f vector2F)
    {
        vector2F = new Vector2f(0, 0);
        var directionVector1 = segment1.second - segment1.first;
        var directionVector2 = segment2.second - segment2.first;

        float denominator = Cross(directionVector1, directionVector2);

        #region параллельность не проверяем

        // более точное вычисление при параллельности и коллинеарности
        //if (Math.Abs(denominator) < Epsilon)
        //{
        //    // Отрезки параллельны или коллинеарны. Проверяем, пересекаются ли проекции.
        //    if (Math.Max(segment1.first.X, segment1.second.X) < Math.Min(segment2.first.X, segment2.second.X) ||
        //        Math.Max(segment1.first.Y, segment1.second.Y) < Math.Min(segment2.first.Y, segment2.second.Y) ||
        //        Math.Max(segment2.first.X, segment2.second.X) < Math.Min(segment1.first.X, segment1.second.X) ||
        //        Math.Max(segment2.first.Y, segment2.second.Y) < Math.Min(segment1.first.Y, segment1.second.Y))
        //    {
        //        // Отрезки не пересекаются
        //        return false;
        //    }

        //    // Отрезки коллинеарны и пересекаются.  Нужно выбрать точку пересечения.
        //    // (Выбор точки зависит от требований к задаче)
        //    vector2F = segment1.first; // Пример: возвращаем первую точку первого отрезка
        //    return true;
        //}
        #endregion


        var crossProduct1 = Cross(directionVector1, segment2.first - segment1.first);
        var crossProduct2 = Cross(directionVector1, segment2.second - segment1.first);

        if (Sign(crossProduct1) == Sign(crossProduct2) || DoubleEquals(crossProduct1, 0.0) || DoubleEquals(crossProduct2, 0.0))
        {
            return false;
        }

        var crossProduct3 = Cross(directionVector2, segment1.first - segment2.first);
        var crossProduct4 = Cross(directionVector2, segment1.second - segment2.first);

        if (Sign(crossProduct3) == Sign(crossProduct4) || DoubleEquals(crossProduct3, 0.0) || DoubleEquals(crossProduct4, 0.0))
        {
            return false;
        }


        var t = crossProduct3 / denominator;
        // Вычисляем точку пересечения
        vector2F.X = segment1.first.X + directionVector1.X * t;
        vector2F.Y = segment1.first.Y + directionVector1.Y * t;

        // вариант с хабра, немного медленней
        // if (DoubleEquals(crossProduct1, crossProduct2))
        //{
        //    //// fix divizion by zero
        //    return false;
        //}
        //vector2F.X = segment1.first.X + directionVector1.X * Math.Abs(crossProduct3) / Math.Abs(crossProduct4 - crossProduct3);
        //vector2F.Y = segment1.first.Y + directionVector1.Y * Math.Abs(crossProduct3) / Math.Abs(crossProduct4 - crossProduct3);

        return true;
    }



    public static float Cross(Vector2f a, Vector2f b)
    {
        return a.X * b.Y - a.Y * b.X;
    }

    public static int Sign(float a)
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
}