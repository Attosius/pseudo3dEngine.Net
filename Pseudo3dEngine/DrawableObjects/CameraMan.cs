using System;
using System.Diagnostics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static System.Formats.Asn1.AsnWriter;

namespace Pseudo3dEngine.DrawableObjects;

public class CameraMan : Drawable
{
    public float DistanceView = 300f;
    public static int Counter = 0;
    public static int RaysCount = 300;
    public List<long> Ticks = new(100);
    public int MouseViewPosition = 0;
    public double Fov { get; set; } = Math.PI / 6;

    public Vector2f CenterCamera { get; set; } // changed to map / person
    public Vector2f CenterMapCamera { get; set; } // changed to map / person
    public World? World { get; set; }
    public World2? World2 { get; set; }
    public Camera2? Camera2 { get; set; }
    public List<TextureParameters> Distances = new List<TextureParameters>();

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
        Camera2.DrawCameraView(target);
        //return;
        Distances.Clear();
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
            double len = 0;
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
                        var valueSecond = crossSegment.Value.second - crossPoint;
                        len = Math.Sqrt(valueSecond.X * valueSecond.X + valueSecond.Y * valueSecond.Y);
                        //public double Abs() => Math.Sqrt(x * x + y * y);

                    }
                }
            }
            if (crossingObject != null)
            {
                crossingObject.DistancePoints.Add((currentAngle, distanceToObject));
            }

            Distances.Add(new TextureParameters
            {
                distance = distanceToObject,
                objectName = crossingObject?.Name,
                progress = len

            });

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
            
            var sprite = new Sprite(Resources.TextureBrick, new IntRect((int)(len * 30), 0, (int)xWidth, (int)Resources.ScreenHeight));
            sprite.Position = new Vector2f(xScreenLeft, xScreenRight - height);
            sprite.Scale = new Vector2f(1f, (float)(height*2) / Resources.ScreenHeight);
            target.Draw(sprite);

            if (i % 100 == 0)
            {
                Console.WriteLine($"RayCount: {i}, vDistance: {len * 30}");
            }
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
            //Console.WriteLine($"RayCount: {i}, height: {height}");

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

        //for (int i = 0; i < Distances.Count - 1; i++)
        //{
        //    var d0 = Distances[i];
        //    var d1 = Distances[i + 1];

        //    //var h0 = Height(d0.distance, Resources.ScreenHeight);
        //    var h1 = Height(d1.distance, Resources.ScreenHeight);

        //    var s = target.Size;
        //    // Нарисуйте полигон (или rect)
        //    RectangleShape poly = new RectangleShape(new Vector2f(Resources.ScreenWidth / Distances.Count, (float)(h1.Item2 - h1.Item1)));
        //    poly.Position = new Vector2f(i * Resources.ScreenWidth / Distances.Count, h1.Item1);
        //    byte alpha = (byte)(255 * (1 - d0.distance / DistanceView));
        //    poly.FillColor = new Color(255, 174, 174, alpha);

        //    if (false)
        //        target.Draw(poly);
        //    // Если UseTextures, рисуйте текстуру

        //    if (d0.objectName != "")
        //    {
        //        //var obj = world.Objects[d0.objectName];
        //        Sprite slice = new Sprite(Resources.TextureBrick);
        //        slice.TextureRect = new IntRect((int)(d0.progress * Resources.ScreenWidth), 0, (int)(Resources.ScreenWidth / Distances.Count), (int)Resources.ScreenHeight);
        //        slice.Position = new Vector2f(i * Resources.ScreenWidth / Distances.Count, Resources.ScreenHeight / 2 - ((h1.Item2 - h1.Item1) / 2));
        //        slice.Scale = new Vector2f(1f, (float)(h1.Item2 - h1.Item1) / Resources.ScreenHeight);
        //        slice.Color = new Color(255, 255, 255, alpha);
        //        target.Draw(slice);
        //    }
        //}


        for (int i = 0; i < RaysCount-1; i++)
        {
            if (!dictDistances.TryGetValue(i, out var crossSegment))
            {
                continue;
            }
            var d1 = Distances[i + 1];
            var h1 = HeightM(d1.distance, Resources.ScreenHeight);



            //var height = Resources.ScreenHeight / crossSegment.Distance * 40;
            //var UDistance = Math.Abs(Helper.DecartDistance(crossSegment.Segment.Value.second, crossSegment.CrossPoint));
            //var oneRayLenght = Math.Abs(Helper.DecartDistance(crossSegment.Segment.Value.second, crossSegment.Segment.Value.first)) / crossSegment.RaysOnSegment;
            //var raysCountInUdist = UDistance / oneRayLenght;

            var xWidth = Resources.ScreenWidth / RaysCount;
            var xScreenLeft = i * xWidth;
            var sprite = new Sprite(Resources.TextureBrick, new IntRect((int)(d1.progress * 30), 0, (int)xWidth, (int)Resources.ScreenHeight));
            sprite.Position = new Vector2f(Resources.ScreenWidth * i / RaysCount, Resources.ScreenHeight/2 - (h1.Item2 - h1.Item1)/2);
            sprite.Scale = new Vector2f(1f, (float)(h1.Item2 - h1.Item1) / Resources.ScreenHeight);
            //target.Draw(sprite);



            //var sprite = new Sprite(Resources.TextureBrick, new IntRect((int)(raysCountInUdist * xWidth), 0, (int)xWidth, (int)height * 2));
            //var udist = uDistance * Resources.ScreenWidth;
            //sprite.setTextureRect(sf::IntRect(v_distances[i].progress * SCREEN_WIDTH, 0, SCREEN_WIDTH / DISTANCES_SEGMENTS, SCREEN_HEIGHT));// двигаем текстуру по progress
            //sprite.setPosition(sf::Vector2f(SCREEN_WIDTH * (i) / DISTANCES_SEGMENTS, SCREEN_HEIGHT / 2 - (h2 - h1) / 2)); // абсолютная позиция
            //var scale = height * 2 / Resources.ScreenHeight;
            
            //if (i % 100 == 0)
            //{
            //    Console.WriteLine($"RayCount: {i}, vDistance: {Distances[i+1].distance}, sprite.Scale {sprite.Scale}");
            //}


            var objects3d = new Object2d();
            //var colorRate2 = (byte)Math.Abs(170 - (distanceToObject / 5)); // 170-gray, 5 - speed of shadow
            var colorRate = (byte)200;
            objects3d.FillColor = new Color(colorRate, colorRate, colorRate, 0);
            objects3d.OutlineThickness = 1;
            var xScreenRight = (float)Resources.ScreenHeight / 2;
            //objects3d.Position = new Vector2f(xScreenLeft, xScreenRight);
            objects3d.Points.Add(new Vector2f(xScreenLeft, h1.Item1));
            objects3d.Points.Add(new Vector2f(xScreenLeft + xWidth, h1.Item1));
            objects3d.Points.Add(new Vector2f(xScreenLeft + xWidth, h1.Item2));
            objects3d.Points.Add(new Vector2f(xScreenLeft, h1.Item2));

            var text = new Text($"{d1.progress:0.0}", Resources.FontCourerNew, 8);
            text.Position = new Vector2f(xScreenLeft, h1.Item2);
            text.FillColor = Color.Black;
            target.Draw(text);

            //target.Draw(objects3d);


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


    private (float, float) Height(double distance, uint windowHeight)
    {
        // (float)(nScreenHeight/2.0) - nScreenHeight / ((float)fDistanceToWall);
        //((nScreenHeight / 2) - (nScreenHeight / (1 * dDistanceToWall)));
        //h.first = (1 - 1 / distance) * SCREEN_HEIGHT / 2;
        //h.second = (1 + 1 / distance) * SCREEN_HEIGHT / 2;

        float h0 = (float)((1 - 1 / distance) * windowHeight / 2);
        float h1 = (float)((1 + 1 / distance) * windowHeight / 2);
        var h = (float)(1 - distance / 300) * windowHeight;
        //h0 = windowHeight / 2 - h / 2;
        //h1 = windowHeight / 2 + h / 2;
        return (h0, h1);
    }
    //pair<double, double> height(double distance)
    //{
    //    pair<double, double> h = { 0, 0 };
    //    double scale = max(0.0, (1 - distance / d_depth)); // Нормализуем расстояние относительно d_depth
    //    h.first = SCREEN_HEIGHT / 2 * (1 - scale);
    //    h.second = SCREEN_HEIGHT / 2 * (1 + scale);
    //    return h;
    //}
    private (float, float) HeightM(double distance, uint windowHeight)
    {
        // (float)(nScreenHeight/2.0) - nScreenHeight / ((float)fDistanceToWall);
        //((nScreenHeight / 2) - (nScreenHeight / (1 * dDistanceToWall)));
        //h.first = (1 - 1 / distance) * SCREEN_HEIGHT / 2;
        //h.second = (1 + 1 / distance) * SCREEN_HEIGHT / 2;

        float h01 = (float)((1 - 1 / distance) * windowHeight / 2);
        float h11 = (float)((1 + 1 / distance) * windowHeight / 2);
        var h = (float)(1 - distance / 300) * windowHeight;
        //var h = (float)(1 - (distance - 50) / 250) * windowHeight;
        //SCREEN_HEIGHT * (1 - (distance - 50) / 250)
        float h0 = windowHeight / 2 - h / 2;
        float h1 = windowHeight / 2 + h / 2;



        float scale = (float)Math.Max(0.0, (1 - distance / 300));

        //SCREEN_HEIGHT * (1 - (distance - 50) / 250)
        h0 = windowHeight / 2 * (1 - scale);
        h1 = windowHeight / 2 * (1 + scale);

        return (h0, h1);
    }


}



