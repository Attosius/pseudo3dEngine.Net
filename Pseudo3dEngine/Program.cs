using System;
using System.Diagnostics;
using Pseudo3dEngine.DrawableObjects;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Pseudo3dEngine
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello, World!");
                var videoMode = new VideoMode(Resources.ScreenWidth, Resources.ScreenHeight);
                var window = new RenderWindow(videoMode, "GL HF!");
                window.Display();

                var texture = new Texture(@"d:\Projects\Pseudo3dEngine.Net\Pseudo3dEngine\cold_heart.jpeg");

                //var circleShape = new CircleShape(50);
                //circleShape.FillColor = Color.Green;

                //var rectangle = new RectangleShape(new Vector2f(120f, 50f));
                //rectangle.FillColor = new Color(255, 175, 174);
                //rectangle.Size = new Vector2f(10, 10);
                //rectangle.PersonPosition = new Vector2f(0, 0);
                //rectangle.OutlineThickness = 2;
                //rectangle.OutlineColor = new Color(255, 255, 255);

                //var line = new RectangleShape (new Vector2f(10, 133));
                //line.Rotation = 45;


                //var lineArr = new Vertex[]
                //{
                //    new Vertex(new Vector2f(10, 50)),
                //    new Vertex(new Vector2f(150, 150)),
                //    new Vertex(new Vector2f(50, 10))
                //};

                var world = new World
                {
                    Person = new Person()
                };
                var cameraMan = new CameraMan
                {
                    World = world,
                    MouseViewPosition = Mouse.GetPosition(window).X,
                };
                var mapCoordinates = new MapCoordinates(Resources.ScreenWidth, Resources.ScreenHeight);
                //var mousePosition = new MousePosition(window);
                // Start the game loop
                var sw = Stopwatch.StartNew();
                var frameCount = 0;
                var fps = 0d;
                var targetFps = 60d; // Задайте желаемый FPS
                var timeOneFrame = 1000 / targetFps; // time in ms

                var lastTime = sw.Elapsed.TotalSeconds;
                var lastFpsCalculationTime = 0d;
                while (window.IsOpen)
                {
                    var currentTime = sw.Elapsed.TotalSeconds;
                    var elapsedTime = currentTime - lastTime;


                    // Ограничение FPS (опционально). Упрощенный вариант. Более точные методы требуют более сложной реализации
                    double sleepTime = timeOneFrame - (elapsedTime * 1000);
                    //Console.WriteLine($"elapsedTime before:{elapsedTime * 1000:0.000}, {timeOneFrame}, {sleepTime}");
                    if (sleepTime > 2)
                    {
                        //Console.WriteLine($"SleepTime:{sleepTime:0.000}, {elapsedTime * 1000:0.000}");
                        //Thread.Sleep(TimeSpan.FromMilliseconds(sleepTime));
                        var endTime = sw.Elapsed.TotalMilliseconds + sleepTime;
                        //Console.WriteLine($"from:{sw.Elapsed.TotalMilliseconds:0.000}, to {endTime:0.000}");
                        while (sw.Elapsed.TotalMilliseconds < endTime)
                        {
                            // Может быть добавлена небольшая задержка для уменьшения нагрузки на CPU,
                            // но это снизит точность.  Thread.Yield() может быть полезен здесь.
                            Thread.Yield();
                        }

                        currentTime = sw.Elapsed.TotalSeconds; // Обновляем currentTime после сна
                        elapsedTime = currentTime - lastTime;
                    }
                    //Console.WriteLine($"elapsedTime after:{elapsedTime * 1000:0.000}");
                    lastTime = currentTime;


                    //frameCount++;
                    //if (frameCount > 70 && sw.ElapsedMilliseconds < 1000)
                    //{
                    //    continue;
                    //}
                    //if (sw.ElapsedMilliseconds > 1000)
                    //{
                    //    fps = frameCount;
                    //    frameCount = 0;
                    //    sw.Restart();
                    //}

                    //var elapsed = sw.ElapsedMilliseconds / (double)1000;
                    //sw.Restart();
                    //Thread.Sleep(10);

                    window.DispatchEvents();
                    window.Closed += (sender, _) => ((RenderWindow)sender!).Close();

                    InputEvents(window, cameraMan, mapCoordinates, (float)elapsedTime);

                    window.Clear();

                    //window.Draw(sprite);

                    //window.Draw(circleShape);
                    //window.Draw(rectangle);
                    //window.Draw(line);
                    //window.Draw(cam);
                    //window.Draw(lineArr, PrimitiveType.LineStrip);
                    //window.Draw(person);

                    window.Draw(cameraMan);
                    window.Draw(world);
                    window.Draw(mapCoordinates);

                    frameCount++;
                    if (sw.Elapsed.TotalSeconds - lastFpsCalculationTime > 1.0)
                    {
                        fps = frameCount / (sw.Elapsed.TotalSeconds - lastFpsCalculationTime);
                        Console.WriteLine($"FPS:{frameCount}, {sw.Elapsed.TotalSeconds}, {lastFpsCalculationTime}");
                        lastFpsCalculationTime = sw.Elapsed.TotalSeconds;
                        frameCount = 0;
                    }
                    ShowStatistic(fps, world.Person, window);
                    //window.Draw(mousePosition);

                    window.Display();
                }
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void InputEvents(RenderWindow window, CameraMan cameraMan, MapCoordinates mapCoordinates, float elapsedTime)
        {
            if (cameraMan.World?.Person == null)
            {
                return;
            }
            var person = cameraMan.World.Person;
            person.IsForceSpeed = Keyboard.IsKeyPressed(Keyboard.Key.LShift);

            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                var personPosition = person.PersonPosition;
                personPosition.X += (float)Math.Sin(person.DirectionRad) * person.Speed * elapsedTime;
                personPosition.Y += (float)Math.Cos(person.DirectionRad) * person.Speed * elapsedTime;
                person.PersonPosition = personPosition;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                var personPosition = person.PersonPosition;
                personPosition.X -= (float)Math.Sin(person.DirectionRad) * person.Speed * elapsedTime;
                personPosition.Y -= (float)Math.Cos(person.DirectionRad) * person.Speed * elapsedTime;
                person.PersonPosition = personPosition;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
            {
                var personPosition = person.PersonPosition;
                personPosition.Y -= person.SpeedStrafe * elapsedTime;
                person.PersonPosition = personPosition;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
            {
                var personPosition = person.PersonPosition;
                personPosition.Y += person.SpeedStrafe * elapsedTime;
                person.PersonPosition = personPosition;
            }


            if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
            {
                var personPosition = person.PersonPosition;
                personPosition.X -= person.SpeedStrafe * elapsedTime;
                person.PersonPosition = personPosition;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
            {
                var personPosition = person.PersonPosition;
                personPosition.X += person.SpeedStrafe * elapsedTime;
                person.PersonPosition = personPosition;
            }
            
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                // нормаль от дирекшин и также взять син и кос
                var personPosition = person.PersonPosition;
                var normDirection = person.DirectionRad - Math.PI / 2;
                personPosition.X -= (float)Math.Sin(normDirection) * person.Speed * elapsedTime;
                personPosition.Y -= (float)Math.Cos(normDirection) * person.Speed * elapsedTime;
                person.PersonPosition = personPosition;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                var personPosition = person.PersonPosition;
                var normDirection = person.DirectionRad + Math.PI / 2;
                personPosition.X -= (float)Math.Sin(normDirection) * person.Speed * elapsedTime;
                personPosition.Y -= (float)Math.Cos(normDirection) * person.Speed * elapsedTime;
                person.PersonPosition = personPosition;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.M))
            {
                mapCoordinates.Visible = !mapCoordinates.Visible;
            }
            var mousePositionNewX = Mouse.GetPosition(window).X;
            var diff = cameraMan.MouseViewPosition - mousePositionNewX;
            if (diff != 0)
            {
                cameraMan.MouseViewPosition = mousePositionNewX;
                person.DirectionRad += person.SpeedTurn * (float)elapsedTime * diff;
            }
        }

        private static void ShowStatistic(double fps, Person person, RenderWindow window)
        {
            //var fps = 1d / elapsed;
            //Console.WriteLine(fps);
            var position = Mouse.GetPosition(window);
            var text = new Text($"PersonPosition: X ({person.Center.X:000.0}) Y ({person.Center.Y:000.0})," +
                                $" angle degree: {person.DirectionDegree:000.00}" +
                                $" angle rad: {person.DirectionRad:00.00}, FPS: {fps:00.00}, Mouse: ({position.X},{position.Y})", Resources.FontCourerNew, 12);
            text.Position = new Vector2f(300, 0);
            text.FillColor = Color.White;
            //text.OutlineThickness = 0.1f;
            window.Draw(text);
        }
    }

    //public class MousePosition : Drawable
    //{
    //    private readonly WindowBase _window;
    //    public Font Font = new Font(@"d:\Projects\Experimentals\Pseudo3dEngine\arial.ttf");

    //    public MousePosition(WindowBase window)
    //    {
    //        _window = window;
    //    }
    //    public void Draw(RenderTarget target, RenderStates states)
    //    {
    //        var position = Mouse.GetPosition(_window);
    //        var text = new Text($"({position.X},{position.Y})", Font, 6);
    //        text.PersonPosition = circleShape.PersonPosition;
    //        text.FillColor = Color.White;
    //        target.Draw(text);
    //    }
    //}
}
