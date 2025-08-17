using System;
using System.Diagnostics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static System.Formats.Asn1.AsnWriter;

namespace Pseudo3dEngine.DrawableObjects;

public class CameraMan : Drawable
{
    public float DistanceView = 600f;
    public static int Counter = 0;
    public static int RaysCount = 1200;
    public List<long> Ticks = new(100);
    public int MouseViewPosition = 0;
    public double Fov { get; set; } = Math.PI / 6;

    public Vector2f CenterCamera { get; set; } // changed to map / person
    public Vector2f CenterMapCamera { get; set; } // changed to map / person
    public World? World { get; set; }
    public bool IsUsingMouse = false;

    //public List<TextureParameters> Distances = new List<TextureParameters>();

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

        ///////////////////////////////// 3d
        DrawObjects(target, person);

        //DrawViewSector(target, person, Object2dTypes.Wall);
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

        sprite = new Sprite(Resources.TextureDesert, new IntRect(0, 0, (int)Resources.ScreenWidth, (int)Resources.SkyHeight));
        sprite.Position = new Vector2f(0, Resources.SkyHeight);
        //sprite.Color = new Color(255, 255, 255, 195);
        //target.Draw(sprite);

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
                    isCrossing = true; ;
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
            var crossShape = new CircleShape(2);
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


        //Distances.Clear();
        var deltaRay = Fov / RaysCount;
        var objectsToCheck = World.Objects.Where(o => o.Type == Object2dTypes.Wall).ToList();
        var leftViewAngle = person.DirectionRad + Fov / 2; // because we turn overclock as pi

        for (var i = 0; i < RaysCount; i++)
        {
            var crossSegmentRet = new CrossSegment();
            var currentAngle = leftViewAngle - deltaRay * i;
            //while (currentAngle > Math.PI) currentAngle -= 2 * Math.PI;
            //while (currentAngle < -Math.PI) currentAngle += 2 * Math.PI;
            var point = Helper.GetPointAtAngleAndDistance(person.Center, currentAngle, DistanceView); // point at the end of our view
            var segmentRay = (first: person.Center, second: point);
            var distanceToObject = float.MaxValue;
            Object2d? crossingObject = null;
            (Vector2f first, Vector2f second)? crossSegment = null;
            double len = 0;
            foreach (var wordObject in objectsToCheck)
            {
                if (wordObject.IsRayCrossingObject(segmentRay, out var crossPoint, out var tempCrossSegment))
                {
                    var currentDistance = segmentRay.first.DecartDistance(crossPoint);
                    if (currentDistance < distanceToObject)
                    {
                        distanceToObject = currentDistance;
                        point = crossPoint;
                        crossSegment = tempCrossSegment;
                        crossingObject = wordObject;
                        len = Math.Abs(Helper.DecartDistance(crossSegment.Value.second, point));

                    }
                }
            }

            if (crossingObject == null)
            {
                continue;
            }


            // кажущийся_размер_в_пикселях = (высота_объекта * высота_экрана) / (2 * расстояние * tan(fov_vertical / 2))
            var originalWallHeight = 40;
            var heightHalf = Resources.ScreenHeight / distanceToObject * originalWallHeight;
            //var height = (float)((originalWallHeight * Resources.ScreenHeight) / (distanceCorrected * Math.Tan(fov_vertical_radians / 2)));
            //var heightCpp = (1 - 1 / distanceToObject) * Resources.ScreenHeight / 2;


            var xWidth = Resources.ScreenWidth / RaysCount;
            var xScreenLeft = i * xWidth;
            var objects3d = new Object2d();
            //var colorRate2 = (byte)Math.Abs(170 - (distanceToObject / 5)); // 170-gray, 5 - speed of shadow
            var colorRate = (byte)137;
            objects3d.FillColor = new Color(colorRate, colorRate, colorRate, 10);
            objects3d.OutlineThickness = 0;
            var yScreenMiddle = (float)Resources.ScreenHeight / 2;
            objects3d.Points.Add(new Vector2f(xScreenLeft, yScreenMiddle - heightHalf));
            objects3d.Points.Add(new Vector2f(xScreenLeft + xWidth, yScreenMiddle - heightHalf));
            objects3d.Points.Add(new Vector2f(xScreenLeft + xWidth, yScreenMiddle + heightHalf));
            objects3d.Points.Add(new Vector2f(xScreenLeft, yScreenMiddle + heightHalf));
            objects3d.OutlineThickness = 1;
            //crossingObject.LineList.Add(objects3d);


            Resources.TextureBrick.Repeated = true;

            var alpha = 255 * (1 - distanceToObject / DistanceView);
            if (alpha > 255)
                alpha = 255;
            if (alpha < 0)
                alpha = 1;
            // len = 5, 11 - и все в начале идет, а надо сместить, увеличить лен до размера полотна. может брать процент от размера экрана

            var horizonScale = Resources.TextureBrick.Size.X / 50; // 50 - size of object in px to wear all texture

            var sprite = new Sprite(Resources.TextureBrick, new IntRect((int)(len * horizonScale), 0, (int)xWidth, (int)Resources.ScreenHeight));
            sprite.Position = new Vector2f(xScreenLeft, yScreenMiddle - heightHalf);
            sprite.Scale = new Vector2f(1f, (float)(heightHalf * 2) / Resources.ScreenHeight);
            //sprite.Color = new Color(255, 255, 255, (byte)alpha);
            target.Draw(sprite);
        }



    }



}
