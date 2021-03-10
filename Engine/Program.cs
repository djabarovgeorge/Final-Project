using Engine.Algorithms;
using Engine.Services;
using System;

namespace Engine
{
    class Program
    {
        private const string Value = "Hello World!";

        static void Main(string[] args)
        {
            // the start of our program

            Manager manager = new Manager();

            manager.Execute();


            Console.WriteLine(Value);
        }
    }
}
