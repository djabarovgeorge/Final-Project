using Engine.Abstracts;
using Engine.Algorithms;
using Engine.Models;
using System;

namespace Engine.Services
{
    class Manager
    {
        private ShiftSetter _constructsShift = new ConstructsShift();
        private Rand _randAlgorithm = new Rand();
        private GreedyAlgorithm _greedyAlgorithm = new GreedyAlgorithm();
        private BFS _bfs = new BFS();
        private WAStar _waStar = new WAStar();





        public void Execute()
        {
            var shiftsContainer = ManageShifts();

            var schedulare = new Schedulare(shiftsContainer);

            //_randAlgorithm.Execute(schedulare, shiftsContainer);


            //var greedySchedulare = new Schedulare(shiftsContainer);

            //_greedyAlgorithm.Execute(greedySchedulare, shiftsContainer);

            //_bfs.Execute(schedulare, shiftsContainer);

            _waStar.Execute(schedulare, shiftsContainer);


            double count = 0;
            double avarage = 0;
            var max = new { Average = 0.0, WeightContainer = new WeightContainer() };

            for (int TwoShiftsInARowWeight = 10; TwoShiftsInARowWeight < 100; TwoShiftsInARowWeight += 10)
            {
                for (int LackSatisfactionConstraintsWeight = 100; LackSatisfactionConstraintsWeight < 1000; LackSatisfactionConstraintsWeight+=10)
                {
                    for (int ShiftsInARow = 10; ShiftsInARow < 100; ShiftsInARow+=10)
                    {
                        for (int UnwantedShift = 10; UnwantedShift < 100; UnwantedShift+=10)
                        {
                            count = 0;

                            var currWheights = new WeightContainer()
                            {
                                TwoShiftsInARowWeight = TwoShiftsInARowWeight,
                                LackSatisfactionConstraintsWeight = LackSatisfactionConstraintsWeight,
                                ShiftsInARow = ShiftsInARow,
                                UnwantedShift = UnwantedShift
                            };

                            Console.WriteLine($"{TwoShiftsInARowWeight} {LackSatisfactionConstraintsWeight} {ShiftsInARow} {UnwantedShift}");

                            var runTimes = 1;

                            for (int i = 0; i < runTimes; i++)
                            {
                                shiftsContainer = ManageShifts();

                                schedulare = new Schedulare(shiftsContainer);

                                var resultSchedulare = _bfs.Execute(schedulare, shiftsContainer, currWheights);

                                count = CommonLogic.GetPercentageOfSatisfaction(resultSchedulare, shiftsContainer);
                            }
                            avarage = count / runTimes;
                            Console.WriteLine($"avarage {avarage}");
                            var localMax = new
                            {
                                Average = avarage,
                                WeightContainer = currWheights
                            };
                            max = max.Average > localMax.Average ? max : localMax;

                            Console.WriteLine($"max {max.Average} {max.WeightContainer.TwoShiftsInARowWeight} {max.WeightContainer.LackSatisfactionConstraintsWeight} {max.WeightContainer.ShiftsInARow} {max.WeightContainer.UnwantedShift}");
                        }
                    }

                }
            }
            _bfs.Execute(schedulare, shiftsContainer);

        }

        //public int TwoShiftsInARowWeight { get; set; }
        //public int LackSatisfactionConstraintsWeight { get; set; }
        //public int ShiftsInARow { get; set; }
        //public int UnwantedShift { get; set; }

        public ShiftsContainer ManageShifts()
        {
            // set all the variables 
            var shiftContainer = _constructsShift.Excute();

            #region Print Constraints
            //var employee = shiftContainer.EmployeeConstraints;
            //Console.WriteLine("Initial params:");
            //Console.WriteLine($"NumberOfDaysOfWork: {_constructsShift.NumberOfDaysOfWork} NumberOfShiftsInDay: {_constructsShift.NumberOfShiftsInDay} NumberOfWokersInShift: {_constructsShift.NumberOfWokersInShift} NumberOfWorkers: {_constructsShift.NumberOfWorkers}");
            //Console.WriteLine("Constraints:");
            //for (int i = 0; i < employee.Count; i++)
            //{
            //    Console.WriteLine($"Empyloee Number: {i + 1} -------------------------");

            //    foreach (var dayOfWork in employee[i].WeeklyConstraints)
            //    {
            //        Console.WriteLine($"{dayOfWork.Key} : {dayOfWork.Value}");
            //    }
            //}
            #endregion

            return shiftContainer;
        }


        // use out huristic mathods
        //...

        // result 
        //...
    }
}

