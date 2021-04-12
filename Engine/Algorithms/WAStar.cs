﻿using C5;
using Engine.Extensions;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Algorithms
{
    class WAStar
    {

        private int _twoShiftsInARowWeight = 10;
        private int _lackSatisfactionConstraintsWeight = 10;
        private int _shiftsInARow = 20;
        private int _unwantedShift = 20;
        private const double ALFA = 0.02;

        private double _threshold;
        public Schedulare Execute(Schedulare schedulare, ShiftsContainer shiftsContainer, WeightContainer weightContainer = null)
        {
            if (weightContainer != null)
                UpdateWeights(weightContainer);

            var openSet = new SortedArray<SchedulareState>(new SchedulareComparerArray());

            var root = new TreeNode<Schedulare>(schedulare);

            var schedulareState = GetSchedulareState(schedulare.DeepClone(), shiftsContainer, root);

            openSet.Add(schedulareState);

            UpdateThreshold(schedulareState);

            var closeSet = new SortedArray<SchedulareState>(new SchedulareComparerArray());

            #region Debug full schedular
            // DEBUG - remove first worker from the first shift
            //schedulare.Days.FirstOrDefault().Shifts.FirstOrDefault().Workers.Remove(schedulare.Days.FirstOrDefault().Shifts.FirstOrDefault().Workers.FirstOrDefault());
            //currShiftToAssin.Workers.Add(new Worker() { Name="nick"});
            #endregion	


            TreeNode<Schedulare> result = null;


            while (!openSet.IsNullOrEmpty())
            {
                var currState = GetCurrentState(openSet);

                openSet.Remove(currState);

                closeSet.Add(currState);

                // DEBUG
                PrintTree(root, currState);
                Console.WriteLine($"_threshold - {_threshold}");
                Console.WriteLine($"openSet count - {openSet.Count}");


                var currNode = currState.Node;

                if ((currState.Weight < 50) && IsGoal(currNode.Value , shiftsContainer))
                {
                    result = currNode;
                    break;
                }

                // DEBUG
                CommonLogic.PrintSchedulare(currNode.Value, shiftsContainer);

                // if the current node is not goal remove the node from open list
                if (IsGoal(currNode.Value, shiftsContainer))
                {
                    openSet.Remove(currState);
                    closeSet.Add(currState);
                    continue;
                }

                // create and add child nodes
                #region build new nodes

                foreach (var emp in shiftsContainer.EmployeeConstraints.Select(x => x.Name))
                {
                    var newNodeSchedulare = currNode.Value.DeepClone();

                    var currShiftToAssin = GetIncompleteShift(newNodeSchedulare, shiftsContainer);

                    // modify schdulare
                    currShiftToAssin.Workers.Add(new Worker() { Name = emp });

                    // add new node to the tree - currNode
                    var childNode = currNode.AddChild(newNodeSchedulare);

                    // get new state
                    var newNodeState = GetSchedulareState(newNodeSchedulare, shiftsContainer, childNode);

                    // add new state to openSet
                    openSet.Add(newNodeState);
                }

                #endregion
            }


            var percentageOfSatisfaction = CommonLogic.GetPercentageOfSatisfaction(result.Value, shiftsContainer);

            // DEBUG 
            CommonLogic.PrintSchedulare(result.Value, shiftsContainer);
            Console.WriteLine($"Percentage of workes constrains satisfaction = {percentageOfSatisfaction}");

            return result.Value;
        }

        private SchedulareState GetCurrentState(SortedArray<SchedulareState> openSet)
        {
            SchedulareState state = null;

            // choose state only if it is not the the min (to cover more space)
            //if(openSet.FirstOrDefault().Weight < _threshold)
            //    state = openSet.FirstOrDefault(x=> x.Weight > _threshold);

            state = openSet.FirstOrDefault(x => x.Weight > _threshold);

            if (state == null)
            {
                state = openSet.LastOrDefault();
            }

            UpdateThreshold(state);

            return state;
        }

        private void UpdateThreshold(SchedulareState state = null)
        {
            //_threshold = state != null? state.Weight -1 : _threshold - 1;
            _threshold = state.Weight * ALFA;
        }

        private void UpdateWeights(WeightContainer weightContainer)
        {
            _twoShiftsInARowWeight = weightContainer.TwoShiftsInARowWeight;
            _lackSatisfactionConstraintsWeight = weightContainer.LackSatisfactionConstraintsWeight;
            _shiftsInARow = weightContainer.ShiftsInARow;
            _unwantedShift = weightContainer.UnwantedShift;
        }

        private static void PrintTree(TreeNode<Schedulare> root, SchedulareState currState)
        {
            Console.WriteLine("------------------------------------------------");
            //root.PrintPretty("", true);
            Console.WriteLine(currState.Weight);
            Console.WriteLine("------------------------------------------------");
        }

        private Shift GetIncompleteShift(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            var incompleteShiftList = GetIncompleteShiftList(schedulare, shiftsContainer);

            return incompleteShiftList.FirstOrDefault();
        }

        private bool IsGoal(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            var incompleteShiftList = GetIncompleteShiftList(schedulare, shiftsContainer);

            var incompleteShiftCount = incompleteShiftList.Count();

            return incompleteShiftCount.Equals(0);
        }

        private static System.Collections.Generic.List<Shift> GetIncompleteShiftList(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            var shiftsList = schedulare.Days.SelectMany(x => x.Shifts).ToList();

            var numberOfWorkersInShift = shiftsContainer.ShiftParams.NumberOfWokersInShift;

            var unmannedShifts = shiftsList.Where(x => !x.Workers.Count.Equals(numberOfWorkersInShift)).ToList();
            return unmannedShifts;
        }

        private SchedulareState GetSchedulareState(Schedulare schedulare, ShiftsContainer shiftsContainer, TreeNode<Schedulare> treeNode)
        {

            var weight = 0;
            // If workers have 2 shifts in the row +10
            TwoShiftsInARowWeight(schedulare, shiftsContainer, ref weight);

            // If the worker did not get the shift he asked for +20
            LackSatisfactionConstraintsWeight(schedulare, shiftsContainer, ref weight);

            // If a worker has 5 work days in the row +20
            var shiftsInARow = 3;
            ShiftsInARow(schedulare, shiftsContainer, ref weight, shiftsInARow);

            // If the worker got a shift that he did not ask +20
            UnwantedShift(schedulare, shiftsContainer, ref weight);

            return new SchedulareState() { Node = treeNode, Weight = weight };
        }

        private int UnwantedShift(Schedulare schedulare, ShiftsContainer shiftsContainer, ref int weight)
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

        private int ShiftsInARow(Schedulare schedulare, ShiftsContainer shiftsContainer, ref int weight, int shiftsInARow)
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

        private int LackSatisfactionConstraintsWeight(Schedulare schedulare, ShiftsContainer shiftsContainer, ref int weight)
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

        private int TwoShiftsInARowWeight(Schedulare schedulare, ShiftsContainer shiftsContainer, ref int weight)
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
    }
}
