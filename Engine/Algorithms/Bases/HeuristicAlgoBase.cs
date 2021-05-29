using C5;
using Engine.Extensions;
using Engine.Interfaces;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Engine.Algorithms.Bases
{
    public abstract class HeuristicAlgoBase : IAlgo
    {
        public int ALGORITHM_RUN_TIME_SECONDS => 60;

        protected const bool DEBUG = true;

        private readonly Stopwatch timer = new Stopwatch();
        protected readonly Stopwatch ExecuteStopwatch = new Stopwatch();

        protected int _twoShiftsInARowWeight = 10;
        protected int _lackSatisfactionConstraintsWeight = 40;
        protected int _shiftsInARow = 20;
        protected int _unwantedShift = 20;
        protected int _twiceOnTheSameShift = 50;

        protected bool IsFinished = false;

        protected ShiftsContainer ShiftsContainer;
        protected TreeNode<Schedulare> TreeRoot { get;  set; }
        protected SchedulareState CurrentBestSolution { get; set; }


        public abstract SchedulareState Execute(Schedulare schedulare, ShiftsContainer shiftsContainer);



        protected virtual bool IsGoal()
        {
            if (IsFinished) return true;

            if (!timer.IsRunning)
                timer.Start();

            if (timer.Elapsed.TotalSeconds > ALGORITHM_RUN_TIME_SECONDS)
            {
                timer.Stop();
                timer.Reset();
                IsFinished = true;
                return true;
            }

            return false;
        }
        protected SchedulareState GetSchedulareState(Schedulare schedulare, ShiftsContainer shiftsContainer, TreeNode<Schedulare> treeNode = null)
        {

            var weight = 0;
            // if workers have 2 shifts in the row +10
            TwoShiftsInARowWeight(schedulare, shiftsContainer, ref weight);

            // if worker did not got the shift he asked for +20
            LackSatisfactionConstraintsWeight(schedulare, shiftsContainer, ref weight);

            // if worker have 5 work days in the row +20
            var shiftsInARow = 3;
            ShiftsInARow(schedulare, shiftsContainer, ref weight, shiftsInARow);

            // if worker got shift that he did not asked +20
            UnwantedShift(schedulare, shiftsContainer, ref weight);

            // if worker assinghned twice on the same shift
            TwiceOnTheSameShift(schedulare, shiftsContainer, ref weight);

            return new SchedulareState() { Node = treeNode, Weight = weight };
        }
        protected void UpdateCurrentBestSolution(SchedulareState currState)
        {
            if (CurrentBestSolution == null)
            {
                CurrentBestSolution = currState;
                CurrentBestSolution.ExecuteTime = ExecuteStopwatch.Elapsed.TotalSeconds;
            }

            // update only if the schedulare is full
            if (!IsSchedulareFull(currState.Node.Value, ShiftsContainer)) return;

            else if (currState.Weight < CurrentBestSolution.Weight)
            {
                CurrentBestSolution = currState;
                CurrentBestSolution.ExecuteTime = ExecuteStopwatch.Elapsed.TotalSeconds;
            }
        }


        protected virtual void PrintDebugData(ShiftsContainer shiftsContainer, SchedulareState state)
        {
            if (!DEBUG) return;

            double percentageOfSatisfaction = CommonLogic.GetPercentageOfSatisfaction(state.Node.Value, shiftsContainer);
            Console.WriteLine($"Weight = {state.Weight}");
            Console.WriteLine($"Percentage of workes constrains satisfaction = {percentageOfSatisfaction}");
            CommonLogic.PrintSchedulare(state.Node.Value, shiftsContainer);
        }


        protected int UnwantedShift(Schedulare schedulare, ShiftsContainer shiftsContainer, ref int weight)
        {
            try
            {
                foreach (var employee in shiftsContainer.EmployeeConstraints)
                {
                    var currEmpConstraints = employee.WeeklyConstraints.Where(x => !x.Value.ContainsContent("Free day")).ToList();

                    foreach (var day in schedulare.Days)
                    {
                        foreach (var shift in day.Shifts)
                        {
                            if (!shift.Workers.Any(x => x.Name.CompareContent(employee.Name))) continue;

                            var constraintDay = currEmpConstraints.FirstOrDefault(x => x.Key.CompareContent(day.Name));

                            if (constraintDay.Key == null) continue;

                            // if the employee asked for this shift skip
                            if (constraintDay.Value.CompareContent(shift.Name)) continue;

                            weight += _unwantedShift;

                        }
                    }
                }

                return weight;
            }
            catch (Exception e)
            {
                CommonLogic.ApeandToFile(e.ToString());
            }
            return default;
        }

        protected int ShiftsInARow(Schedulare schedulare, ShiftsContainer shiftsContainer, ref int weight, int shiftsInARow)
        {
            try
            {
                foreach (var employee in shiftsContainer.EmployeeConstraints)
                {
                    var shiftsDaysList = schedulare.Days.ToList();

                    var count = 0;

                    foreach (var day in shiftsDaysList)
                    {
                        if (day.Shifts.SelectMany(x => x.Workers).Any(x => x.Name.CompareContent(employee.Name)))
                            count++;
                        else count = 0;
                    }

                    if (count >= shiftsInARow)
                        weight += _shiftsInARow;
                }

                return weight;
            }
            catch (Exception e)
            {
                CommonLogic.ApeandToFile(e.ToString());
            }
            return default;
        }

        protected int LackSatisfactionConstraintsWeight(Schedulare schedulare, ShiftsContainer shiftsContainer, ref int weight)
        {
            try
            {
                foreach (var employee in shiftsContainer.EmployeeConstraints)
                {
                    var currEmpConstraints = employee.WeeklyConstraints.Where(x => !x.Value.ContainsContent("Free day"));

                    foreach (var shiftConstrain in currEmpConstraints)
                    {
                        var desireShiftFromSchedulare = schedulare.Days.FirstOrDefault(x => x.Name.CompareContent(shiftConstrain.Key)).Shifts.
                                                                        FirstOrDefault(y => y.Name.CompareContent(shiftConstrain.Value));

                        var ifEmployeeGotTheDesiredShift = desireShiftFromSchedulare.Workers.Any(x => x.Name.CompareContent(employee.Name));

                        if (ifEmployeeGotTheDesiredShift) continue;

                        weight += _lackSatisfactionConstraintsWeight;
                    }
                }

                return weight;
            }
            catch (Exception e)
            {
                CommonLogic.ApeandToFile(e.ToString());
            }
            return default;
        }

        protected int TwoShiftsInARowWeight(Schedulare schedulare, ShiftsContainer shiftsContainer, ref int weight)
        {
            try
            {
                foreach (var employee in shiftsContainer.EmployeeConstraints)
                {
                    var shiftsList = schedulare.Days.SelectMany(x => x.Shifts).ToList();

                    for (int i = 0; i < shiftsList.Count - 1; i++)
                    {
                        if (shiftsList[i].Workers.Any(x => x.Name.CompareContent(employee.Name)) && shiftsList[i + 1].Workers.Any(x => x.Name.CompareContent(employee.Name)))
                        {
                            weight += _twoShiftsInARowWeight;
                        }
                    }
                }

                return weight;
            }
            catch (Exception e)
            {
                CommonLogic.ApeandToFile(e.ToString());
            }
            return default;
        }

        protected Shift GetIncompleteShift(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            try
            {
                var incompleteShiftList = GetIncompleteShiftList(schedulare, shiftsContainer);

                return incompleteShiftList.FirstOrDefault();
            }
            catch (Exception e)
            {
                CommonLogic.ApeandToFile(e.ToString());
            }
            return default;
        }

        protected bool IsSchedulareFull(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            try
            {
                var incompleteShiftList = GetIncompleteShiftList(schedulare, shiftsContainer);

                var incompleteShiftCount = incompleteShiftList.Count();

                return incompleteShiftCount.Equals(0);
            }
            catch (Exception e)
            {
                CommonLogic.ApeandToFile(e.ToString());
            }
            return default;
        }

        //protected static List<Shift> GetIncompleteShiftList(Schedulare schedulare, ShiftsContainer shiftsContainer)
        //{
        //    var shiftsList = schedulare.Days.SelectMany(x => x.Shifts).ToList();

        //    var numberOfWorkersInShift = shiftsContainer.ShiftParams.NumberOfWokersInShift;

        //    var unmannedShifts = shiftsList.Where(x => !x.Workers.Count.Equals(numberOfWorkersInShift)).ToList();

        //    return unmannedShifts;
        //}

        protected static List<Shift> GetIncompleteShiftList(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            try
            {
                var shiftsList = schedulare.Days.SelectMany(x => x.Shifts).ToList();

                var numberOfWorkersInShift = shiftsContainer.ShiftParams.NumberOfWokersInShift;

                var unmannedShifts = shiftsList.Where(x => !x.Workers.Where(y => y.Name.IsNullOrEmpty()).ToList().Count.Equals(0) ||
                                                        x.Workers.Count < numberOfWorkersInShift).ToList();

                return unmannedShifts;
            }
            catch (Exception e)
            {
                CommonLogic.ApeandToFile(e.ToString());
            }
            return default;
        }


        private int TwiceOnTheSameShift(Schedulare schedulare, ShiftsContainer shiftsContainer, ref int weight)
        {
            try
            {
                var shiftsList = schedulare.Days.SelectMany(x => x.Shifts).ToList();

                var groupedWorkers = shiftsList.SelectMany(x => x.Workers.GroupBy(y => y.Name)).ToList();

                foreach (var workerGroup in groupedWorkers)
                    if (workerGroup.Count() > 1) weight += _twiceOnTheSameShift;

                return weight;
            }
            catch (Exception e)
            {
                CommonLogic.ApeandToFile(e.ToString());
            }
            return default;
        }

        protected Shift GetRandomIncompleteShift(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            try
            {
                var incompleteShiftList = GetIncompleteShiftList(schedulare, shiftsContainer);

                var random = new Random();

                var randNumber = random.Next(0, incompleteShiftList.Count - 1);

                return incompleteShiftList[randNumber];
            }
            catch (Exception e)
            {
                CommonLogic.ApeandToFile(e.ToString());
            }
            return default;
        }

        protected DayShift GetRandomShift(List<DayShift> shiftsList)
        {
            var random = new Random();

            var randNumber = random.Next(0, shiftsList.Count - 1);

            return shiftsList[randNumber];
        }
        protected static void PrintTree(TreeNode<Schedulare> root, SchedulareState currState)
        {
            Console.WriteLine("------------------------------------------------");
            //root.PrintPretty("", true);
            Console.WriteLine(currState.Weight);
            Console.WriteLine("------------------------------------------------");
        }
    }
}
