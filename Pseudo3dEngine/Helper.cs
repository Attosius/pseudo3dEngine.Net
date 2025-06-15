using SFML.System;

namespace Pseudo3dEngine
{
    public static class Helper
    {
        public static Vector2f GetPointAtAngleAndDistance(Vector2f center, double angle, float distance)
        {
            var x = center.X + (float)Math.Sin(angle) * distance;
            var y = center.Y + (float)Math.Cos(angle) * distance;
            return new Vector2f(x, y);
        }

        public static Vector2f ScalarMult(this Vector2f a, double num)
        {
            return new Vector2f(a.X * (float)num, a.Y * (float)num);
        }
        public static float ManhattanDistance(this Vector2f a, Vector2f b)
        {
            return Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y);
        }
        //public static float ManhattanDistance(this Vector2f a, Vector2f b)
        //{
        //    return Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y);
        //}

        //public static float DecartDistance(this Vector2f a, Vector2f b)
        public static float DecartDistance(this Vector2f a, Vector2f b)
        {
            return MathF.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
        }
    }
}
