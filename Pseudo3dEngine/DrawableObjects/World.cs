using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine.DrawableObjects;

public class World : Drawable
{
    public List<Object2d> Objects = new List<Object2d>();
    public Person? Person { get; set; }

    public World()
    {
        var square = new Object2d
        {
            Name = "Small"
        };
        square.Points.Add(new Vector2f(25, 150));
        square.Points.Add(new Vector2f(100, 150));
        square.Points.Add(new Vector2f(100, 200));
        square.Points.Add(new Vector2f(25, 200));
        Objects.Add(square);
        square = new Object2d
        {
            Name = "Rect"
        };
        square.Points.Add(new Vector2f(150, 150));
        square.Points.Add(new Vector2f(250, 150));
        square.Points.Add(new Vector2f(250, 175));
        square.Points.Add(new Vector2f(150, 175));
        Objects.Add(square);

        square = new Object2d();
        square.Points.Add(new Vector2f(350, 150));
        square.Points.Add(new Vector2f(550, 250));
        square.Points.Add(new Vector2f(550, 275));
        square.Points.Add(new Vector2f(350, 275));
        Objects.Add(square);
        square = new Object2d
        {
            Name = "Five"
        };
        square.Points.Add(new Vector2f(250, 450));
        square.Points.Add(new Vector2f(350, 550));
        square.Points.Add(new Vector2f(350, 575));
        square.Points.Add(new Vector2f(250, 575));
        square.Points.Add(new Vector2f(150, 475));
        Objects.Add(square);

        square = new Object2d();
        square.Points.Add(new Vector2f(350, 650));
        square.Points.Add(new Vector2f(350, 750));
        square.Points.Add(new Vector2f(250, 775));
        Objects.Add(square);
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        foreach (var object2d in Objects)
        {
            target.Draw(object2d);
        }

        if (Person == null)
        {
            return;
        }
        target.Draw(Person);

       
    }
}