using Engine.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Services
{
    class ConstructsShift : ShiftSetter
    {
        public void Excute(int numberOfShiftsInDay, int numberOfDaysOfWork, int numberOfWorkersInShift)
        {
            NumberOfShiftsInDay = numberOfShiftsInDay;
            WorketNumberOfDaysOfWork = numberOfDaysOfWork;
            NumberOfWokersInShift = numberOfWorkersInShift;
            // build the constraints
            SetConstraints();
        }

        public void Excute()
        {

            // build numbet of shift in day
            SetNumberOfShiftsInDay();

            // build number of days of work
            SetNumberOfDaysOfWork();

            // build number of workers in shift
            SetNumberOfWorkersInShift();

            // build the constraints
            SetConstraints();

        }

        public override void SetNumberOfShiftsInDay()
        {
            NumberOfShiftsInDay = GenerateRandomNumberInRange(2, 4);
        }
        public override void SetNumberOfDaysOfWork()
        {
            WorketNumberOfDaysOfWork = GenerateRandomNumberInRange(2, 5);
        }
        public override void SetNumberOfWorkersInShift()
        {
            WorketNumberOfDaysOfWork = GenerateRandomNumberInRange(2, 4);
        }

        public override void SetConstraints()
        {
            throw new NotImplementedException();
        }



        private int GenerateRandomNumberInRange(int min, int max)
        {
            Random rnd = new Random();
            return rnd.Next(0, 100);
        }
    }
}
