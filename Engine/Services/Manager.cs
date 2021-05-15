using Engine.Abstracts;
using Engine.Algorithms;
using Engine.Extensions;
using Engine.Interfaces;
using Engine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Services
{
    class Manager
    {
        private ShiftSetter _constructsShift = new ConstructsShift();
        private Rand _randAlgorithm = new Rand();
        private GreedyAlgorithm _greedyAlgorithm = new GreedyAlgorithm();
        private readonly IAlgo _bfs = new BFS();
        private readonly IAlgo _waStar = new WAStar();
        private readonly IAlgo _tabu = new Tabu();
        private readonly IAlgo _tabuR = new TabuR();
        private Dictionary<string, SchedulareListStatistics> _schedulareStatisticsList = new Dictionary<string, SchedulareListStatistics>();
        private const string FILENAME = "algodata";

        public Manager()
        {
            CommonLogic.InitFileSuffix();
        }


        public void Execute()
        {
            RunCycle();

        }

        private void RunCycle()
        {

            for (int executionAmount = 1; executionAmount < 1000; executionAmount++)
            {
                CommonLogic.ApeandToFile($"EXECUTION_AMOUNT - {executionAmount}");
                CommonLogic.ApeandToFile($"ALGORITHM_RUN_TIME_SECONDS - {120}");
                Console.WriteLine($"EXECUTION_AMOUNT - {executionAmount}");
                Console.WriteLine($"ALGORITHM_RUN_TIME_SECONDS - {120}");

                RunHuristicMethods();

                UpdateSatisfactionAvarage();

                PrintStats();
            }
            CommonLogic.ApeandToFile(string.Empty);
        }

        private void RunHuristicMethods()
        {
            var shiftsContainer = _constructsShift.Excute();

            var schedulare = new Schedulare(shiftsContainer);

            var bfsResult = _bfs.Execute(schedulare.DeepClone(), shiftsContainer);

            PrintDebugData(shiftsContainer, bfsResult, "BFS");

            UpdateSchefulareStatistics(shiftsContainer, bfsResult, "BFS");

            var waStarResult = _waStar.Execute(schedulare.DeepClone(), shiftsContainer);

            PrintDebugData(shiftsContainer, waStarResult, "W A Star");

            UpdateSchefulareStatistics(shiftsContainer, waStarResult, "WAStar");

            var randSchedular = _randAlgorithm.Execute(schedulare.DeepClone(), shiftsContainer);

            var tabuResult = _tabu.Execute(randSchedular.DeepClone(), shiftsContainer);

            PrintDebugData(shiftsContainer, tabuResult, "TABU");

            UpdateSchefulareStatistics(shiftsContainer, tabuResult, "TABU");

            var tabuRResult = _tabuR.Execute(randSchedular.DeepClone(), shiftsContainer);

            PrintDebugData(shiftsContainer, tabuRResult, "TABU Random");

            UpdateSchefulareStatistics(shiftsContainer, tabuRResult, "TabuRandom");

        }

        private void PrintStats()
        {
            foreach (var algoList in _schedulareStatisticsList)
            {
                var algName = algoList.Key;

                CommonLogic.ApeandToFile(algName);

                CommonLogic.ApeandToFile($"Satisfaction Avarage = {algoList.Value.SatisfactionAvarage}");

                CommonLogic.ApeandToFile($"Weight Avarage = {algoList.Value.WeightAvarage}");

                CommonLogic.ApeandToFile($"Execute Time Avarage = {algoList.Value.ExecuteTimeAvarage}");
            }

            CommonLogic.ApeandToFile(string.Empty);

        }

        private void UpdateSatisfactionAvarage()
        {
            foreach (var algoList in _schedulareStatisticsList)
            {
                algoList.Value.SatisfactionAvarage = algoList.Value.SchedulareSatisfactionList.Select(x => x.Satisfaction).Sum() / algoList.Value.SchedulareSatisfactionList.Count;
                algoList.Value.WeightAvarage = algoList.Value.SchedulareSatisfactionList.Select(x => x.Weight).Sum() / algoList.Value.SchedulareSatisfactionList.Count;
                algoList.Value.ExecuteTimeAvarage = algoList.Value.SchedulareSatisfactionList.Select(x => x.ExecuteTime).Sum() / algoList.Value.SchedulareSatisfactionList.Count;
            }
        }

        private void UpdateSchefulareStatistics(ShiftsContainer shiftsContainer, SchedulareState algoResult, string algoString)
        {
            if (!_schedulareStatisticsList.ContainsKey(algoString))
            {
                _schedulareStatisticsList.AddOrUpdate(algoString, new SchedulareListStatistics());
            }
            var bfsSatisfaction = CommonLogic.GetPercentageOfSatisfaction(algoResult.Node.Value, shiftsContainer);
            _schedulareStatisticsList[algoString].SchedulareSatisfactionList.Add(new SchedulareSatisfaction(algoResult.Node.Value, bfsSatisfaction, shiftsContainer, algoResult.Weight, algoResult.ExecuteTime));
        }

        private void PrintDebugData(ShiftsContainer shiftsContainer, SchedulareState state,string algoName)
        {
            Console.WriteLine(algoName);
            double percentageOfSatisfaction = CommonLogic.GetPercentageOfSatisfaction(state.Node.Value, shiftsContainer);
            Console.WriteLine($"Weight = {state.Weight}");
            Console.WriteLine($"Satisfaction = {percentageOfSatisfaction}");
            CommonLogic.PrintSchedulare(state.Node.Value, shiftsContainer);

            CommonLogic.ApeandToFile(algoName, FILENAME);
            CommonLogic.ApeandToFile($"Weight = {state.Weight}", FILENAME);
            CommonLogic.ApeandToFile($"Satisfaction = {percentageOfSatisfaction}", FILENAME);
            string schedulareStateJson = JsonConvert.SerializeObject(state.Node.Value);
            CommonLogic.ApeandToFile(schedulareStateJson, FILENAME);
            string shiftsContainerJson = JsonConvert.SerializeObject(shiftsContainer);
            CommonLogic.ApeandToFile(shiftsContainerJson, FILENAME);
        }
    }
}

