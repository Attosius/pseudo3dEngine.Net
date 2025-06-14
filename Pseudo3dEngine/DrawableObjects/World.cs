using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine.DrawableObjects;

public class World : Drawable
{
    public List<Object2d> Objects = new List<Object2d>();
    public Person? Person { get; set; }

    public World()
    {
        var scale = 0.2f;
        var positionMapShift = new Vector2f(10, -10);
        Object2d square;
        square = new Object2d
        {
            Name = "Small",
            //Scale = new Vector2f(0.5f, 0.5f),
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(125, 150));
        square.Points.Add(new Vector2f(200, 150));
        square.Points.Add(new Vector2f(200, 200));
        square.Points.Add(new Vector2f(125, 200));
        Objects.Add(square);

        var squareScaled = new Object2d
        {
            Name = "SmallScale",
            FillColor = Color.Red,
            //Position = positionMapShift,
            Type = Object2dTypes.MapWall
        };
        square.Points.ForEach(o => squareScaled.Points.Add(new Vector2f(o.X * scale, o.Y * scale)));
        Objects.Add(squareScaled);
        ///////////////////
        

        square = new Object2d
        {
            Name = "Rect",
            //Scale = new Vector2f(0.5f, 0.5f),
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(250, 150));
        square.Points.Add(new Vector2f(350, 150));
        square.Points.Add(new Vector2f(350, 175));
        square.Points.Add(new Vector2f(250, 175));
        Objects.Add(square);

        squareScaled = new Object2d
        {
            Name = "RectScale",
            FillColor = Color.Red,
            //Position = positionMapShift,
            Type = Object2dTypes.MapWall
        };
        square.Points.ForEach(o => squareScaled.Points.Add(new Vector2f(o.X * scale, o.Y * scale)));
        Objects.Add(squareScaled);

        /////////////////////////
        square = new Object2d()
        {
            Name = "Third",
            //Scale = new Vector2f(0.5f, 0.5f),
            Type = Object2dTypes.Wall
        }; ;
        square.Points.Add(new Vector2f(450, 150));
        square.Points.Add(new Vector2f(650, 250));
        square.Points.Add(new Vector2f(650, 275));
        square.Points.Add(new Vector2f(450, 275));
        Objects.Add(square);

        squareScaled = new Object2d
        {
            Name = "ThirdScale",
            FillColor = Color.Red,
            Type = Object2dTypes.MapWall
        };
        square.Points.ForEach(o => squareScaled.Points.Add(new Vector2f(o.X * scale, o.Y * scale)));
        Objects.Add(squareScaled);

        /////////////////

        square = new Object2d
        {
            Name = "Five",
            //Scale = new Vector2f(0.5f, 0.5f),
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(250, 450));
        square.Points.Add(new Vector2f(350, 550));
        square.Points.Add(new Vector2f(350, 575));
        square.Points.Add(new Vector2f(250, 575));
        square.Points.Add(new Vector2f(150, 475));
        Objects.Add(square);

        squareScaled = new Object2d
        {
            Name = "FiveScale",
            FillColor = Color.Red,
            Type = Object2dTypes.MapWall
        };
        square.Points.ForEach(o => squareScaled.Points.Add(new Vector2f(o.X * scale, o.Y * scale)));
        Objects.Add(squareScaled);


        square = new Object2d()
        {
            Name = "Triangle",
            //Scale = new Vector2f(0.5f, 0.5f),
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(350, 650));
        square.Points.Add(new Vector2f(350, 750));
        square.Points.Add(new Vector2f(250, 775));
        Objects.Add(square);

        squareScaled = new Object2d
        {
            Name = "TriangleScale",
            FillColor = Color.Red,
            Type = Object2dTypes.MapWall
        };
        square.Points.ForEach(o => squareScaled.Points.Add(new Vector2f(o.X * scale, o.Y * scale)));
        Objects.Add(squareScaled);

        var circle2d = new Circle2d(50)
        {
            Name = "Circle",
            //Scale = new Vector2f(0.5f, 0.5f),
            Type = Object2dTypes.Wall
        };
        circle2d.SetCenter(new Vector2f(150, 300));
        Objects.Add(circle2d);


        var circle2dScaled = new Circle2d(circle2d.Radius * scale)
        {
            Name = "CircleScale",
            FillColor = Color.Red,
            Type = Object2dTypes.MapWall
        };
        circle2dScaled.SetCenter(new Vector2f(circle2d.Center.X * scale, circle2d.Center.Y * scale));
        Objects.Add(circle2dScaled);
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        var drawableObjects = new HashSet<Object2dTypes>
        {
            Object2dTypes.Floor,
            Object2dTypes.MapWall,
            Object2dTypes.Sky
        };

        foreach (var object2d in Objects.Where(o => drawableObjects.Contains(o.Type)))
        {
            target.Draw(object2d);
        }

        if (Person == null)
        {
            return;
        }
        //target.Draw(Person);
        target.Draw(Person.GetScaledForMapPerson());
       
    }
}

public enum Object2dTypes
{
    None = 0,
    Wall = 1,
    MapWall = 2,
    Sky = 3,
    Floor = 4,
}