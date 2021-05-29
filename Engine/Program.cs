using Engine.Algorithms;
using Engine.Factories;
using Engine.Interfaces;
using Engine.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Engine
{
    class Program
    {
        static Task Main(string[] args) =>
            CreateHostBuilder(args).Build().RunAsync();

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services.AddHostedService<Manager>()
                        .AddSingleton<IAlgo, BFS>()
                        .AddSingleton<IAlgo, WAStar>()
                        .AddSingleton<IAlgo, Tabu>()
                        .AddSingleton<IAlgo, TabuR>()
                        .AddSingleton<IAlgo, Rand>()
                        .AddSingleton<IAlgo, GreedyAlgorithm>()
                        .AddSingleton<IFactory, AlgoFactory>());

    }
}

