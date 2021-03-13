using Engine.Abstracts;
using Engine.Algorithms;
using Engine.Models;

namespace Engine.Services
{
    class Manager
    {
        private ShiftSetter _constructsShift = new ConstructsShift();
        private RandAlgorithm _randAlgorithm = new RandAlgorithm();
        private GreedyAlgorithm _greedyAlgorithm = new GreedyAlgorithm();



        public void Execute()
        {
            var shiftsContainer = ManageShifts();

            var schedulare = new Schedulare(shiftsContainer);

            _randAlgorithm.Execute(schedulare, shiftsContainer);

            var greedySchedulare = new Schedulare(shiftsContainer);

            _greedyAlgorithm.Execute(greedySchedulare, shiftsContainer);


        }

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

