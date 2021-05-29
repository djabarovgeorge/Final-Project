using Engine.Abstracts;
using Engine.Algorithms;
using Engine.Extensions;
using Engine.Interfaces;
using Engine.Models;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Services
{
    public class Manager : BackgroundService
    {
        private ShiftSetter _constructsShift = new ConstructsShift();
        private Dictionary<string, SchedulareListStatistics> _schedulareStatisticsList = new Dictionary<string, SchedulareListStatistics>();
        private IFactory _factory;
        private const string FILENAME = "algodata";
        public Manager(IFactory factory)
        {
            _factory = factory;
        }
        public Manager()
        {
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CommonLogic.InitFileSuffix();

            var shiftsContainer = _constructsShift.Excute();

            var schedulare = new Schedulare(shiftsContainer);

            var randResult = RunRandAlgo(shiftsContainer, schedulare);

            RunCycle();

            return Task.FromResult(true);
        }

        private void RunCycle()
        {
            for (int executionAmount = 1; executionAmount < 1000; executionAmount++)
            {
                PrintOnStartInfo(executionAmount);

                RunHuristicMethods();

                UpdateSatisfactionAvarage();

                PrintStats();
            }
        }

        private void PrintOnStartInfo(int executionAmount)
        {
            var algoRunInSeconds = _factory.GetAll().FirstOrDefault().Value.ALGORITHM_RUN_TIME_SECONDS;

            CommonLogic.ApeandToFile($"EXECUTION_AMOUNT - {executionAmount}");
            CommonLogic.ApeandToFile($"ALGORITHM_RUN_TIME_SECONDS - {algoRunInSeconds}");
            Console.WriteLine($"EXECUTION_AMOUNT - {executionAmount}");
            Console.WriteLine($"ALGORITHM_RUN_TIME_SECONDS - {algoRunInSeconds}");
        }

        private void RunHuristicMethods()
        {
            var shiftsContainer = _constructsShift.Excute();

            var schedulare = new Schedulare(shiftsContainer);

            var randResult = RunRandAlgo(shiftsContainer, schedulare);

            foreach (var algo in _factory.GetAll())
            {
                RunAlgo(shiftsContainer, schedulare, randResult, algo);
            }
        }

        private SchedulareState RunRandAlgo(ShiftsContainer shiftsContainer, Schedulare schedulare)
        {
            var randAlgo = _factory.GetAlgo("Rand");
            var randResult = randAlgo.Execute(schedulare.DeepClone(), shiftsContainer);
            PrintDebugData(shiftsContainer, randResult, randAlgo.ToString());
            UpdateSchefulareStatistics(shiftsContainer, randResult, randAlgo.ToString());
            return randResult;
        }

        private void RunAlgo(ShiftsContainer shiftsContainer, Schedulare schedulare, SchedulareState randResult, KeyValuePair<string, IAlgo> algo)
        {
            SchedulareState result = null;

            if (algo.Key.ContainsContent("rand")) return;

            if (algo.Key.ContainsContent("tabu"))
                result = algo.Value.Execute(randResult.Node.Value.DeepClone(), shiftsContainer);
            else
                result = algo.Value.Execute(schedulare.DeepClone(), shiftsContainer);

            PrintDebugData(shiftsContainer, result, algo.Key.ToString());
            UpdateSchefulareStatistics(shiftsContainer, result, algo.Key.ToString());
        }

        private void PrintStats()
        {
            foreach (var algoList in _schedulareStatisticsList)
            {
                var algName = algoList.Key;

                CommonLogic.ApeandToFile(algName);

                CommonLogic.ApeandToFile($"Satisfaction Avarage = {algoList.Value.SatisfactionAvarage}");

                var satStdDev = Math.Round(algoList.Value.SchedulareSatisfactionList.Select(x => x.Satisfaction).StdDev(), 3);

                CommonLogic.ApeandToFile($"Satisfaction standard deviation = {satStdDev}");

                CommonLogic.ApeandToFile($"Weight Avarage = {algoList.Value.WeightAvarage}");

                var weightStdDev = Math.Round(algoList.Value.SchedulareSatisfactionList.Select(x => x.Weight).StdDev(), 3);

                CommonLogic.ApeandToFile($"Weight standard deviation = {weightStdDev}");

                CommonLogic.ApeandToFile($"Execute Time Avarage = {algoList.Value.ExecuteTimeAvarage}");

                CommonLogic.ApeandToFile($"Most Unfortunate Worker Avarage = { Math.Round(algoList.Value.MostUnfortunateWorkerPerAvarage, 3 )}%");

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
                algoList.Value.MostUnfortunateWorkerPerAvarage = algoList.Value.SchedulareSatisfactionList.Select(x => x.MostUnfortunateWorkerPer).Sum() / algoList.Value.SchedulareSatisfactionList.Count;
            }
        }

        private void UpdateSchefulareStatistics(ShiftsContainer shiftsContainer, SchedulareState algoResult, string algoString)
        {
            if (!_schedulareStatisticsList.ContainsKey(algoString))
            {
                _schedulareStatisticsList.AddOrUpdate(algoString, new SchedulareListStatistics());
            }
            var bfsSatisfaction = CommonLogic.GetPercentageOfSatisfaction(algoResult.Node.Value, shiftsContainer);
            _schedulareStatisticsList[algoString].SchedulareSatisfactionList.Add(new SchedulareSatisfaction(algoResult.Node.Value, bfsSatisfaction, shiftsContainer, algoResult.Weight, 
                                                                                                            algoResult.ExecuteTime, algoResult.MostUnfortunateWorkerPer));
        }

        private void PrintDebugData(ShiftsContainer shiftsContainer, SchedulareState state, string algoName)
        {
            Console.WriteLine(algoName);
            double percentageOfSatisfaction = CommonLogic.GetPercentageOfSatisfaction(state.Node.Value, shiftsContainer);
            Console.WriteLine($"Weight = {state.Weight}");
            Console.WriteLine($"Satisfaction = {percentageOfSatisfaction}");
            state.MostUnfortunateWorkerPer = CommonLogic.LocateAndPrintMostUnfortunateWorker(state.Node.Value, shiftsContainer);
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