public struct Point2D
{
    public double x, y;

    public Point2D(double x, double y) { this.x = x; this.y = y; }

    public static Point2D operator +(Point2D a, Point2D b) => new Point2D(a.x + b.x, a.y + b.y);
    public static Point2D operator -(Point2D a, Point2D b) => new Point2D(a.x - b.x, a.y - b.y);
    public Point2D Normalize()
    {
        double len = Math.Sqrt(x * x + y * y);
        return new Point2D(x / len, y / len);
    }
    public double Abs() => Math.Sqrt(x * x + y * y);
}

public static class Geometry
{
    static int Sign(double n) => n >= 0 ? 1 : -1;

    static double Cross(Point2D p1, Point2D p2) => p1.x * p2.y - p1.y * p2.x;

    // Возвращает true, если есть пересечение, и в point записывает координаты точки пересечения
    public static bool SegmentsCrossing(
        (Point2D, Point2D) s1,
        (Point2D, Point2D) s2,
        out Point2D point)
    {
        point = new Point2D();
        var cut1 = s1.Item2 - s1.Item1;
        var cut2 = s2.Item2 - s2.Item1;

        double prod1 = Cross(cut1, s2.Item1 - s1.Item1);
        double prod2 = Cross(cut1, s2.Item2 - s1.Item1);

        if (Sign(prod1) == Sign(prod2) || prod1 == 0 || prod2 == 0)
            return false;

        prod1 = Cross(cut2, s1.Item1 - s2.Item1);
        prod2 = Cross(cut2, s1.Item2 - s2.Item1);

        if (Sign(prod1) == Sign(prod2) || prod1 == 0 || prod2 == 0)
            return false;

        double t = Math.Abs(prod1) / Math.Abs(prod2 - prod1);
        point = new Point2D(
            s1.Item1.x + cut1.x * t,
            s1.Item1.y + cut1.y * t
        );
        return true;
    }
}


