using C5;
using Engine.Algorithms.Bases;
using Engine.Extensions;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Engine.Algorithms
{
    public class Tabu : HeuristicAlgoBase
    {
        private const int GOAL_WEIGHT = 100;
        protected readonly int EXPLORATION_TIME_SECONDS = 2;


        public override Schedulare Execute(Schedulare schedulare, ShiftsContainer shiftsContainer, WeightContainer weightContainer = null)
        {

            UpdateWeights(weightContainer);

            var root = new TreeNode<Schedulare>(schedulare);

            var schedulareState = GetSchedulareState(schedulare.DeepClone(), shiftsContainer, root);

            UpdateCurrentBestSolution(schedulareState);

            var closeSet = new CircularList(20);

            Stopwatch timer = new Stopwatch();

            //var shiftsList = schedulareState.Node.Value.Days.SelectMany(x => x.Shifts).ToList();

            var isFinished = false;

            while (!IsGoal(CurrentBestSolution))
            {
                foreach (var day in schedulareState.Node.Value.Days)
                {
                    foreach (var shift in day.Shifts)
                    {
                        var currExplorationState = CurrentBestSolution.DeepClone();

                        var openSet = new IntervalHeap<SchedulareState>(new SchedulareComparer()) { currExplorationState };

                        // DEBUG 
                        PrintDebugData(shiftsContainer, currExplorationState);

                        //closeSet.Add(schedulareState.Node.Value);

                        timer.Reset();
                        timer.Start();

                        #region Exploration loop

                        while (timer.Elapsed.TotalSeconds < EXPLORATION_TIME_SECONDS)
                        {

                            // break id exploration ended with no result
                            if (openSet.Count.Equals(0))
                                break;

                            var currState = openSet.FindMin();

                            UpdateCurrentBestSolution(currState);

                            // IsGoal
                            if (IsGoal(CurrentBestSolution))
                            {
                                isFinished = true;
                                break;
                            }

                            #region Update queue sets
                            openSet.DeleteMin();
                            closeSet.Add(currState.Node.Value);
                            #endregion


                            for (int workerIndex = 0; workerIndex < shift.Workers.Count; workerIndex++)
                            {

                                var currStateInOrderToReplaceEmp = currState.DeepClone();

                                var currStateNodeInOrderToReplaceEmp = currStateInOrderToReplaceEmp.Node;

                                // remove emp from current solution to explore better solution
                                var currShiftDay = currStateInOrderToReplaceEmp.Node.Value.Days.FirstOrDefault(x => x.Name.CompareContent(day.Name));
                                var currShift = currShiftDay.Shifts.FirstOrDefault(x => x.Name.CompareContent(shift.Name));
                                currShift.Workers[workerIndex] = new Worker() { Name = string.Empty }; 

                                // DEBUG 
                                PrintDebugData(shiftsContainer, currStateInOrderToReplaceEmp);

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

                        if (isFinished) break;
                    }
                    if (isFinished) break;
                }
            }
        
            return CurrentBestSolution.Node.Value;
        }



        public Schedulare ExecuteRandom(Schedulare schedulare, ShiftsContainer shiftsContainer, WeightContainer weightContainer = null)
        {

            UpdateWeights(weightContainer);

            var root = new TreeNode<Schedulare>(schedulare);

            var schedulareState = GetSchedulareState(schedulare.DeepClone(), shiftsContainer, root);

            UpdateCurrentBestSolution(schedulareState);

            var closeSet = new CircularList(20);

            Stopwatch timer = new Stopwatch();

            var shiftsList = GetShiftList(schedulareState.Node.Value);

            var isFinished = false;

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

                    // break id exploration ended with no result
                    if (openSet.Count.Equals(0))
                        break;

                    var currState = openSet.FindMin();

                    UpdateCurrentBestSolution(currState);

                    // IsGoal
                    if (IsGoal(CurrentBestSolution))
                    {
                        isFinished = true;
                        break;
                    }

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

                if (isFinished) break;

            }

            return CurrentBestSolution.Node.Value;
        }

        private List<DayShift> GetShiftList(Schedulare value)
        {
            var shiftsList = value.Days.SelectMany(x => x.Shifts).ToList();

            var list = new List<DayShift>();

            foreach (var day in value.Days)
            {
                foreach (var shift in day.Shifts)
                {
                    list.Add(new DayShift(){ DayName = day.Name , Shift = shift });
                }
            }

            return list;
        }

        private static bool IsGoal(SchedulareState currState)
        {
            return currState.Weight < GOAL_WEIGHT;
        }

    }


    public class CircularList
    { 
        public List<Schedulare> ItemList { get; set; }
        private int _size;

        public CircularList(int size)
        {
            ItemList = new List<Schedulare>();
            _size = size;
        }

        public void Add(Schedulare item)
        {
            if (ItemList.Count.Equals(_size))
                ItemList.Remove(ItemList.FirstOrDefault());

            ItemList.Add(item);
        }

        public bool Contains(Schedulare schedulare)
        {
            var isContains = true;

            foreach (var itemSchedulare in ItemList)
            {
                isContains = true;

                foreach (var day in itemSchedulare.Days)
                {
                    foreach (var shift in day.Shifts)
                    {
                        bool isDiff = IfDifferenceInShift(schedulare, day, shift);
                        if (isDiff)
                        {
                            isContains = false;
                            break;
                        }
                    }
                    if (!isContains)
                        break;
                }
                if (isContains)
                    return true;
            }
            return false;
        }

        private static bool IfDifferenceInShift(Schedulare schedulare, Day day, Shift shift)
        {
            var inputDay = schedulare.Days.FirstOrDefault(x => x.Name.CompareContent(day.Name));
            var inputShift = inputDay.Shifts.FirstOrDefault(x => x.Name.CompareContent(shift.Name));
            var isDiff = !shift.Workers.Count.Equals(inputShift.Workers.Count) ||
                !shift.Workers.All(x => inputShift.Workers.Any(y => y.Name.CompareContent(x.Name)));
            return isDiff;
        }

    }
        public class DayShift
        {
            public string DayName { get; set; }
            public Shift Shift { get; set; }
        }
}