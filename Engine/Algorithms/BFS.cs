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
        public override Schedulare Execute(Schedulare schedulare, ShiftsContainer shiftsContainer, WeightContainer weightContainer = null)
        {

            UpdateWeights(weightContainer);

            var openSet = new IntervalHeap<SchedulareState>(new SchedulareComparer());

            var root = new TreeNode<Schedulare>(schedulare);

            var schedulareState = GetSchedulareState(schedulare.DeepClone(), shiftsContainer, root);

            openSet.Add(schedulareState);

            var closeSet = new IntervalHeap<SchedulareState>(new SchedulareComparer());

            TreeNode<Schedulare> result = null;

            while (!openSet.IsEmpty)
            {

                var currState = openSet.FindMin();

                #region Update queue sets
                openSet.DeleteMin();
                closeSet.Add(currState);
                #endregion

                var currNode = currState.Node;

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
                    openSet.DeleteMin();
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

            // DEBUG
            PrintDebugData(shiftsContainer, CurrentBestSolution);

            return result.Value;
        }
    }
}
