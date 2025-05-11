using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine.DrawableObjects;

public class Object2d : Drawable
{
    public List<Vector2f> Points = new();
    public Vector2f Position = new(0, 0);
    public Color FillColor = new Color(255, 175, 174, 100);
    public Color OutlineColor = new Color(255, 255, 255);

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
        convexShape.OutlineThickness = 2;
        convexShape.OutlineColor = OutlineColor;
        convexShape.Position = Position;
        target.Draw(convexShape);
    }
}