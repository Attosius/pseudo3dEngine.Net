using System.Diagnostics;
using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine.DrawableObjects;

public class CameraMan : Drawable
{
    public float DistanceView = 300f;
    public static int Counter = 0;
    public static int RaysCount = 1200;
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
        //DrawSky(target);


        var ang = new Circle2d(10);
        ang.FillColor = Color.Yellow;
        var angPosition = new Vector2f(500, 400);
        ang.Position = angPosition;
        target.Draw(ang);

        var point = Helper.GetPointAtAngleAndDistance(ang.Center, World.Person.Direction, 100);
        var obj2d = new Object2d();
        obj2d.FillColor = Color.White;
        //obj2d.OutlineThickness = 1;
        //obj2d.Position = angPosition;
        obj2d.Points.Add(ang.Center);
        obj2d.Points.Add(point);
        //obj2d.Points.Add(new Vector2f(point.X + 10, point.Y + 10));
        obj2d.Points.Add(ang.Center);
        //obj2d.Points.Add(new Vector2f(500, 400));
        //obj2d.Points.Add(new Vector2f(500, 500));
        //obj2d.Points.Add(new Vector2f(500, 510));
        //obj2d.Points.Add(new Vector2f(500, 400));
        //target.Draw(obj2d);

        var lineArr = new Vertex[]
        {
            new Vertex(ang.Center),
            new Vertex(point),
            //new Vertex(ang.Center)
        };
        target.Draw(lineArr, PrimitiveType.LineStrip);


        var ang2 = new Circle2d(10);
        ang2.FillColor = Color.Transparent;
        ang2.OutlineThickness = 1;
        ang2.Position = point - new Vector2f(ang2.Radius, ang2.Radius);
        target.Draw(ang2);

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

        var leftViewAngle = person.Direction + Fov / 2;
        var deltaRay = Fov / RaysCount;
        var sw = Stopwatch.StartNew();
        var objectsToCheck = World.Objects.Where(o => o.Type == object2dTypes).ToList();
        for (var i = 0; i < RaysCount; i++)
        {
            var currentAngle = leftViewAngle - deltaRay * i;
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


        //var objects3d = new Object2d();
        //foreach (var object2d in objectsToCheck)
        //{
        //    var obj2d = new Object2d();
        //    obj2d.FillColor = new Color(255, 175, 200, 70);
        //    obj2d.OutlineThickness = 0;
        //    obj2d.Points.Add(Center);
        //    obj2d.Points.Add(viewSector.Points[i + 1]);
        //    obj2d.Points.Add(viewSector.Points[i + 2]);
        //    target.Draw(obj2d);
        //    foreach (var object2dDistancePoint in object2d.DistancePoints)
        //    {

        //    }
        //}
        //objects3d.Points.Add(point);



        //Ticks.Add(sw.ElapsedTicks);
        //if (Counter % 100 == 0)
        //{
        //    Console.WriteLine($"Middle {Ticks.Sum() / Ticks.Count: 0.00} Sw {sw.ElapsedTicks:0.000}");
        //    Ticks = new List<long>(100);
        //}
        //sw.Restart();

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

            if (i == 1 || i == viewSector.Points.Count - 1)
            {
                var text = new Text($"{i}", Resources.FontCourerNew, 12);
                text.Position = viewSector.Points[i];
                text.FillColor = Color.White;
                target.Draw(text);
            }

        }




        if (object2dTypes != Object2dTypes.Wall)
        {
            return;
        }

        var rightViewAngle = person.Direction + Fov / 2;
        ///////////////////////////////// 3d
        objectsToCheck.ForEach(o => o.DistancePoints.Clear());
        for (var i = 0; i < RaysCount; i++)
        {
            var currentAngle = leftViewAngle - deltaRay * i;
            var point = Helper.GetPointAtAngleAndDistance(Center, currentAngle, DistanceView);
            var segmentRay = (first: Center, second: point);
            var distanceToObject = float.MaxValue;
            Object2d crossingObject = null;
            foreach (var wordObject in objectsToCheck)
            {
                if (wordObject.IsRayCrossingObject(segmentRay, out var crossPoint))
                {
                    var currentDistance = segmentRay.first.ManhattanDistance(crossPoint);
                    if (currentDistance < distanceToObject)
                    {
                        distanceToObject = currentDistance;
                        point = crossPoint;
                        crossingObject = wordObject;
                    }
                }
            }

            if (crossingObject != null)
            {
                crossingObject.DistancePoints.Add((currentAngle, distanceToObject));
            }

            if (crossingObject == null)
            {
                continue;
            }


            var height = Resources.ScreenHeight / distanceToObject * 20;
            var heightCpp = (1 - 1/distanceToObject) *  Resources.ScreenHeight / 2;
            var xWeight = Resources.ScreenWidth / RaysCount;
            var xScreenLeft = i * xWeight;
            var objects3d = new Object2d();
            objects3d.FillColor = Color.Blue;
            objects3d.OutlineThickness = 0;
            objects3d.Points.Add(new Vector2f(xScreenLeft, (float)Resources.ScreenHeight / 2 - height / 2));
            objects3d.Points.Add(new Vector2f(xScreenLeft + xWeight, (float)Resources.ScreenHeight / 2 - height / 2));
            objects3d.Points.Add(new Vector2f(xScreenLeft + xWeight, (float)Resources.ScreenHeight / 2 + height / 2));
            objects3d.Points.Add(new Vector2f(xScreenLeft, (float)Resources.ScreenHeight / 2 + height / 2));
            //if (i % 10 == 0)
            //{
            //    var text = new Text($"{new Vector2f(xScreenLeft, (float)Resources.ScreenHeight / 2 - height / 2)}", Resources.FontCourerNew, 12);
            //    text.Position = new Vector2f(xScreenLeft, (float)Resources.ScreenHeight / 2 - height / 2);
            //    text.FillColor = Color.White;
            //    target.Draw(text);

            //    var text2 = new Text($"{new Vector2f(xScreenLeft + xWeight, (float)Resources.ScreenHeight / 2 + height / 2)}", Resources.FontCourerNew, 12);
            //    text2.Position = new Vector2f(xScreenLeft + xWeight, (float)Resources.ScreenHeight / 2 + height / 2);
            //    text2.FillColor = Color.White;
            //    target.Draw(text2);
            //}
            target.Draw(objects3d);
        }


    }
    
}