public class Object2d2 : Drawable
{
    public List<Point2D> Nodes { get; set; } // v_points2D
    public Point2D Position { get; set; } // p_position
    public Point2D Velocity { get; set; } // p_velocity
    public string Name { get; set; } // s_name
    private Texture T_texture; // T_texture
    private string s_texture; // s_texture
    private bool texture_loaded = false; // texture_loaded
    private float SCALE = 40;

    public Object2d2(Point2D position = new Point2D(), List<Point2D> points = null, string texture = "WALL_TEXTURE", Point2D velocity = new Point2D())
    {
        Position = position;
        Nodes = points ?? new List<Point2D>();
        Velocity = velocity;
        s_texture = texture;
    }

    public double X { get { return Position.x; } } // x()
    public double Y { get { return Position.y; } } // y()

    public void SetPosition(Point2D position) { Position = position; } // setPosition
    public void Shift(Point2D vector) { Position += vector; } // shift

    public void SetName(string name) { Name = name; } // setName
    public string GetName() { return Name; } // getName

    public Texture LoadTexture() // loadTexture
    {
        if (texture_loaded)
        {
            return T_texture;
        }
        texture_loaded = true;
        T_texture = new Texture(s_texture);
        if (T_texture.IsInvalid)
        {
            texture_loaded = false;
        }
        T_texture.Repeated = true;
        return T_texture;
    }

    public virtual void Draw(RenderTarget target, RenderStates states)
    {
        ConvexShape polygon = new ConvexShape((uint)Nodes.Count);
        for (int i = 0; i < Nodes.Count; i++)
        {
            polygon.SetPoint((uint)i, new Vector2f((float)Nodes[i].x * SCALE, (float)Nodes[i].y * SCALE));
        }
        polygon.OutlineColor = Color.Black;
        //polygon.FillColor = FILL_COLOR;
        //polygon.OutlineThickness = OUTLINE_THICKNESS;
        polygon.Position = new Vector2f((float)X * SCALE, (float)Y * SCALE);
        target.Draw(polygon);
    }

    // Static method for segment intersection is moved to a separate Geometry class (see previous answer)
}

public class Camera2 : Object2d2
{
    public double direction = 0;
    public double fieldOfView = Math.PI / 6;
    public double depth;
    public int distancesSegments = 50; // Как в оригинале DISTANCES_SEGMENTS
    public List<TextureParameters> Distances = new List<TextureParameters>();
    public int ShiftDirectionIndex;
    public World2 world;

