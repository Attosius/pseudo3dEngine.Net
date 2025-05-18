using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
