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
        //var positionMapShift = new Vector2f(10, -10);
        Object2d? square = null;
        square = new Object2d
        {
            Name = "RectTowerLeft",
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(125, 150));
        square.Points.Add(new Vector2f(200, 150));
        square.Points.Add(new Vector2f(200, 200));
        square.Points.Add(new Vector2f(125, 200));
        //Objects.Add(square);
        //AddSquareScaled(square, scale);
        /////////////////////
        ///
        square = new Object2d
        {
            Name = "RectTowerLeft",
            Type = Object2dTypes.Wall,
            Position = new Vector2f(125, 150)
        };
        square.Points.Add(new Vector2f(0, 0));
        square.Points.Add(new Vector2f(75, 0));
        square.Points.Add(new Vector2f(75, 50));
        square.Points.Add(new Vector2f(0, 50));
        Objects.Add(square);
        AddSquareScaled(square, scale);
        /////////////////////

        square = new Object2d
        {
            Name = "RectTowerRight",
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(1025, 150));
        square.Points.Add(new Vector2f(1100, 150));
        square.Points.Add(new Vector2f(1100, 200));
        square.Points.Add(new Vector2f(1025, 200));
        Objects.Add(square);
        AddSquareScaled(square, scale);
        /////////////////////

        square = new Object2d
        {
            Name = "RectLoongFrontMains",
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(200, 150));
        square.Points.Add(new Vector2f(1025, 150));
        square.Points.Add(new Vector2f(1025, 175));
        square.Points.Add(new Vector2f(200, 175));
        Objects.Add(square);
        AddSquareScaled(square, scale);

        ///////////////

        square = new Object2d
        {
            Name = "RectVerticalLeft",
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(150, 200));
        square.Points.Add(new Vector2f(175, 200));
        square.Points.Add(new Vector2f(175, 925));
        square.Points.Add(new Vector2f(150, 925));
        Objects.Add(square);
        AddSquareScaled(square, scale);

        square = new Object2d
        {
            Name = "RectVerticalRight",
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(1050, 200));
        square.Points.Add(new Vector2f(1075, 200));
        square.Points.Add(new Vector2f(1075, 925));
        square.Points.Add(new Vector2f(1050, 925));
        Objects.Add(square);
        AddSquareScaled(square, scale);


        var circle2d = new Circle2d(50)
        {
            Name = "Circle",
            Type = Object2dTypes.Wall
        };
        circle2d.SetCenter(new Vector2f(325, 300));
        Objects.Add(circle2d);
        AddCircleScaled(circle2d, scale);

        square = new Object2d
        {
            Name = "SquareBigNordEast",
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(850, 250));
        square.Points.Add(new Vector2f(950, 250));
        square.Points.Add(new Vector2f(950, 350));
        square.Points.Add(new Vector2f(850, 350));
        Objects.Add(square);
        AddSquareScaled(square, scale);

        square = new Object2d
        {
            Name = "SquareNordWest",
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(450, 275));
        square.Points.Add(new Vector2f(500, 275));
        square.Points.Add(new Vector2f(500, 325));
        square.Points.Add(new Vector2f(450, 325));
        Objects.Add(square);
        AddSquareScaled(square, scale);

        //square = new Object2d
        //{
        //    Name = "SquareSmallMiddle",
        //    Type = Object2dTypes.Wall
        //};
        //square.Points.Add(new Vector2f(575, 275));
        //square.Points.Add(new Vector2f(625, 275));
        //square.Points.Add(new Vector2f(625, 325));
        //square.Points.Add(new Vector2f(575, 325));
        //Objects.Add(square);
        //AddSquareScaled(square, scale);

        square = new Object2d
        {
            Name = "SquareSmallMiddle",
            Type = Object2dTypes.Wall,
            Position = new Vector2f(575, 275)
        };
        square.Points.Add(new Vector2f(0, 0));
        square.Points.Add(new Vector2f(50, 0));
        square.Points.Add(new Vector2f(50, 50));
        square.Points.Add(new Vector2f(0, 50));
        Objects.Add(square);
        AddSquareScaled(square, scale);


        square = new Object2d
        {
            Name = "SquareSmallNordEast",
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(700, 275));
        square.Points.Add(new Vector2f(750, 275));
        square.Points.Add(new Vector2f(750, 325));
        square.Points.Add(new Vector2f(700, 325));
        Objects.Add(square);
        AddSquareScaled(square, scale);

        square = new Object2d
        {
            Name = "RectDown",
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(250, 400));
        square.Points.Add(new Vector2f(450, 400));
        square.Points.Add(new Vector2f(450, 450));
        square.Points.Add(new Vector2f(250, 450));
        Objects.Add(square);
        AddSquareScaled(square, scale);


        square = new Object2d
        {
            Name = "Triangle",
            Type = Object2dTypes.Wall
        };
        square.Points.Add(new Vector2f(900, 400));
        square.Points.Add(new Vector2f(925, 475));
        square.Points.Add(new Vector2f(875, 475));
        Objects.Add(square);
        AddSquareScaled(square, scale);

    }

    private void AddCircleScaled(Circle2d circle2d, float scale)
    {
        var circle2dScaled = new Circle2d(circle2d.Radius * scale)
        {
            Name = $"{circle2d.Name}_Scaled",
            FillColor = Color.Red,
            OutlineThickness = 1f,
            OutlineColor = Color.Black,
            Type = Object2dTypes.MapWall
        };
        circle2dScaled.SetCenter(new Vector2f(circle2d.Center.X * scale, circle2d.Center.Y * scale));
        Objects.Add(circle2dScaled);
    }

    private void AddSquareScaled(Object2d square, float scale)
    {
        var squareScaled = new Object2d
        {
            Name = $"{square.Name}_Scaled",
            FillColor = Color.Red,
            Position = square.Position * scale,
            OutlineThickness = 1f,
            OutlineColor = Color.Black,
            Type = Object2dTypes.MapWall
        };
        square.Points.ForEach(o => squareScaled.Points.Add(new Vector2f(o.X * scale, o.Y * scale)));
        Objects.Add(squareScaled);
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        var drawableObjects = new HashSet<Object2dTypes>
        {
            //Object2dTypes.Floor,
            //Object2dTypes.Wall,
            Object2dTypes.MapWall,
            //Object2dTypes.Sky
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