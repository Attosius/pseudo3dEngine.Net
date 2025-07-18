﻿using System;
using System.Diagnostics;
using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine.DrawableObjects;

public class CameraMan : Drawable
{
    public float DistanceView = 1400f;
    public static int Counter = 0;
    public static int RaysCount = 50;
    public List<long> Ticks = new(100);
    public int MouseViewPosition = 0;
    public double Fov { get; set; } = Math.PI / 6;

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
        //DrawSky(target);
        ///////////////////////////////// 3d
        DrawObjects(target, person);

        DrawViewSector(target, person, Object2dTypes.Wall);
        DrawViewSector(target, mapPerson, Object2dTypes.MapWall);


        //var ang = new Circle2d(10);
        //ang.FillColor = Color.Yellow;
        //var angPosition = new Vector2f(500, 400);
        //ang.Position = angPosition;
        //target.Draw(ang);

        //var point = Helper.GetPointAtAngleAndDistance(ang.CenterCamera, World.Person.DirectionRad, 100);
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
        //Resources.TextureSky.Repeated = true;
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

        var leftViewAngle = person.DirectionRad + Fov / 2; // because we turn overclock as pi
        var deltaRay = Fov / RaysCount;
        var sw = Stopwatch.StartNew();
        var objectsToCheck = World.Objects.Where(o => o.Type == object2dTypes).ToList();
        for (var i = 0; i < RaysCount; i++)
        {
            var currentAngle = leftViewAngle - deltaRay * i;
            var point = Helper.GetPointAtAngleAndDistance(person.Center, currentAngle, DistanceView);
            var pointToShowOnMap = Helper.GetPointAtAngleAndDistance(person.Center, currentAngle, DistanceView);
            var segmentRay = (first: person.Center, second: point);
            var distanceToObject = float.MaxValue;
            var isCrossing = false;
            foreach (var wordObject in objectsToCheck)
            {
                if (wordObject.IsRayCrossingObject(segmentRay, out var crossPoint, out var _))
                {
                    var currentDistance = segmentRay.first.DecartDistance(crossPoint);
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


    public class CrossSegment
    {
        public (Vector2f first, Vector2f second)? Segment { get; set; }
        public float Distance { get; set; }
        public Object2d Obj { get; set; }
        public int RaysOnSegment { get; set; }
        public Vector2f CrossPoint { get; set; }
    }


    private void DrawObjects(RenderTarget target, Person person)
    {
        var deltaRay = Fov / RaysCount;
        var objectsToCheck = World.Objects.Where(o => o.Type == Object2dTypes.Wall).ToList();
        var leftViewAngle = person.DirectionRad + Fov / 2; // because we turn overclock as pi
        objectsToCheck.ForEach(o => o.DistancePoints.Clear());
        objectsToCheck.ForEach(o => o.RayCounterList.Clear());
        objectsToCheck.ForEach(o => o.LineList.Clear());
        objectsToCheck.ForEach(o => o.RayCounter = 0);
        var dictDistances = new Dictionary<int, CrossSegment>();
        var segmCounter = new Dictionary<Vector2f, int>();
        for (var i = 0; i < RaysCount; i++)
        {
            var crossSegmentRet = new CrossSegment();
            var currentAngle = leftViewAngle - deltaRay * i;
            //while (currentAngle > Math.PI) currentAngle -= 2 * Math.PI;
            //while (currentAngle < -Math.PI) currentAngle += 2 * Math.PI;
            var point = Helper.GetPointAtAngleAndDistance(person.Center, currentAngle, DistanceView);
            var segmentRay = (first: person.Center, second: point);
            var distanceToObject = float.MaxValue;
            Object2d? crossingObject = null;
            (Vector2f first, Vector2f second)? crossSegment = null;
            foreach (var wordObject in objectsToCheck)
            {
                if (wordObject.IsRayCrossingObject(segmentRay, out var crossPoint, out var tempCrossSegment))
                {
                    //crossingObject.RayCounter++;
                    var currentDistance = segmentRay.first.DecartDistance(crossPoint);
                    if (currentDistance < distanceToObject)
                    {
                        distanceToObject = currentDistance;
                        point = crossPoint;
                        crossSegment = tempCrossSegment;
                        crossingObject = wordObject;
                        //var segmLen = Math.Abs(Helper.DecartDistance(crossSegment.Value.second, crossSegment.Value.first));
                        crossingObject.UDistance = Math.Abs(Helper.DecartDistance(crossSegment.Value.second, point));
                        crossSegmentRet.Distance = distanceToObject;
                        crossSegmentRet.Segment = crossSegment;
                        crossSegmentRet.Obj = crossingObject;
                        crossSegmentRet.CrossPoint = point;

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

            dictDistances[i] = crossSegmentRet;

            if (segmCounter.TryGetValue(crossSegment.Value.first, out var count))
            {
                segmCounter[crossSegment.Value.first]++;
            }else
            {
                segmCounter[crossSegment.Value.first] = 1;
            }


            // кажущийся_размер_в_пикселях = (высота_объекта * высота_экрана) / (2 * расстояние * tan(fov_vertical / 2))
            var originalWallHeight = 40;
            //var fov_vertical_radians = 2 * Math.Atan(Math.Tan(Fov / 2) * Resources.ScreenHeight / Resources.ScreenWidth);
            //var ang = currentAngle * 180 / Math.PI;
            //var pers = person.DirectionRad * 180 / Math.PI;
            //var coss = ((float)currentAngle - person.DirectionRad) * 180 / Math.PI;

            float distanceCorrected = distanceToObject * MathF.Cos(MathF.Abs((float)currentAngle - person.DirectionRad)); // Учет угла

            var h1 = (1 - 1 / distanceToObject) * Resources.ScreenHeight / 2;
            var h2 = (1 + 1 / distanceToObject) * Resources.ScreenHeight / 2;
            //h.first = (1 - 1 / distance) * SCREEN_HEIGHT / 2;
            //h.second = (1 + 1 / distance) * SCREEN_HEIGHT / 2;
            var height = Resources.ScreenHeight / distanceToObject * originalWallHeight;
            //var height2 = Resources.ScreenHeight / distanceToObject * originalWallHeight;
            //var uDistance = Math.Abs(Helper.DecartDistance(crossSegment.Value.second, point));
            //uDistance = crossSegment.Value.second - point);
            if (crossSegment != null)
            {
            }
            var segmLenth = Math.Abs(Helper.DecartDistance(crossSegment.Value.second, crossSegment.Value.first));
            var rayLenght = segmLenth / crossingObject.RayCounter;

            var tuple = new Tuple<int, Vector2f, float, float>(crossingObject.RayCounter, point, distanceCorrected, crossingObject.UDistance);
            crossingObject.RayCounterList.Add(tuple);
            //var height = (float)((originalWallHeight * Resources.ScreenHeight) /
            //                             (distanceCorrected * Math.Tan(fov_vertical_radians / 2)));
            //var heightCpp = (1 - 1 / distanceToObject) * Resources.ScreenHeight / 2;


            var xWidth = Resources.ScreenWidth / RaysCount;
            var xScreenLeft = i * xWidth;
            var objects3d = new Object2d();
            //var colorRate2 = (byte)Math.Abs(170 - (distanceToObject / 5)); // 170-gray, 5 - speed of shadow
            var colorRate = (byte)137;
            objects3d.FillColor = new Color(colorRate, colorRate, colorRate);
            objects3d.OutlineThickness = 0;
            var xScreenRight = (float)Resources.ScreenHeight / 2;
            //objects3d.Position = new Vector2f(xScreenLeft, xScreenRight);
            objects3d.Points.Add(new Vector2f(xScreenLeft, xScreenRight - height ));
            objects3d.Points.Add(new Vector2f(xScreenLeft + xWidth, xScreenRight - height ));
            objects3d.Points.Add(new Vector2f(xScreenLeft + xWidth, xScreenRight + height ));
            objects3d.Points.Add(new Vector2f(xScreenLeft, xScreenRight + height ));
            crossingObject.LineList.Add(objects3d);

            target.Draw(objects3d);

            Resources.TextureBrick.Repeated = true;

            //var sprite = new Sprite(Resources.TextureBrick, new IntRect((int)vDistance, 0, (int)xWidth, (int)height * 2));
            //sprite.Position = new Vector2f(xScreenLeft, xScreenRight - height);

            //var rayValue = Resources.ScreenWidth / (float)RaysCount;
            //var d = uDistance / rayValue; // количество лучей в uDistance
            //var uDistScaled = d * xWidth;
            // uDistScaled +4pix
            //var sprite = new Sprite(Resources.TextureBrick, new IntRect(int left, int top, int width, int height));
            //var sprite = new Sprite(Resources.TextureBrick, new IntRect((int)(crossingObject.UDistance * Resources.TextureBrick.Size.X), 0, (int)xWidth, (int)height * 2));
            //var udist = uDistance * Resources.ScreenWidth;
            //sprite.Position = new Vector2f(xScreenLeft, (float)Resources.ScreenHeight / 2 - height / 2);
            //target.Draw(sprite);
            //// почему-то vDistance != xWidth
            //var text = new Text($"{i},{crossingObject.UDistance:0}|", Resources.FontCourerNew, 12);
            //text.Position = new Vector2f(xScreenLeft, (float)Resources.ScreenHeight / 2 - height / 2);
            //text.FillColor = Color.Black;
            //target.Draw(text);
            //Console.WriteLine($"RayCount: {i}, vDistance: {crossingObject.UDistance}, xWidth {xWidth},s {crossSegment.Value.second},p {point}");
            Console.WriteLine($"RayCount: {i}, height: {height}");

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
        }


        //sprite.setTexture(W_world[v_distances[i].object].loadTexture()); // для луча пересечения берем объект и грузим его текстуру
        //sprite.setTextureRect(sf::IntRect(v_distances[i].progress * SCREEN_WIDTH, 0, SCREEN_WIDTH / DISTANCES_SEGMENTS, SCREEN_HEIGHT));// двигаем текстуру по progress
        //sprite.setPosition(sf::Vector2f(SCREEN_WIDTH * (i) / DISTANCES_SEGMENTS, SCREEN_HEIGHT / 2 - (h2 - h1) / 2)); // абсолютная позиция


        foreach (var crSegm in dictDistances.Values)
        {
            crSegm.RaysOnSegment = segmCounter[crSegm.Segment.Value.first];
        }

        for (int i = 0; i < RaysCount; i++)
        {
            if (!dictDistances.TryGetValue(i, out var crossSegment))
            {
                continue;
            }
            var height = Resources.ScreenHeight / crossSegment.Distance * 40;
            var UDistance = Math.Abs(Helper.DecartDistance(crossSegment.Segment.Value.second, crossSegment.CrossPoint));

            var oneRayLenght = Math.Abs(Helper.DecartDistance(crossSegment.Segment.Value.second, crossSegment.Segment.Value.first)) / crossSegment.RaysOnSegment;

            var raysCountInUdist = UDistance / oneRayLenght;


            var xWidth = Resources.ScreenWidth / RaysCount;
            var xScreenLeft = i * xWidth;
            var sprite = new Sprite(Resources.TextureBrick, new IntRect((int)(raysCountInUdist * xWidth), 0, (int)xWidth, (int)height * 2));
            //var udist = uDistance * Resources.ScreenWidth;
            
            sprite.Position = new Vector2f(xScreenLeft, (float)Resources.ScreenHeight / 2 - height);
            var scale = height * 2 / Resources.ScreenHeight;
            sprite.Scale = new Vector2f(1, scale);
            target.Draw(sprite);
            Console.WriteLine($"RayCount: {i}, vDistance: {raysCountInUdist * xWidth}, sprite.Scale {sprite.Scale}");

        }

        //foreach (var object2d in objectsToCheck)
        //{
        //    if (object2d.RayCounterList.Count == 0)
        //    {
        //        continue;
        //    }
        //    Resources.TextureBrick.Repeated = true;
        //    var xWidth =50;
        //    var i = 0;
        //    var f = object2d.LineList.First();
        //    var l = object2d.LineList.Last();
        //    var sprite = new Sprite(Resources.TextureBrick, new IntRect((int)0, 0, (int)(l.Points.First().X - f.Points.First().X), (int)l.Points.First().Y));
        //    sprite.Position = f.Points.First();
        //    Console.WriteLine(sprite.Position);
        //    target.Draw(sprite);
        //}

    }
}