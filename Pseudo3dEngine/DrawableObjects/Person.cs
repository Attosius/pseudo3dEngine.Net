using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;

namespace Pseudo3dEngine.DrawableObjects;

public class Person : Object2d
{

    public Vector2f PersonPosition { get; set; } = new Vector2f(600, 550);
    public float Radius { get; set; } = 10;
    public Vector2f Center => new Vector2f(PersonPosition.X + Radius, PersonPosition.Y + Radius);
    public float Direction { get; set; } = -(float)Math.PI; // 1f * (float)Math.PI / 180;
    public float Speed = 100f;
    public float SpeedTurn = 1f;

    public float SpeedStrafe = 200f;
    //public float DistanceView = 200f;

    public Person GetScaledForMapPerson()
    {
        var mappedPerson = Copy();
        var scale = 0.2f;
        mappedPerson.Name = "mappedPerson";
        mappedPerson.Radius *= scale;
        mappedPerson.PersonPosition *= scale;
        mappedPerson.Speed *= scale;
        mappedPerson.SpeedTurn *= scale;
        mappedPerson.SpeedStrafe *= scale;
        return mappedPerson;

    }

    public float DirectionDegree => Direction * 180 / (float)Math.PI;

    public Person()
    {
        Name = "Person";
        //MappedPerson = Copy();
    }
    public Person Copy()
    {
        // TODO fix serialize every frame
        var serialized = JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings()
        {
        });
        var person = JsonConvert.DeserializeObject<Person>(serialized);
        return person;
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        DrawPerson(target);

        //CenterCamera = new Vector2f(PersonPosition.X + Radius, PersonPosition.Y + Radius);

        //var viewSector = new Object2d();
        //viewSector.Points.Add(CenterCamera);

        //var leftViewAngle = Direction - Fov / 2;
        //var leftPoint = Helper.GetPointAtAngleAndDistance(CenterCamera, leftViewAngle, DistanceView);
        //viewSector.Points.Add(leftPoint);

        //var delta = Fov / 10;
        //var currentAngle = leftViewAngle;
        //for (int i = 0; i < 10; i++)
        //{
        //    var point = Helper.GetPointAtAngleAndDistance(CenterCamera, currentAngle, DistanceView);
        //    currentAngle += delta;
        //    viewSector.Points.Add(point);
        //}

        //var rightViewAngle = Direction + Fov / 2;
        //var rightPoint = Helper.GetPointAtAngleAndDistance(CenterCamera, rightViewAngle, DistanceView);
        //viewSector.Points.Add(rightPoint);
        //target.Draw(viewSector);

        //DrawDirection(target, states, CenterCamera);

        //
        //var deltaRay = Fov / 100;
        //for (int i = 0; i < 100; i++)
        //{
        //    currentAngle = leftViewAngle + deltaRay * i;
        //    var point = Helper.GetPointAtAngleAndDistance(CenterCamera, currentAngle, DistanceView);
        //    // center, point
        //}
    }


    private void DrawPerson(RenderTarget target)
    {
        var person = new CircleShape(Radius);
        person.FillColor = Color.Red;
        person.Position = PersonPosition;
        target.Draw(person);
    }

}