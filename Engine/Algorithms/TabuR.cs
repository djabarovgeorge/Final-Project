using C5;
using Engine.Algorithms.Bases;
using Engine.Extensions;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Engine.Algorithms
{
    class TabuR : HeuristicAlgoBase
    {
        protected readonly int EXPLORATION_TIME_SECONDS = 2;

        public CircularList CloseSet { get; set; }
        public IntervalHeap<SchedulareState> OpenSet { get; private set; }
        private Stopwatch ExplorationStopwatch { get; set; }


        public override SchedulareState Execute(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            InitParams(schedulare, shiftsContainer);

            var schedulareState = GetSchedulareState(schedulare.DeepClone(), shiftsContainer, TreeRoot);

            UpdateCurrentBestSolution(schedulareState);

            OpenSet.Add(schedulareState);

            ExecuteStopwatch.Start();

            var shiftsList = GetShiftList(schedulareState.Node.Value);

            while (!IsGoal())
            {
                var randShift = GetRandomShift(shiftsList);

                // DEBUG 
                PrintDebugData(shiftsContainer, CurrentBestSolution);

                ExplorationStopwatch.Reset();

                ExplorationStopwatch.Start();

                #region Exploration loop

                while (ExplorationStopwatch.Elapsed.TotalSeconds < EXPLORATION_TIME_SECONDS)
                {
                    // IsGoal
                    // OR break id exploration ended with no result
                    if (IsGoal() || OpenSet.Count.Equals(0)) break;

                    var currState = OpenSet.FindMin();

                    UpdateCurrentBestSolution(currState);

                    OpenSet.DeleteMin();

                    CloseSet.Add(currState.Node.Value);


                    for (int workerIndex = 0; workerIndex < randShift.Shift.Workers.Count; workerIndex++)
                    {
                        var currStateInOrderToReplaceEmp = currState.DeepClone();

                        var currStateNodeInOrderToReplaceEmp = currStateInOrderToReplaceEmp.Node;

                        RemoveEmpFromCurrShift(randShift, workerIndex, currStateInOrderToReplaceEmp);

                        // DEBUG 
                        PrintDebugData(shiftsContainer, currStateInOrderToReplaceEmp);

                        #region build new nodes
                        foreach (var emp in shiftsContainer.EmployeeConstraints.Select(x => x.Name))
                        {
                            var newNodeSchedulare = currStateNodeInOrderToReplaceEmp.Value.DeepClone();

                            var currShiftToAssin = GetIncompleteShift(newNodeSchedulare, shiftsContainer);

                            // modify schdulare
                            currShiftToAssin.Workers.Remove(currShiftToAssin.Workers.FirstOrDefault(x => x.Name.IsNullOrEmpty()));
                            currShiftToAssin.Workers.Add(new Worker() { Name = emp });

                            // validate if the new state in tabu list - is yes ignore it
                            if (CloseSet.Contains(newNodeSchedulare))
                            {
                                if (!DEBUG) continue;

                                Console.WriteLine($"####### Tabu list filterd #######");

                                continue;
                            }

                            // add new node to the tree - currNode
                            var childNode = currStateNodeInOrderToReplaceEmp.AddChild(newNodeSchedulare);

                            // get new state
                            var newNodeState = GetSchedulareState(newNodeSchedulare, shiftsContainer, childNode);

                            // add new state to openSet
                            OpenSet.Add(newNodeState);

                            // DEBUG 
                            PrintDebugData(shiftsContainer, newNodeState);
                        }
                        #endregion
                    }
                }
                ExplorationStopwatch.Stop();

                #endregion

                if (IsGoal()) break;

            }

            var ret = CurrentBestSolution;

            CurrentBestSolution = null;
            IsFinished = false;
            ExecuteStopwatch.Reset();

            return ret;
        }

        private static void RemoveEmpFromCurrShift(DayShift randShift, int workerIndex, SchedulareState currStateInOrderToReplaceEmp)
        {
            // remove emp from current solution to explore better solution
            var currShiftDay = currStateInOrderToReplaceEmp.Node.Value.Days.FirstOrDefault(x => x.Name.CompareContent(randShift.DayName));
            var currShift = currShiftDay.Shifts.FirstOrDefault(x => x.Name.CompareContent(randShift.Shift.Name));
            currShift.Workers[workerIndex] = new Worker() { Name = string.Empty };
        }

        private void InitParams(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            ShiftsContainer = shiftsContainer;

            TreeRoot = new TreeNode<Schedulare>(schedulare);

            CloseSet = new CircularList(20);

            OpenSet = new IntervalHeap<SchedulareState>(new SchedulareComparer());

            ExplorationStopwatch = new Stopwatch();
        }

        private List<DayShift> GetShiftList(Schedulare value)
        {
            var shiftsList = value.Days.SelectMany(x => x.Shifts).ToList();

            var list = new List<DayShift>();

            foreach (var day in value.Days)
            {
                foreach (var shift in day.Shifts)
                {
                    list.Add(new DayShift() { DayName = day.Name, Shift = shift });
                }
            }

            return list;
        }

    }
}