    public Camera2(World2 world, Point2D pos, double dir, double fov = Math.PI / 2, double depth = 300) : base(pos)
    {
        this.world = world;
        this.direction = dir;
        this.fieldOfView = fov;
        this.depth = depth;
    }

    public void UpdateDistances()
    {
        Distances.Clear();

        double maxScalarSum = 0;
        int directionMaxScalarSum = 0;
        Point2D shiftDir = new Point2D(0, 0);
        var j = 0;
        for (int i = 0; i < 2 * Math.PI / fieldOfView * distancesSegments; i++)
        {
            j++;
            double left = direction - fieldOfView / 2;
            double rayDir = direction + (((double)i / distancesSegments - 0.5) * fieldOfView);

            var rayEnd = new Point2D(X + depth * Math.Cos(rayDir), Y + depth * Math.Sin(rayDir));
            var segment1 = (new Point2D(X, Y), rayEnd);

            Point2D nearCross = rayEnd; // точка конца луча
            string objName = "";
            double progress = 0;
            double len = 0;

            foreach (var objKvp in world.Objects)
            {
                var obj = objKvp.Value;
                if (obj == this || obj.Nodes.Count < 2) continue;

                var nodeList = obj.Nodes;
                for (int k = 0; k < nodeList.Count; k++)
                {
                    var p1 = obj.Position + nodeList[k];
                    var p2 = obj.Position + nodeList[(k + 1) % nodeList.Count];
                    if (Geometry.SegmentsCrossing(segment1, (p1, p2), out var crossPoint))
                    {
                        if ((nearCross - Position).Abs() > (crossPoint - Position).Abs())
                        {
                            nearCross = crossPoint;
                            objName = obj.Name;
                            len = (p2 - nearCross).Abs();
                        }
                    }
                }
            }

            Distances.Add(new TextureParameters
            {
                distance = (Position - nearCross).Abs(),
                progress = len, // по желанию: progress можно дополнительно нормализовать
                objectName = objName
            });
        }
    }
    public void DrawCameraView(RenderTarget window)
    {
        if (Distances.Count == 0)
            UpdateDistances();
        return;
        // Нарисовать небо и пол (sprite sky/floor)
        if (true)
        {
            Sprite sky = new Sprite(Resources.TextureSky);
            sky.TextureRect = new IntRect((int)(direction * window.Size.X / 2), 1080 - (int)window.Size.Y, (int)window.Size.X, 1080);
            sky.Position = new Vector2f(0, 0);
            window.Draw(sky);

            Sprite floor = new Sprite(Resources.TextureSky);
            floor.TextureRect = new IntRect(0, 0, (int)window.Size.X, 1080);
            floor.Position = new Vector2f(0, window.Size.Y / 2);
            window.Draw(floor);
        }

        // для каждого луча:
        for (int i = 0; i < Distances.Count - 1; i++)
        {
            var d0 = Distances[i];
            var d1 = Distances[i + 1];

            var h0 = Height(d0.distance, window.Size.Y);
            var h1 = Height(d1.distance, window.Size.Y);

            // Нарисуйте полигон (или rect)
            RectangleShape poly = new RectangleShape(new Vector2f(window.Size.X / Distances.Count, (float)(h1.Item2 - h1.Item1)));
            poly.Position = new Vector2f(i * window.Size.X / Distances.Count, h1.Item1);
            byte alpha = (byte)(255 * (1 - d0.distance / depth));
            poly.FillColor = new Color(255, 174, 174, alpha);

            //if (!UseTextures)
            //    window.Draw(poly);
            // Если UseTextures, рисуйте текстуру

            if (d0.objectName != "")
            {
                var obj = world.Objects[d0.objectName];
                Sprite slice = new Sprite(obj.LoadTexture());
                slice.TextureRect = new IntRect((int)(d0.progress * window.Size.X), 0, (int)(window.Size.X / Distances.Count), (int)window.Size.Y);
                slice.Position = new Vector2f(i * window.Size.X / Distances.Count, window.Size.Y / 2 - ((h1.Item2 - h1.Item1) / 2));
                slice.Scale = new Vector2f(1f, (float)(h1.Item2 - h1.Item1) / window.Size.Y);
                slice.Color = new Color(255, 255, 255, alpha);
                window.Draw(slice);
            }
        }
    }

    private (float, float) Height(double distance, uint windowHeight)
    {
        float h0 = (float)((1 - 1 / distance) * windowHeight / 2);
        float h1 = (float)((1 + 1 / distance) * windowHeight / 2);
        return (h0, h1);
    }

}

public class TextureParameters
{
    public double distance;
    public double progress;
    public string objectName;
}
public class World2
{
    public Dictionary<string, Object2d2> Objects = new Dictionary<string, Object2d2>();

    public void AddObject(string name, Object2d2 obj) { Objects[name] = obj; }
}