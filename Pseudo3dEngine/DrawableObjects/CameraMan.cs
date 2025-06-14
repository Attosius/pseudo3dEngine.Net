using System.Diagnostics;
using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine.DrawableObjects;

public class CameraMan : Drawable
{
    public float DistanceView = 1400f;
    public static int Counter = 0;
    public static int RaysCount = 1200;
    public List<long> Ticks = new(100);
    public double Fov { get; set; } = 3.14 / 2;

    public Vector2f CenterCamera { get; set; } // changed to map / person
    public Vector2f CenterMapCamera { get; set; } // changed to map / person
    public World? World { get; set; }

    public void Draw(RenderTarget target, RenderStates states)
    {
        Counter++;
        if (World?.Person == null)
        {
            return;
        }
        var person = World.Person;
        var mapPerson = person.GetScaledForMapPerson();
        CenterCamera = person.Center;
        CenterMapCamera = mapPerson.Center;
        DrawSky(target);
        //DrawViewSector(target, person, Object2dTypes.Wall);
        DrawViewSector(target, mapPerson, Object2dTypes.MapWall);
        ///////////////////////////////// 3d
        DrawObjects(target, person);


        //var ang = new Circle2d(10);
        //ang.FillColor = Color.Yellow;
        //var angPosition = new Vector2f(500, 400);
        //ang.Position = angPosition;
        //target.Draw(ang);

        //var point = Helper.GetPointAtAngleAndDistance(ang.CenterCamera, World.Person.Direction, 100);
        //var obj2d = new Object2d();
        //obj2d.FillColor = Color.White;
        //obj2d.Points.Add(ang.CenterCamera);
        //obj2d.Points.Add(point);
        //obj2d.Points.Add(ang.CenterCamera);

        //var lineArr = new Vertex[]
        //{
        //    new Vertex(ang.CenterCamera),
        //    new Vertex(point),
        //    //new Vertex(ang.CenterCamera)
        //};
        //target.Draw(lineArr, PrimitiveType.LineStrip);


        //var ang2 = new Circle2d(10);
        //ang2.FillColor = Color.Transparent;
        //ang2.OutlineThickness = 1;
        //ang2.Position = point - new Vector2f(ang2.Radius, ang2.Radius);
        //target.Draw(ang2);

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
            //Scale = new Vector2f(0.5f, 0.5f),
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
        var viewSector = new Object2d();
        viewSector.Points.Add(person.Center);

        var leftViewAngle = person.Direction + Fov / 2; // because we turn overclock as pi
        var deltaRay = Fov / RaysCount;
        var sw = Stopwatch.StartNew();
        var objectsToCheck = World.Objects.Where(o => o.Type == object2dTypes).ToList();
        for (var i = 0; i < RaysCount; i++)
        {
            var currentAngle = leftViewAngle - deltaRay * i;
            var point = Helper.GetPointAtAngleAndDistance(person.Center, currentAngle, DistanceView);
            var pointToShowOnMap = Helper.GetPointAtAngleAndDistance(person.Center, currentAngle, 50);
            var segmentRay = (first: person.Center, second: point);
            var distanceToObject = float.MaxValue;
            var isCrossing = false;
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
                    isCrossing = true;;
                }
            }

            viewSector.Points.Add(isCrossing ? point : pointToShowOnMap);
        }

        

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
            obj2d.Points.Add(person.Center);
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
        
    }

    private void DrawObjects(RenderTarget target, Person person)
    {
        var deltaRay = Fov / RaysCount;
        var objectsToCheck = World.Objects.Where(o => o.Type == Object2dTypes.Wall).ToList();
        var leftViewAngle = person.Direction + Fov / 2; // because we turn overclock as pi
        objectsToCheck.ForEach(o => o.DistancePoints.Clear());
        for (var i = 0; i < RaysCount; i++)
        {
            var currentAngle = leftViewAngle - deltaRay * i;
            var point = Helper.GetPointAtAngleAndDistance(person.Center, currentAngle, DistanceView);
            var segmentRay = (first: person.Center, second: point);
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
            var heightCpp = (1 - 1 / distanceToObject) * Resources.ScreenHeight / 2;
            var xWeight = Resources.ScreenWidth / RaysCount;
            var xScreenLeft = i * xWeight;
            var objects3d = new Object2d();
            objects3d.FillColor = new Color(137, 137, 137);
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