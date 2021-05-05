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

        private const double ALFA = 0.55;
        private double _threshold;
        public override Schedulare Execute(Schedulare schedulare, ShiftsContainer shiftsContainer, WeightContainer weightContainer = null)
        {
            UpdateWeights(weightContainer);

            var openSet = new SortedArray<SchedulareState>(new SchedulareComparerArray());

            var root = new TreeNode<Schedulare>(schedulare);

            var schedulareState = GetSchedulareState(schedulare.DeepClone(), shiftsContainer, root);

            openSet.Add(schedulareState);

            UpdateThreshold(schedulareState);

            var closeSet = new SortedArray<SchedulareState>(new SchedulareComparerArray());

            while (!openSet.IsNullOrEmpty())
            {
                var currState = GetCurrentState(openSet);

                openSet.Remove(currState);

                closeSet.Add(currState);

                var currNode = currState.Node;

                var sat = CommonLogic.GetPercentageOfSatisfaction(currNode.Value, shiftsContainer);

                PrintDebugData(shiftsContainer, currState);

                if (IsGoal() && IsSchedulareFull(currNode.Value, shiftsContainer))
                {
                    UpdateCurrentBestSolution(currState);
                    break;
                }

                // if the current node is full schedulare but it is not goal yet 
                // remove the node from open list and look for another solutions
                if (IsSchedulareFull(currNode.Value, shiftsContainer))
                {
                    UpdateCurrentBestSolution(currState);
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


            PrintDebugData(shiftsContainer, CurrentBestSolution);

            return CurrentBestSolution.Node.Value;
        }
        protected override void PrintDebugData(ShiftsContainer shiftsContainer, SchedulareState state)
        {
            Console.WriteLine($"_threshold - {_threshold}");
            base.PrintDebugData(shiftsContainer, state);
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


    }
}
