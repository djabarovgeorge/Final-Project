using C5;
using Engine.Algorithms.Bases;
using Engine.Extensions;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Algorithms
{
    public class WAStar : HeuristicAlgoBase
    {

        private const double ALFA = 1.0001;
        private double _threshold;
        private bool _isWithTH = false;

        public SortedArray<SchedulareState> OpenSet { get; private set; }
        public SortedArray<SchedulareState> CloseSet { get; private set; }

        public override SchedulareState Execute(Schedulare schedulare, ShiftsContainer shiftsContainer, WeightContainer weightContainer = null)
        {
            InitParams(schedulare, shiftsContainer, weightContainer);

            var schedulareState = GetSchedulareState(schedulare.DeepClone(), shiftsContainer, TreeRoot);

            OpenSet.Add(schedulareState);

            ExecuteStopwatch.Start();

            UpdateThreshold(schedulareState);

            while (!OpenSet.IsNullOrEmpty())
            {
                var currState = GetCurrentState(OpenSet);

                UpdateCurrentBestSolution(currState);

                OpenSet.Remove(currState);

                CloseSet.Add(currState);

                var currNode = currState.Node;

                PrintDebugData(shiftsContainer, currState);

                if (IsGoal())
                {
                    UpdateCurrentBestSolution(currState);
                    break;
                }

                // if the current node is full schedulare but it is not goal yet 
                // remove the node from open list and look for another solutions
                if (IsSchedulareFull(currNode.Value, shiftsContainer))
                {
                    UpdateCurrentBestSolution(currState);
                    OpenSet.Remove(currState);
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

            PrintDebugData(shiftsContainer, CurrentBestSolution);

            var ret = CurrentBestSolution;

            CurrentBestSolution = null;
            IsFinished = false;
            ExecuteStopwatch.Reset();

            return ret;
        }

        private void InitParams(Schedulare schedulare, ShiftsContainer shiftsContainer, WeightContainer weightContainer)
        {
            ShiftsContainer = shiftsContainer;

            UpdateWeights(weightContainer);

            OpenSet = new SortedArray<SchedulareState>(new SchedulareComparerArray());

            TreeRoot = new TreeNode<Schedulare>(schedulare);

            CloseSet = new SortedArray<SchedulareState>(new SchedulareComparerArray());
        }

        protected override void PrintDebugData(ShiftsContainer shiftsContainer, SchedulareState state)
        {
            if (!DEBUG) return;

            Console.WriteLine($"_threshold - {_threshold}");
            base.PrintDebugData(shiftsContainer, state);
        }
        private SchedulareState GetCurrentState(SortedArray<SchedulareState> openSet)
        {
            SchedulareState state = null;

            if (!_isWithTH)
            {
                _isWithTH = true;
                return openSet.FindMin();
            }

            state = openSet.FirstOrDefault(x => x.Weight > _threshold);

            if (state == null)
            {
                state = openSet.LastOrDefault();
            }

            UpdateThreshold(state);

            _isWithTH = false;
            return state;
        }

        private void UpdateThreshold(SchedulareState state = null)
        {
            //_threshold = state != null? state.Weight -1 : _threshold - 1;
            //_threshold = state.Weight * ALFA;
            _threshold = OpenSet.FirstOrDefault().Weight * ALFA;

        }


    }
}
