using System;

namespace VotiroGarage
{
    //Copy-paste from the task
    public enum Color
    {
        Red,
        Blue,
        Green,
        Yellow,
        Black,
        White
    }

    public abstract class Car
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public int FuelTank { get; set; }
    }

    public interface IGarageLog
    {
        void Log(string message);
    }

    public class GarageLog : IGarageLog
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
