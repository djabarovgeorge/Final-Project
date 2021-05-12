using Engine.Abstracts;
using Engine.Algorithms;
using Engine.Extensions;
using Engine.Interfaces;
using Engine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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


        private int EXECUTION_AMOUNT = 5;




        public void Execute()
        {

            for (int i = EXECUTION_AMOUNT; i < 1000; i+=1)
            {
                EXECUTION_AMOUNT = i;
                RunCycle();
            }

        }

        private void RunCycle()
        {
            CommonLogic.ApeandToFile($"EXECUTION_AMOUNT - {EXECUTION_AMOUNT}");
            CommonLogic.ApeandToFile($"ALGORITHM_RUN_TIME_SECONDS - {60}");
            Console.WriteLine($"EXECUTION_AMOUNT - {EXECUTION_AMOUNT}");
            Console.WriteLine($"ALGORITHM_RUN_TIME_SECONDS - {60}");

            for (int i = 0; i < EXECUTION_AMOUNT; i++)
            {
                var shiftsContainer = _constructsShift.Excute();

                var schedulare = new Schedulare(shiftsContainer);

                var bfsResult = _bfs.Execute(schedulare.DeepClone(), shiftsContainer);

                //
                Console.WriteLine($"bfsResult");
                PrintDebugData(shiftsContainer, bfsResult);

                UpdateSchefulareStatistics(shiftsContainer, bfsResult, "bfs");

                var waStarResult = _waStar.Execute(schedulare.DeepClone(), shiftsContainer);

                //
                Console.WriteLine($"waStarResult");
                PrintDebugData(shiftsContainer, waStarResult);

                UpdateSchefulareStatistics(shiftsContainer, waStarResult, "waStar");

                var randSchedular = _randAlgorithm.Execute(schedulare.DeepClone(), shiftsContainer);

                var tabuResult = _tabu.Execute(randSchedular.DeepClone(), shiftsContainer);

                //
                Console.WriteLine($"tabuResult");
                PrintDebugData(shiftsContainer, tabuResult);

                UpdateSchefulareStatistics(shiftsContainer, tabuResult, "tabu");

                var tabuRResult = _tabuR.Execute(randSchedular.DeepClone(), shiftsContainer);

                //
                Console.WriteLine($"tabuRResult");
                PrintDebugData(shiftsContainer, tabuRResult);

                UpdateSchefulareStatistics(shiftsContainer, tabuRResult, "tabuR");
            }

            foreach (var algoList in _schedulareStatisticsList)
            {
                algoList.Value.SatisfactionAvarage = algoList.Value.SchedulareSatisfactionList.Select(x => x.Satisfaction).Sum() / algoList.Value.SchedulareSatisfactionList.Count;
            }

            foreach (var algoList in _schedulareStatisticsList)
            {
                var algName = algoList.Key;
                var algAvarage = algoList.Value.SatisfactionAvarage;

                CommonLogic.ApeandToFile(algName);
                CommonLogic.ApeandToFile(algAvarage.ToString());

            }

            CommonLogic.ApeandToFile(string.Empty);
        }


        private void UpdateSchefulareStatistics(ShiftsContainer shiftsContainer, SchedulareState algoResult, string algoString)
        {
            if (!_schedulareStatisticsList.ContainsKey(algoString))
            {
                _schedulareStatisticsList.AddOrUpdate(algoString, new SchedulareListStatistics());
            }
            var bfsSatisfaction = CommonLogic.GetPercentageOfSatisfaction(algoResult.Node.Value, shiftsContainer);
            _schedulareStatisticsList[algoString].SchedulareSatisfactionList.Add(new SchedulareSatisfaction(algoResult.Node.Value, bfsSatisfaction, shiftsContainer, algoResult.Weight));
        }

        private void PrintDebugData(ShiftsContainer shiftsContainer, SchedulareState state)
        {

            double percentageOfSatisfaction = CommonLogic.GetPercentageOfSatisfaction(state.Node.Value, shiftsContainer);
            Console.WriteLine($"Weight = {state.Weight}");
            Console.WriteLine($"Satisfaction = {percentageOfSatisfaction}");
            CommonLogic.PrintSchedulare(state.Node.Value, shiftsContainer);


            CommonLogic.ApeandToFile($"Weight = {state.Weight}", FILENAME);
            CommonLogic.ApeandToFile($"Satisfaction = {percentageOfSatisfaction}", FILENAME);
            string schedulareStateJson = JsonConvert.SerializeObject(state.Node.Value);
            CommonLogic.ApeandToFile(schedulareStateJson, FILENAME);
            string shiftsContainerJson = JsonConvert.SerializeObject(shiftsContainer);
            CommonLogic.ApeandToFile(shiftsContainerJson, FILENAME);
        }
    }
}

