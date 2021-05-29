using C5;
using Engine.Algorithms.Bases;
using Engine.Extensions;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Algorithms
{
    public class BFS : HeuristicAlgoBase
    {
        public IntervalHeap<SchedulareState> CloseSet { get; private set; }
        public IntervalHeap<SchedulareState> OpenSet { get; private set; }

        public override SchedulareState Execute(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            InitParams(schedulare, shiftsContainer);

            var schedulareState = GetSchedulareState(schedulare.DeepClone(), shiftsContainer, TreeRoot);

            OpenSet.Add(schedulareState);

            ExecuteStopwatch.Start();

            while (!OpenSet.IsEmpty)
            {
                var currState = OpenSet.FindMin();

                OpenSet.DeleteMin();

                CloseSet.Add(currState);

                var currNode = currState.Node;

                UpdateCurrentBestSolution(currState);

                if (IsGoal() && IsSchedulareFull(currNode.Value, shiftsContainer))
                {
                    UpdateCurrentBestSolution(currState);
                    break;
                }

                // DEBUG
                PrintDebugData(shiftsContainer, currState);

                // if the current node is full schedulare but it is not goal yet 
                // remove the node from open list and look for another solutions
                if (IsSchedulareFull(currNode.Value, shiftsContainer))
                {
                    OpenSet.DeleteMin();
                    CloseSet.Add(currState);
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
                    OpenSet.Add(newNodeState);
                }

                #endregion
            }

            // DEBUG
            PrintDebugData(shiftsContainer, CurrentBestSolution);

            var ret = CurrentBestSolution;

            CurrentBestSolution = null;
            IsFinished = false;
            ExecuteStopwatch.Reset();

            return ret;
        }

        private void InitParams(Schedulare schedulare, ShiftsContainer shiftsContainer)
        {
            ShiftsContainer = shiftsContainer;

            CloseSet = new IntervalHeap<SchedulareState>(new SchedulareComparer());

            OpenSet = new IntervalHeap<SchedulareState>(new SchedulareComparer());

            TreeRoot = new TreeNode<Schedulare>(schedulare);
        }
    }
}
