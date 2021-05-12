using Engine.Extensions;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Algorithms
{
    public class Rand
    {

        private List<Worker> _workersBackLog = new List<Worker>();
        private List<Day> _unresolvedShifts = new List<Day>();

        public Schedulare Execute(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            // step 1 init with all the constrains
            InitialTheSchedular(schedulare, shiftsContainer);


            // step 2 validate shifts
            ValidateInitialStep(schedulare, shiftsContainer);


            // step 3 resolve conflicts from previous step
            ResolveAndFillTheSchdulare(schedulare, shiftsContainer);

            return schedulare;
        }

        private void ResolveAndFillTheSchdulare(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            var numberOfWokersInShift = shiftsContainer.ShiftParams.NumberOfWokersInShift;

            while (!_workersBackLog.Count.Equals(0))
            {
                // get available shifts
                var days = schedulare.Days.DeepClone();
                var tempAvailableShifts = days.Select(x => new { x.Name, Shifts = x.Shifts.Where(y => y.Workers.Count < numberOfWokersInShift).ToList() }).ToList();
                var availableShifts = tempAvailableShifts.Where(x => !x.Shifts.IsNullOrEmpty()).ToList();

                // fetch shift from back log
                var backLogShiftDayToFill = availableShifts.FirstOrDefault();
                var backLogShiftToFill = new { backLogShiftDayToFill.Name, Shift = backLogShiftDayToFill.Shifts.FirstOrDefault() };

                // fetch matching shift from schedulare as was fetched from the backlog
                var schedulareDay = schedulare.Days.FirstOrDefault(x => x.Name.ContainsContent(backLogShiftDayToFill.Name));
                var schedulareShift = schedulareDay.Shifts.FirstOrDefault(x => x.Name.CompareContent(backLogShiftToFill.Shift.Name));

                AssignRandomEmployee(schedulare, schedulareDay, schedulareShift);

            }
        }

        private void AssignRandomEmployee(Schedulare schedulare, Day schedulareDay, Shift schedulareShift)
        {
            Func<Schedulare, Day, Shift, bool, bool> action = TryOrAssignEmployee;
            var isAssined = CommonLogic.TryWithRetries(10, () => TryOrAssignEmployee(schedulare, schedulareDay, schedulareShift));

            if (!isAssined) // failed to fill current shift 'schedulareShift'
            {
                TryOrAssignEmployee(schedulare, schedulareDay, schedulareShift, false);
                _unresolvedShifts.Add(new Day() { Name = schedulareDay.Name, Shifts = schedulareDay.Shifts.Where(x => x.Name.CompareContent(schedulareShift.Name)).ToList() });
            }
        }

        private bool TryOrAssignEmployee(Schedulare schedulare, Day schedulareDay, Shift schedulareShift, bool validate = true)
        {
            var randomEmployee = GetRandomEmployee();

            if(validate) // validate if the employee can enter the shift
                if (!CommonLogic.IsValidToAssign(schedulare, schedulareDay, schedulareShift, randomEmployee)) return false;

            // add worker to the schedulare 
            schedulareShift.Workers.Add(randomEmployee);

            // remove worker from the backlog
            _workersBackLog.Remove(randomEmployee);

            return true;
        }


        private void ValidateInitialStep(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            var validNumberOfWorkersInShift = shiftsContainer.ShiftParams.NumberOfWokersInShift;
            foreach (var day in schedulare.Days)
            {
                foreach (var shift in day.Shifts)
                {
                    if (validNumberOfWorkersInShift >= shift.Workers.Count()) continue;

                    var randList = GetListOfRandomNumber(shift.Workers.Count());

                    List<int> fortuneWorkerIndexList = GetFortuneWorkerIndexList(validNumberOfWorkersInShift, randList);

                    List<Worker> fortuneWorkerList = InitializeFortuneWorkers(shift, fortuneWorkerIndexList);

                    var names = fortuneWorkerList.Select(x => x.Name).ToList();
                    var workersThatWereRemoved = shift.Workers.Where(x => !names.Any(y => y.Contains(x.Name))).ToList();

                    //add workers what was not lucky to the back log
                    _workersBackLog.AddRange(workersThatWereRemoved);

                    shift.Workers = fortuneWorkerList;
                }
            }
        }

        private static List<Worker> InitializeFortuneWorkers(Shift shift, List<int> fortuneWorkerIndexList)
        {
            // init workers that stays on the shift
            var fortuneWorkerList = new List<Worker>();
            foreach (var index in fortuneWorkerIndexList)
            {
                fortuneWorkerList.Add(shift.Workers[index]);
            }

            return fortuneWorkerList;
        }

        private static List<int> GetFortuneWorkerIndexList(int validNumberOfWorkersInShift, List<double> randList)
        {
            // collect the indexs of the workers that stays on the shift
            var fortuneWorkerIndexList = new List<int>();
            for (int i = 0; i < validNumberOfWorkersInShift; i++)
            {
                var maxVal = randList.Max(x => x);
                var index = randList.FindIndex(x => x == maxVal);
                randList[index] = 0;
                fortuneWorkerIndexList.Add(index);
            }

            return fortuneWorkerIndexList;
        }

        private List<double> GetListOfRandomNumber(int count)
        {
            var randomList = new List<double>();
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                randomList.Add(random.NextDouble());
            }
            return randomList;
        }

        private Worker GetRandomEmployee()
        {
            Random random = new Random();
            int rInt = random.Next(0, _workersBackLog.Count());
            return _workersBackLog[rInt];
        }

        /// <summary>
        /// Assign the shifts as is from the constrains
        /// Can cause conflicts in schedulare
        /// </summary>
        /// <param name="schedulare"></param>
        /// <param name="shiftsContainer"></param>
        private static void InitialTheSchedular(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            foreach (var emp in shiftsContainer.EmployeeConstraints)
            {
                var currEmpName = new Worker() { Name = emp.Name };

                foreach (var empDayConstraint in emp.WeeklyConstraints)
                {
                    if (empDayConstraint.Value.Contains("Free day")) continue;

                    var constraintsDay = empDayConstraint.Key;
                    var constraintsShift = empDayConstraint.Value;

                    var schedulareDay = schedulare.Days.FirstOrDefault(x => x.Name.Contains(constraintsDay));

                    var schedulareShift = schedulareDay.Shifts.FirstOrDefault(x => x.Name.Contains(constraintsShift));

                    schedulareShift.Workers.Add(currEmpName);

                }
            }
        }
    }
}
