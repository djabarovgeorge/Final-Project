using C5;
using Engine.Extensions;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Engine.Algorithms
{
    class Tabu
    {
        private const int GOAL_WEIGHT = 100;
        private const int EXPLORATION_TIME_SECONDS = 2;
        private int _twoShiftsInARowWeight = 10;
        private int _lackSatisfactionConstraintsWeight = 20;
        private int _shiftsInARow = 20;
        private int _unwantedShift = 20;
        public SchedulareState CurrentBestSolution { get; set; }

        public Schedulare Execute(Schedulare schedulare, ShiftsContainer shiftsContainer, WeightContainer weightContainer = null)
        {
            if (weightContainer != null)
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

        private void PrintDebugData(ShiftsContainer shiftsContainer, SchedulareState state)
        {
            double percentageOfSatisfaction = CommonLogic.GetPercentageOfSatisfaction(state.Node.Value, shiftsContainer);
            Console.WriteLine($"Weight = {state.Weight}");
            Console.WriteLine($"Percentage of workes constrains satisfaction = {percentageOfSatisfaction}");
            CommonLogic.PrintSchedulare(state.Node.Value, shiftsContainer);
        }

        private bool HillClimbing(ShiftsContainer shiftsContainer, IntervalHeap<SchedulareState> openSet, CircularList closeSet)
        {

            var currState = openSet.FindMin();
            UpdateCurrentBestSolution(currState);

            // IsGoal
            if (IsGoal(currState)) return true;


            #region Update queue sets
            openSet.DeleteMin();
            closeSet.Add(currState.Node.Value);
            #endregion

            var currNode = currState.Node;

            // DEBUG
            PrintDebugData(shiftsContainer, currState);

            // create and add child nodes
            #region build new nodes
            foreach (var emp in shiftsContainer.EmployeeConstraints.Select(x => x.Name))
            {
                var newNodeSchedulare = currNode.Value.DeepClone();

                var currShiftToAssin = GetIncompleteShift(newNodeSchedulare, shiftsContainer);

                // modify schdulare
                currShiftToAssin.Workers.Remove(currShiftToAssin.Workers.FirstOrDefault(x => x.Name.IsNullOrEmpty()));
                currShiftToAssin.Workers.Add(new Worker() { Name = emp });

                // validate if the new state in tabu list - is yes ignore it
                if (closeSet.Contains(newNodeSchedulare)) continue;

                // add new node to the tree - currNode
                var childNode = currNode.AddChild(newNodeSchedulare);

                // get new state
                var newNodeState = GetSchedulareState(newNodeSchedulare, shiftsContainer, childNode);

                // add new state to openSet
                openSet.Add(newNodeState);

                // DEBUG 
                PrintDebugData(shiftsContainer, newNodeState);
            }
            #endregion

            return false;
        }

        private static bool IsGoal(SchedulareState currState)
        {
            return currState.Weight < GOAL_WEIGHT;
        }

        private void UpdateCurrentBestSolution(SchedulareState currState)
        {
            if (CurrentBestSolution == null)
                CurrentBestSolution = currState;

            else if (currState.Weight < CurrentBestSolution.Weight)
                CurrentBestSolution = currState;
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
            var incompleteShiftList = GetInvalidShiftsList(schedulare, shiftsContainer);

            return incompleteShiftList.FirstOrDefault();
        }

        private bool IsSchedulareFull(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            var incompleteShiftList = GetIncompleteShiftList(schedulare, shiftsContainer);

            var incompleteShiftCount = incompleteShiftList.Count();

            return incompleteShiftCount.Equals(0);
        }

        private static List<Shift> GetIncompleteShiftList(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            var shiftsList = schedulare.Days.SelectMany(x => x.Shifts).ToList();

            var numberOfWorkersInShift = shiftsContainer.ShiftParams.NumberOfWokersInShift;

            var unmannedShifts = shiftsList.Where(x => !x.Workers.Count.Equals(numberOfWorkersInShift)).ToList();

            return unmannedShifts;
        }


        private static List<Shift> GetInvalidShiftsList(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            var shiftsList = schedulare.Days.SelectMany(x => x.Shifts).ToList();

            var numberOfWorkersInShift = shiftsContainer.ShiftParams.NumberOfWokersInShift;

            var unmannedShifts = shiftsList.Where(x => !x.Workers.Where(y=> y.Name.IsNullOrEmpty()).ToList().Count.Equals(0)).ToList();

            return unmannedShifts;
        }

        private SchedulareState GetSchedulareState(Schedulare schedulare, ShiftsContainer shiftsContainer, TreeNode<Schedulare> treeNode)
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



    //class CircularQueue<T>
    //{
    //    private T[] ele;
    //    private int front;
    //    private int rear;
    //    private int max;
    //    private int count;

    //    public CircularQueue(int size)
    //    {
    //        ele = new T[size];
    //        front = 0;
    //        rear = -1;
    //        max = size;
    //        count = 0;
    //    }

    //    public void insert(T item)
    //    {
    //        var listItems = (List<SchedulareState>)ele;
    //        listItems.
    //        if (count == max)
    //        {
    //            delete();
    //            //Console.WriteLine("Queue Overflow");
    //            //return;
    //        }
    //        //else
    //        //{
    //        rear = (rear + 1) % max;
    //        ele[rear] = item;

    //        count++;
    //        //}
    //    }

    //    public void delete()
    //    {
    //        if (count == 0)
    //        {
    //            Console.WriteLine("Queue is Empty");
    //        }
    //        else
    //        {
    //            Console.WriteLine("deleted element is: " + ele[front]);

    //            front = (front + 1) % max;

    //            count--;
    //        }
    //    }

    //    public void printQueue()
    //    {
    //        int i = 0;
    //        int j = 0;

    //        if (count == 0)
    //        {
    //            Console.WriteLine("Queue is Empty");
    //            return;
    //        }
    //        else
    //        {
    //            for (i = front; j < count;)
    //            {
    //                Console.WriteLine("Item[" + (i + 1) + "]: " + ele[i]);

    //                i = (i + 1) % max;
    //                j++;

    //            }
    //        }
    //    }
    //}


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
}