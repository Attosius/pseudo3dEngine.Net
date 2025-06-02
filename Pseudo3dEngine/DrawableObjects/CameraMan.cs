using System.Diagnostics;
using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine.DrawableObjects;

public class CameraMan : Drawable
{
    public float DistanceView = 400f;
    public static int Counter = 0;
    public static int RaysCount = 100;
    public List<long> Ticks = new(100);
    public double Fov { get; set; } = 3.14 / 2;

    public Vector2f Center { get; set; } // changed to map / person
    public World? World { get; set; }

    public void Draw(RenderTarget target, RenderStates states)
    {
        Counter++;
        if (World?.Person == null)
        {
            return;
        }
        var person = World.Person;
        DrawViewSector(target, person, Object2dTypes.Wall);
        DrawViewSector(target, person.GetScaledForMapPerson(), Object2dTypes.MapWall);
        DrawSky(target);
        //target.Draw(viewSector);
    }

    private void DrawSky(RenderTarget target)
    {

        //var sky = new Object2d()
        //{
        //    Name = "Sky",
        //    Scale = new Vector2f(0.5f, 0.5f),
        //    Type = Object2dTypes.Sky,
        //    FillColor = new Color(93, 214, 255, 95),
        //    OutlineThickness = 0
        //};
        //sky.Points.Add(new Vector2f(0, 0));
        //sky.Points.Add(new Vector2f(Resources.ScreenWidth, 0));
        //sky.Points.Add(new Vector2f(Resources.ScreenWidth, Resources.SkyHeight));
        //sky.Points.Add(new Vector2f(0, Resources.SkyHeight));
        //target.Draw(sky);

        var sprite = new Sprite(Resources.TextureSky, new IntRect(0, 0, (int)Resources.ScreenWidth, (int)Resources.SkyHeight));
        sprite.Color = new Color(255, 255, 255, 195);
        target.Draw(sprite);

        var floor = new Object2d()
        {
            Name = "Floor",
            Scale = new Vector2f(0.5f, 0.5f),
            Type = Object2dTypes.Floor,
            FillColor = new Color(255, 219, 128, 95),
            OutlineThickness = 0
        };
        floor.Points.Add(new Vector2f(0, Resources.SkyHeight));
        floor.Points.Add(new Vector2f(Resources.ScreenWidth, Resources.SkyHeight));
        floor.Points.Add(new Vector2f(Resources.ScreenWidth, Resources.ScreenHeight));
        floor.Points.Add(new Vector2f(0, Resources.ScreenHeight));
        target.Draw(floor);
    }

    private void DrawViewSector(RenderTarget target, Person person, Object2dTypes object2dTypes)
    {
        if (World == null)
        {
            return;
        }
        Center = person.Center;

        var viewSector = new Object2d();
        viewSector.Points.Add(Center);

        var leftViewAngle = person.Direction - Fov / 2;
        var deltaRay = Fov / RaysCount;
        var sw = Stopwatch.StartNew();
        var objectsToCheck = World.Objects.Where(o => o.Type == object2dTypes).ToList();
        for (var i = 0; i < RaysCount; i++)
        {
            var currentAngle = leftViewAngle + deltaRay * i;
            var point = Helper.GetPointAtAngleAndDistance(Center, currentAngle, DistanceView);
            var segmentRay = (first: Center, second: point);
            var distanceToObject = float.MaxValue;
            foreach (var wordObject in objectsToCheck)
            {
                if (wordObject.IsRayCrossingObject(segmentRay, out var crossPoint))
                {
                    var currentDistance = segmentRay.first.ManhattanDistance(crossPoint);
                    if (currentDistance < distanceToObject)
                    {
                        distanceToObject = currentDistance;
                        point = crossPoint;
                    }
                }
            }

            viewSector.Points.Add(point);
        }

        //Ticks.Add(sw.ElapsedTicks);
        //if (Counter % 100 == 0)
        //{
        //    Console.WriteLine($"Middle {Ticks.Sum() / Ticks.Count: 0.00} Sw {sw.ElapsedTicks:0.000}");
        //    Ticks = new List<long>(100);
        //}
        sw.Restart();

        // shape on view sector
        for (int i = 0; i < viewSector.Points.Count - 2; i++)
        {
            var obj2d = new Object2d();
            obj2d.FillColor = new Color(255, 175, 200, 70);
            obj2d.OutlineThickness = 0;
            obj2d.Points.Add(Center);
            obj2d.Points.Add(viewSector.Points[i + 1]);
            obj2d.Points.Add(viewSector.Points[i + 2]);
            target.Draw(obj2d);
        }

        // green points on horizont
        for (int i = 0; i < viewSector.Points.Count; i++)
        {
            var crossShape = new CircleShape(1);
            crossShape.FillColor = Color.Green;
            crossShape.Position = viewSector.Points[i] - new Vector2f(1, 1);
            target.Draw(crossShape);

            //var text = new Text($"{i}", Resources.FontCourerNew, 12);
            //text.Position = viewSector.Points[i];
            //text.FillColor = Color.White;
            //target.Draw(text);
        }
    }
    
}