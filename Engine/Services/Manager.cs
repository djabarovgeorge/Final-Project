using Engine.Abstracts;
using Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Services
{
    class Manager
    {
        private ShiftSetter shiftSetter = new ConstructsShift();
        public void ManageShifts()
        {
            // set all the variables 
            var employee = shiftSetter.Excute();

            Console.WriteLine("Initial params:");
            Console.WriteLine($"NumberOfDaysOfWork: {shiftSetter.NumberOfDaysOfWork} NumberOfShiftsInDay: {shiftSetter.NumberOfShiftsInDay} NumberOfWokersInShift: {shiftSetter.NumberOfWokersInShift} NumberOfWorkers: {shiftSetter.NumberOfWorkers}");
            Console.WriteLine("Constraints:");
            for (int i = 0; i < employee.Count; i++)
            {
                Console.WriteLine($"Empyloee Number: {i + 1} -------------------------");

                foreach (var dayOfWork in employee[i].WeeklyConstraints)
                {
                    Console.WriteLine($"{dayOfWork.Key} : {dayOfWork.Value}");
                }
            }

        }

        // use out huristic mathods
        //...

        // result 
        //...
    }
}

