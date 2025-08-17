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
                

                var world = new World
                {
                    Person = new Person()
                };

                //world.Objects.ForEach(obj =>
                //{
                //    if (obj.Type == Object2dTypes.Wall)
                //    {
                //        obj.Position = obj.Position + new Vector2f(0, -150);
                //    }
                //});

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
                            Thread.Yield();
                        }

                        currentTime = sw.Elapsed.TotalSeconds; // Обновляем currentTime после сна
                        elapsedTime = currentTime - lastTime;
                    }
                    lastTime = currentTime;

                    

                    window.DispatchEvents();
                    window.Closed += (sender, _) => ((RenderWindow)sender!).Close();

                    InputEvents(window, cameraMan, mapCoordinates, (float)elapsedTime);

                    window.Clear();
                    
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
            if (diff != 0 && cameraMan.IsUsingMouse)
            {
                cameraMan.MouseViewPosition = mousePositionNewX;
                person.DirectionRad += person.SpeedTurn * (float)elapsedTime * diff;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
            {
                person.DirectionRad += person.SpeedTurn * elapsedTime;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.E))
            {
                person.DirectionRad -= person.SpeedTurn * elapsedTime;
            }

            if (Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                //Con
                cameraMan.IsUsingMouse = !cameraMan.IsUsingMouse;
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
}
