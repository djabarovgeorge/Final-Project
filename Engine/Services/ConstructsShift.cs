using Engine.Abstracts;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Services
{
    public class ConstructsShift : ShiftSetter
    {
        private bool isInitialized = false;
        public ConstructsShift()
        {

        }
        public ConstructsShift(int numberOfShiftsInDay, int numberOfDaysOfWork, int numberOfWorkersInShift, int numberOfWorkers)
        {
            SetNumberOfShiftsInDay(numberOfShiftsInDay);
            SetNumberOfDaysOfWork(numberOfDaysOfWork);
            SetNumberOfWorkersInShift(numberOfWorkersInShift);
            SetNumberOfWorkers(numberOfWorkers);
            this.isInitialized = true;
        }


        public override ShiftsContainer Excute()
        {
            //if(!isInitialized)
            GenerateShiftParams();

            var handler = new ConstraintsHandler(NumberOfWorkers, NumberOfDaysOfWork, NumberOfShiftsInDay, NumberOfWokersInShift);

            var empConstraints = handler.MakeListOfEmployess();
            var shiftParams = new ShiftParams()
            {
                DaysOfWork = DaysOfWork,
                NumberOfDaysOfWork = NumberOfDaysOfWork,
                NumberOfShiftsInDay = NumberOfShiftsInDay,
                NumberOfWokersInShift = NumberOfWokersInShift,
                NumberOfWorkers = NumberOfWorkers
            };

            return new ShiftsContainer() { EmployeeConstraints = empConstraints, ShiftParams = shiftParams };
        }

        public override void SetNumberOfShiftsInDay(int num)
        {
            NumberOfShiftsInDay = num;
        }
        public override void SetNumberOfDaysOfWork(int num)
        {
            NumberOfDaysOfWork = num;
        }
        public override void SetNumberOfWorkersInShift(int num)
        {
            NumberOfWokersInShift = num;
        }
        public override void SetNumberOfWorkers(int num)
        {
            NumberOfWorkers = num;
        }



        private int GenerateRandomNumberInRange(int min, int max)
        {
            Random rnd = new Random();
            return rnd.Next(min, max);
        }

        public void GenerateShiftParams()
        {
            var listOfPosibleParams = new List<List<int>>();


            #region Build posible params so the shifts will suit the number of employees

            for (int NumberOfDaysOfWork = 2; NumberOfDaysOfWork <= 5; NumberOfDaysOfWork++)
            {
                for (int NumberOfShiftsInDay = 2; NumberOfShiftsInDay <= 4; NumberOfShiftsInDay++)
                {
                    for (int NumberOfWokersInShift = 2; NumberOfWokersInShift <= 4; NumberOfWokersInShift++)
                    {
                        for (int workers = 3; workers <= 30; workers++)
                        {
                            var checkIfValid = (DaysOfWork * (float)NumberOfShiftsInDay * (float)NumberOfWokersInShift) / workers;
                            var ifEnoughWorkers = (DaysOfWork * NumberOfShiftsInDay * NumberOfWokersInShift) == workers* NumberOfDaysOfWork;
                            if (checkIfValid % 2 == 0 && ifEnoughWorkers)
                            {
                                listOfPosibleParams.Add(new List<int> { NumberOfDaysOfWork, NumberOfShiftsInDay, NumberOfWokersInShift, workers });
                                //Console.WriteLine($"NumberOfDaysOfWork: {NumberOfDaysOfWork} NumberOfShiftsInDay: {NumberOfShiftsInDay} NumberOfWokersInShift: {NumberOfWokersInShift} workers: {workers}");
                                //Console.WriteLine($"[{NumberOfDaysOfWork}, {NumberOfShiftsInDay}, {NumberOfWokersInShift}, {workers}]");
                            }
                        }
                    }
                }
            }
            #endregion

            // Extract one posible combination
            var randIndex = GenerateRandomNumberInRange(0, listOfPosibleParams.Count);

            #region Debug the smallest combination
            //randIndex = 5;
            #endregion

            SetNumberOfDaysOfWork(listOfPosibleParams[randIndex][0]);
            SetNumberOfShiftsInDay(listOfPosibleParams[randIndex][1]);
            SetNumberOfWorkersInShift(listOfPosibleParams[randIndex][2]);
            SetNumberOfWorkers(listOfPosibleParams[randIndex][3]);

        }
    }
}
