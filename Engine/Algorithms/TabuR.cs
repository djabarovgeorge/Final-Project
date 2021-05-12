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

        public override SchedulareState Execute(Schedulare schedulare, ShiftsContainer shiftsContainer, WeightContainer weightContainer = null)
        {

            UpdateWeights(weightContainer);

            var root = new TreeNode<Schedulare>(schedulare);

            var schedulareState = GetSchedulareState(schedulare.DeepClone(), shiftsContainer, root);

            UpdateCurrentBestSolution(schedulareState);

            var closeSet = new CircularList(20);

            var timer = new Stopwatch();

            var shiftsList = GetShiftList(schedulareState.Node.Value);

            while (!IsGoal())
            {
                var randShift = GetRandomShift(shiftsList);

                var currExplorationState = CurrentBestSolution.DeepClone();

                var openSet = new IntervalHeap<SchedulareState>(new SchedulareComparer()) { currExplorationState };

                // DEBUG 
                PrintDebugData(shiftsContainer, currExplorationState);

                timer.Reset();
                timer.Start();

                #region Exploration loop

                while (timer.Elapsed.TotalSeconds < EXPLORATION_TIME_SECONDS)
                {

                    // IsGoal
                    if (IsGoal()) break;

                    // break id exploration ended with no result
                    if (openSet.Count.Equals(0))
                        break;

                    var currState = openSet.FindMin();

                    UpdateCurrentBestSolution(currState);

                    #region Update queue sets
                    openSet.DeleteMin();
                    closeSet.Add(currState.Node.Value);
                    #endregion


                    for (int workerIndex = 0; workerIndex < randShift.Shift.Workers.Count; workerIndex++)
                    {

                        var currStateInOrderToReplaceEmp = currState.DeepClone();

                        var currStateNodeInOrderToReplaceEmp = currStateInOrderToReplaceEmp.Node;

                        // remove emp from current solution to explore better solution
                        var currShiftDay = currStateInOrderToReplaceEmp.Node.Value.Days.FirstOrDefault(x => x.Name.CompareContent(randShift.DayName));
                        var currShift = currShiftDay.Shifts.FirstOrDefault(x => x.Name.CompareContent(randShift.Shift.Name));
                        currShift.Workers[workerIndex] = new Worker() { Name = string.Empty };

                        // DEBUG 
                        //PrintDebugData(shiftsContainer, currStateInOrderToReplaceEmp);

                        #region HillClimbing
                        // todo pass shift to assingh

                        #region build new nodes
                        foreach (var emp in shiftsContainer.EmployeeConstraints.Select(x => x.Name))
                        {
                            var newNodeSchedulare = currStateNodeInOrderToReplaceEmp.Value.DeepClone();

                            var currShiftToAssin = GetIncompleteShift(newNodeSchedulare, shiftsContainer);

                            // modify schdulare
                            currShiftToAssin.Workers.Remove(currShiftToAssin.Workers.FirstOrDefault(x => x.Name.IsNullOrEmpty()));
                            currShiftToAssin.Workers.Add(new Worker() { Name = emp });

                            // validate if the new state in tabu list - is yes ignore it
                            if (closeSet.Contains(newNodeSchedulare))
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
                            openSet.Add(newNodeState);

                            // DEBUG 
                            PrintDebugData(shiftsContainer, newNodeState);
                        }
                        #endregion
                        #endregion
                    }
                }
                timer.Stop();

                #endregion

                if (IsGoal()) break;

            }

            var ret = CurrentBestSolution;

            CurrentBestSolution = null;
            IsFinished = false;

            return ret;
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
